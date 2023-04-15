using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour
{
    public float deltaMove = 21;
    public float minPosX = -21;
    public float maxPosX = 21;

    Vector3 initPosition;
    Selected currentSelected;
    Vector3 positionVelocity;
    float posX;

    void Start()
    {
        initPosition = transform.position;
    }

    void Update()
    {
        // RAYCAST :
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                SelectObject(hit.transform);
            }
            else
            {
                UnselectObject();
            }
        }

        // MOVE CAMERA :
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveRight();
        transform.position = Vector3.SmoothDamp(transform.position, initPosition + Vector3.right * posX, ref positionVelocity, 0.3f);
    }

    public void MoveLeft()
    {
        posX -= deltaMove;
        posX = Mathf.Clamp(posX, minPosX, maxPosX);
    }
    public void MoveRight()
    {
        posX += deltaMove;
        posX = Mathf.Clamp(posX, minPosX, maxPosX);
    }

    void SelectObject(Transform _transform)
    {
        Selected newSelected = _transform.GetComponent<Selected>();
        if (newSelected == null)
            newSelected = _transform.gameObject.AddComponent<Selected>();

        if (newSelected == currentSelected)
            return;

        UnselectObject();

        currentSelected = newSelected;
        currentSelected.transform.SetParent(transform);
        currentSelected.timer = 0;
        currentSelected.isSelected = true;
    }

    void UnselectObject()
    {
        if (currentSelected == null)
            return;

        currentSelected.transform.SetParent(currentSelected.initParent);
        currentSelected.isSelected = false;
        currentSelected.reset = true;
        currentSelected.ResetDepth();
        currentSelected = null;
    }

}

public class Selected : MonoBehaviour
{
    public bool isSelected = false;
    public bool reset;
    public float timer;
    public float depth;

    public Transform initParent;
    public Vector3 initPosition;
    public Quaternion initRotation;

    Rigidbody rb;
    Vector3 positionVelocity;
    bool drag;
    Vector3 integral;

    void Awake()
    {
        initParent = transform.parent;
        initPosition = transform.position;
        initRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        ResetDepth();
    }

    void Update()
    {
        // SELECTION :
        if (isSelected)
        {
            depth -= (Input.mouseScrollDelta.y * 0.5f);
            depth = Mathf.Clamp(depth, 1f, 5f);
            Vector3 target = Vector3.forward * depth;

            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, target, ref positionVelocity, 0.2f, 50f);
            if (Input.GetMouseButtonDown(1))
            {
                reset = true;
                ResetDepth();
            }
            drag = Input.GetMouseButton(0);
            if (drag)
                reset = false;
        }
        else
        {
            Vector3 target = initPosition;
            transform.position = Vector3.SmoothDamp(transform.position, target, ref positionVelocity, 0.2f, 50f);

            timer += Time.deltaTime;
            if (timer > 5f)
                Destroy(this);
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        if (reset)
        {
            DoRotationPID(initRotation, rb, ref integral, 350, 0, 50);
        }
        else if (isSelected && drag)
        {
            if (drag)
            {
                float x = Input.GetAxis("Mouse X") * 100 * Time.fixedDeltaTime;
                float y = Input.GetAxis("Mouse Y") * 100 * Time.fixedDeltaTime;
                rb.AddTorque(-transform.parent.up * x, ForceMode.VelocityChange);
                rb.AddTorque(transform.parent.right * y, ForceMode.VelocityChange);
            }
        }
    }

    public void ResetDepth()
    {
        depth = 2.5f;
    }

    void DoRotationPID(Quaternion targetRotation, Rigidbody rigidbody, ref Vector3 integral, float kp, float ki, float kd)
    {
        //if (float.IsNaN(target.x)) return;
        //if (float.IsNaN(target.y)) return;
        //if (float.IsNaN(target.z)) return;

        Quaternion desiredRotation = targetRotation;
        Vector3 x;
        float xMag;
        Quaternion q = desiredRotation * Quaternion.Inverse(rigidbody.transform.rotation);

        // Q can be the-long-rotation-around-the-sphere eg. 350 degrees
        // We want the equivalant short rotation eg. -10 degrees
        // Check if rotation is greater than 190 degees == q.w is negative
        if (q.w < 0)
        {
            // Convert the quaterion to eqivalent "short way around" quaterion
            q.x = -q.x;
            q.y = -q.y;
            q.z = -q.z;
            q.w = -q.w;
        }
        q.ToAngleAxis(out xMag, out x);
        x.Normalize();
        x *= Mathf.Deg2Rad;

        integral += x * xMag * Time.fixedDeltaTime;

        Vector3 P = kp * x * xMag;
        Vector3 I = ki * integral;
        Vector3 D = kd * -rigidbody.angularVelocity;
        Vector3 pidv = P + I + D;

        //Quaternion rotInertia2World = RB.inertiaTensorRotation * transform.rotation;
        //pidv = Quaternion.Inverse(rotInertia2World) * pidv;
        //pidv.Scale(RB.inertiaTensor);
        //pidv = rotInertia2World * pidv;
        rigidbody.AddTorque(pidv, ForceMode.Acceleration);
    }
}