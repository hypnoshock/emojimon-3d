using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

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
        #if UNITY_EDITOR
        DebugUpdate();
        #endif
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

    // Some dummy state change on keys in teh absence of Bridge being implemented
    private void DebugUpdate()
    {
        // modify state directly bur still call UpdateState to trigger event

        Position move = new Position() { X=0, Y=0 };
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            move.X = -1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            move.X = 1;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            move.Y = -1;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            move.Y = 1;
        }

        if (move.X !=0 || move.Y != 0)
        {
            Position pos = gameState.PlayerPosition;
            pos.X += move.X;
            pos.Y += move.Y;
            gameState.PlayerPosition = pos;
            UpdateState(gameState);
        }

        // encounter
        if (Input.GetKeyDown(KeyCode.E)) {
            gameState.HasEncounter = true;
            gameState.MonsterType = UnityEngine.Random.Range(1,4);
            var encounter = new Encounter() 
            {
                Monsters = new List<string>() {"DUMMY"},
                ActionCount = 1
            };
            gameState.Encounter = encounter;
            UpdateState(gameState);
        }

        // runaway
        if (Input.GetKeyDown(KeyCode.R)) {
            gameState.HasEncounter = false;
            UpdateState(gameState);
        }

        // other player
        if (Input.GetKeyDown(KeyCode.O)) {
            gameState.OtherPlayers.Add(
                new OtherPlayer() 
                {
                    Entity = Time.frameCount,
                    Position = new Position() 
                    { 
                        X = UnityEngine.Random.Range(0,20), 
                        Y = UnityEngine.Random.Range(0,20) 
                    },
                });
            UpdateState(gameState);
        }
    }

    private void UpdateState(GameState state)
    {
        gameState = state;
        // _account = state.PlayerAddr as string;
        _hasStateUpdated = true;

        LogState(state);
    }

    private void LogState(GameState gameState)
    {
        Debug.Log("Unity: GameStateMediator::UpdateState()");
        Debug.Log(
            $"Unity: GameStateMediator:: playerPosition: {gameState.PlayerPosition.X}, {gameState.PlayerPosition.Y}"
        );
        Debug.Log($"Unity: GameStateMediator:: CanSpawn: {gameState.CanSpawn}");
        Debug.Log($"Unity: GameStateMediator:: OtherPlayers.Count: {gameState.OtherPlayers.Count}");

        // Forgot how to do LINQs equivalent to reduce
        var tileVals = "";
        for (var i = 0; i < gameState.Map.TerrainValues.Count; i++)
        {
            var tv = gameState.Map.TerrainValues[i];
            tileVals += $"{tv.Value},";
        }
        Debug.Log($"Unity: GameStateMediator:: tileVals: ${tileVals}");

        for (var i = 0; i < gameState.OtherPlayers.Count; i++)
        {
            var pos = gameState.OtherPlayers[i].Position;
            var entity = gameState.OtherPlayers[i].Entity;
            Debug.Log($"Unity: GameStateMediator:: OtherPlayer: entity: {entity} pos: {pos.X}, {pos.Y}");
        }

        Debug.Log($"Unity: GameStateMediator:: HasEncounter: {gameState.HasEncounter}");

        if (gameState.HasEncounter)
        {
            var encounter = gameState.Encounter;
            Debug.Log($"Unity: GameStateMediator:: encounter.ActionCount: {encounter.ActionCount}");

            for (var i = 0; i < encounter.Monsters.Count; i++)
            {
                var monster = encounter.Monsters[i];
                Debug.Log($"Unity: GameStateMediator:: Monster: {monster}");
            }

            Debug.Log($"Unity: GameStateMediator:: monsterType: {gameState.MonsterType}");
        }
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
            "Bridge not yet implemented. For now using hard coded game state for testing"
        );

        OnState(DebugState.GameStateJSON);
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
