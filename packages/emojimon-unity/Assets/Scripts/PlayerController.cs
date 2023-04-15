using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private Vector3 targetPos;
    public float damping = 1;
    private Animator animator;

    protected void Start()
    {
        animator = GetComponentsInChildren<Animator>()[0];

        GameStateMediator.Instance.EventStateUpdated += OnStateUpdate;
    }

    private void OnStateUpdate(GameState gs)
    {
        targetPos = new Vector3(gs.PlayerPosition.X, 0, -gs.PlayerPosition.Y);
    }

    private void OnStateUpdateOld(GameState gs)
    {
        var pos = gs.PlayerPosition;
        this.transform.SetLocalPositionAndRotation(
            new Vector3(pos.X, 0, -pos.Y),
            Quaternion.identity
        );
    }

    private void Update()
    {
        this.transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPos,
            damping * Time.deltaTime
        );

        var remainingDist = Vector3.Distance(this.transform.localPosition, targetPos);
        if (remainingDist > 0.1)
        {
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }
}
