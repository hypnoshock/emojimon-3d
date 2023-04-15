using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    protected void Start()
    {
        GameStateMediator.Instance.EventStateUpdated += OnStateUpdate;
    }

    private void OnStateUpdate(GameState gs)
    {
        var pos = gs.PlayerPosition;
        this.transform.SetLocalPositionAndRotation(
            new Vector3(pos.X, this.transform.position.y, -pos.Y),
            Quaternion.identity
        );
    }
}
