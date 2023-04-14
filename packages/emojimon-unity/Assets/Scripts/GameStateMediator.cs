using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Emojimon
{
    struct GenericMessage
    {
        public string msg;
    }

    struct DispatchMessage
    {
        public string msg;
        public string action;
        public object[] args;
    }

    // TODO: The json schema has this structure defined, find a way to export that structure instead of definiing it manually here
    public class GameState
    {
        [Newtonsoft.Json.JsonProperty(
            "playerPos",
            Required = Newtonsoft.Json.Required.DisallowNull,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
        )]
        public Vector3Int PlayerPos { get; set; }
    }

    public class GameStateMediator : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void SendMessageRPC(string msgJSON);

        [DllImport("__Internal")]
        private static extern void UnityReadyRPC();

        public static GameStateMediator Instance;

        public GameState gameState { get; private set; }

        [SerializeField]
        private string _account;

        private bool _hasStateUpdated;

        // -- EVENTS
        public Action<GameState> EventStateUpdated;

        // -- //

        protected void Awake()
        {
            Instance = this;
        }

        protected void Start()
        {
            Debug.Log("GameStateMediator::Start()");

#if UNITY_EDITOR
            StartNodeProcess();
#elif UNITY_WEBGL
            UnityReadyRPC();
#endif
        }

        protected void Update()
        {
            // state events get dispatched in the update loop so that the event happens in the main thread which is required
            // by anything that renders anything as a side effect of the state update
            if (_hasStateUpdated)
            {
                _hasStateUpdated = false;
                if (EventStateUpdated != null)
                {
                    EventStateUpdated.Invoke(gameState);
                }
            }
        }

        private void UpdateState(GameState state)
        {
            gameState = state;
            // _account = state.PlayerAddr as string;
            _hasStateUpdated = true;
        }

#if UNITY_EDITOR

        // -- Dawnseekers Bridge node.js thread
        private string _nodePath;
        private string _privateKey;

        private Thread _nodeJSThread;
        private System.Diagnostics.Process _nodeJSProcess;

        protected void OnDestroy()
        {
            KillNodeProcess();
        }

        /* TODO: If time during the hackathon implement a bridge to MUD using node.js */
        private void StartNodeProcess()
        {
            Debug.Log("GameStateMediator::StartNodeProcess()");

            Debug.LogError(
                "Bridge not yet implemented. Without this there is no state update and therefore nothing will be rendered"
            );

            // _nodePath = EmojimonDevSettings.instance.NodePath;
            // _privateKey = EmojimonDevSettings.instance.PrivateKey;

            // try
            // {
            //     _nodeJSThread = new Thread(new ThreadStart(NodeProcessThread));
            //     _nodeJSThread.Start();
            // }
            // catch (Exception e)
            // {
            //     Debug.Log(e.Message);
            // }
        }

        private void KillNodeProcess()
        {
            if (_nodeJSProcess != null)
            {
                _nodeJSProcess.Kill();
            }
        }

        public void NodeProcessThread()
        {
            if (MudDevSettings.instance.NodePath == "")
            {
                Debug.LogError(
                    "GameStateMediator:: Node path not set. Make sure the absolute path to node is set in the Edit > Project Settings > Dawnseekers panel. Path can be found using `which node`"
                );
                return;
            }

            Debug.Log(
                $"GameStateMediator::NodeProcessThread() Starting DawnseekersBridge \nNodePath: {MudDevSettings.instance.NodePath} \nPrivKey: {MudDevSettings.instance.PrivateKey}"
            );

            _nodeJSProcess = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    WorkingDirectory = "../mud-bridge",
                    FileName = _nodePath,
                    Arguments = "./dist/index.js " + _privateKey,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                }
            };

            try
            {
                _nodeJSProcess.Start();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError(
                    "GameStateMediator:: Unable to start bridge. Please check your Node path and make sure that the bridge has been built with npm run build"
                );
                return;
            }

            while (!_nodeJSProcess.StandardOutput.EndOfStream)
            {
                var line = _nodeJSProcess.StandardOutput.ReadLine();

                if (line.Length > 0 && line[0] == '{')
                {
                    try
                    {
                        var state = JsonConvert.DeserializeObject<GameState>(line);
                        UpdateState(state);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("MudBridge:\n" + line);
                        Debug.LogError(e);
                    }
                }
                else
                {
                    Debug.Log("MudBridge:\n" + line);
                }
            }

            Debug.Log("Node process exiting");

            _nodeJSProcess.WaitForExit();

            Debug.Log("Kill thread");
        }
        // -- End of Node.js process -- //

#endif

        // -- MESSAGE OUT

        public void MovePlayer(string account, Vector3Int pos)
        {
            DispatchAction("movePlayer", account, pos.x, pos.y);
        }

        public void DispatchAction(string action, params object[] args)
        {
            var msg = new DispatchMessage
            {
                msg = "dispatch",
                action = action,
                args = args
            };
            var json = JsonConvert.SerializeObject(msg);
            SendMessage(json);
        }

        private new void SendMessage(string json)
        {
#if UNITY_EDITOR
            _nodeJSProcess.StandardInput.WriteLine(json);
#elif UNITY_WEBGL
            // -- Send interaction up to react client
            SendMessageRPC(json);
#endif
        }

        // -- MESSAGE IN
        private string _prevStateJson = "";

        public void OnState(string stateJson)
        {
            if (_prevStateJson == stateJson)
                return;

            try
            {
                var state = JsonConvert.DeserializeObject<GameState>(stateJson);
                UpdateState(state);
            }
            catch (Exception e)
            {
                Debug.Log("GameStateMediator::OnState():\n" + stateJson);
                Debug.LogError(e);
            }
        }
    }
}
