using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform Player;

    [SerializeField]
    Transform CameraEncounterPosition;

    [SerializeField]
    Transform CameraGamePosition;

    [SerializeField]
    Camera Camera;

    Vector3 targetPos;
    Quaternion targetRot;

    [SerializeField]
    float damping = 1;

    protected void Start()
    {
        GameStateMediator.Instance.EventStateUpdated += OnStateUpdate;
        Camera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        transform.position = Player.position;

        var t = damping * Time.deltaTime;

        Camera.transform.localPosition = Vector3.Lerp(Camera.transform.localPosition, targetPos, t);

        Camera.transform.localRotation = Quaternion.Slerp(
            Camera.transform.localRotation,
            targetRot,
            t
        );
    }

    private void OnStateUpdate(GameState gs)
    {
        if (gs.HasEncounter)
        {
            targetPos = CameraEncounterPosition.localPosition;
            targetRot = CameraEncounterPosition.localRotation;
        }
        else
        {
            targetPos = CameraGamePosition.localPosition;
            targetRot = CameraGamePosition.localRotation;
        }
    }
}
