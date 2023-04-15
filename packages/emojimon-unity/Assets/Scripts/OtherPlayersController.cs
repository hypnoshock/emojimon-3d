using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayersController : MonoBehaviour
{
    [SerializeField] GameObject OtherPlayerPrefab;
    Dictionary<int, OtherPlayerController> InstantiatedPlayers;

    protected void Start()
    {
        InstantiatedPlayers = new Dictionary<int, OtherPlayerController>();
        GameStateMediator.Instance.EventStateUpdated += OnStateUpdate;
    }

    private void OnStateUpdate(GameState gs)
    {
        foreach (var otherPlayer in gs.OtherPlayers)
        {
            var targetPos = new Vector3(
                otherPlayer.Position.X - 10, 0, 10 - otherPlayer.Position.Y);

            if (!InstantiatedPlayers.ContainsKey(otherPlayer.Entity)) {
               var obj = GameObject.Instantiate(
                OtherPlayerPrefab, 
                transform ) as GameObject;
               InstantiatedPlayers[otherPlayer.Entity] = obj.GetComponent<OtherPlayerController>();
               InstantiatedPlayers[otherPlayer.Entity].transform.position = targetPos;
            }
             
            InstantiatedPlayers[otherPlayer.Entity].SetTarget(targetPos);
        }
    }
}
