using UnityEngine;
using System.Collections.Generic;

public class OtherPlayerController : MonoBehaviour
{
    private Vector3 targetPos;
    [SerializeField] private GameObject physicalCharacter;
    private Rigidbody rb;
    Vector3 bounceBackVel;

    public void SetTarget(Vector3 pos)
    {
        rb = physicalCharacter.GetComponent<Rigidbody>();
        targetPos = pos;
    }

    private void Update()
    {
        if (rb == null)
            return;

        var offset = targetPos - rb.position;
        var dot = Vector3.Dot(targetPos, rb.velocity);
        offset.y = 0;
        
        if (offset.magnitude > 0.3f)
        {
            float strength = Mathf.Min(offset.magnitude, 1f);
            strength *= strength;
            Vector3 force = offset * strength;
            rb.AddForce(force);
        }
        else 
        {
            rb.velocity = Vector3.zero;
        }
    }
}
