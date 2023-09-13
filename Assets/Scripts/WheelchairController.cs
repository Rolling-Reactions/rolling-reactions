using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Valve.VR;
using Valve.VR.InteractionSystem;

public class WheelchairController : MonoBehaviour
{
    public AxleInfo axleInfo; // the information about each individual axle
    public float maxTorque; // maximum torque the motor can apply to wheel

    private Hand hand;

    private void Start()
    {
        hand = GetComponent<Hand>();
    }

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
        SteamVR_Action_Boolean state = SteamVR_Input.GetBooleanAction("GrabGrip");
        Debug.Log(state.GetState(SteamVR_Input_Sources.Any));
        float torqueLeft = maxTorque * Convert.ToInt32(state.GetState(SteamVR_Input_Sources.LeftHand));
        float torqueRight = maxTorque * Convert.ToInt32(state.GetState(SteamVR_Input_Sources.RightHand));

        axleInfo.leftWheel.motorTorque = torqueLeft;
        axleInfo.rightWheel.motorTorque = torqueRight;
    }

    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public WheelCollider leftSupport;
        public WheelCollider rightSupport;
    }
}