using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    // We'll probably get rid of this by putting the caracter in a parent object and setting the paren'ts
    // position to the desired offset but I'm not sure what structure we're going with yet so I'll use an offset
    public Vector3Int Offset;

    protected void Start()
    {
        GameStateMediator.Instance.EventStateUpdated += OnStateUpdate;
    }

    private void OnStateUpdate(GameState gs)
    {
        var pos = gs.PlayerPosition;
        this.transform.SetLocalPositionAndRotation(
            new Vector3(pos.X + Offset.x, this.transform.position.y, -pos.Y + Offset.z),
            Quaternion.identity
        );
    }
}
