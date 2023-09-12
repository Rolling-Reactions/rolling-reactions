using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WheelchairController : MonoBehaviour
{
    public AxleInfo axleInfo; // the information about each individual axle
    public float maxTorque; // maximum torque the motor can apply to wheel

    // finds the corresponding visual wheel
    // correctly applies the transform
    void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    void Update()
    {
        ApplyLocalPositionToVisuals(axleInfo.leftWheel);
        ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        ApplyLocalPositionToVisuals(axleInfo.leftSupport);
        ApplyLocalPositionToVisuals(axleInfo.rightSupport);
    }

    void FixedUpdate()
    {
        float torqueLeft = maxTorque * Input.GetAxis("Left wheel");
        float torqueRight = maxTorque * Input.GetAxis("Right wheel");

        axleInfo.leftWheel.motorTorque = torqueLeft;
        axleInfo.rightWheel.motorTorque = torqueRight;

        Debug.Log(torqueLeft + " " + torqueRight);
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public WheelCollider leftSupport;
    public WheelCollider rightSupport;
}