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
    public float maxBrakeTorque;

    private Rigidbody rb;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();

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
        ApplyLocalPositionToVisuals(axleInfo.wheels[(int)Wheels.RearLeft]);
        ApplyLocalPositionToVisuals(axleInfo.wheels[(int)Wheels.RearRight]);
        ApplyLocalPositionToVisuals(axleInfo.wheels[(int)Wheels.CasterLeft]);
        ApplyLocalPositionToVisuals(axleInfo.wheels[(int)Wheels.CasterRight]);
    }

    void FixedUpdate()
    {
        SteamVR_Action_Pose pose = SteamVR_Input.GetPoseAction("Pose");
        SteamVR_Action_Boolean grip = SteamVR_Input.GetBooleanAction("GrabGrip");

        SteamVR_Input_Sources[] inputHands = { SteamVR_Input_Sources.LeftHand, SteamVR_Input_Sources.RightHand };
        for (int i = 0; i < 2; i++)
        {
            float torque = 0;
            float brakeTorque = 0;
            SteamVR_Input_Sources inputHand = inputHands[i];
            if (grip.GetStateUp(inputHand)){GetVelocity(inputHand, i); }
            if (grip.GetStateDown(inputHand)) {
                if (pose.GetVelocity(inputHand).z > -0.1 && pose.GetVelocity(inputHand).z < 0.1)
                {
                    axleInfo.wheels[i].wheelDampingRate = 20;
                }
            }

        }
    }
    void GetVelocity(SteamVR_Input_Sources inputHand, int i)
    {
        Debug.Log("here");
        float highestVelocity = 0f;
        SteamVR_Action_Boolean grip = SteamVR_Input.GetBooleanAction("GrabGrip");
        SteamVR_Action_Pose pose = SteamVR_Input.GetPoseAction("Pose");


        //float velocity = pose.GetVelocity(inputHand).z;
        //if (Math.Abs(velocity) > 0.1f)
        //{
        //    Debug.Log(axleInfo.wheels[1].rpm);
        //    torque = maxTorque;

        //}
        //else
        //{

        //    axleInfo.wheels[i].brakeTorque = brakeTorque;
        //}

        //axleInfo.wheels[i].motorTorque = torque;
        if(pose.GetVelocity(inputHand).z < -0.1)
        {
            highestVelocity = -0.2f;
            axleInfo.wheels[i].wheelDampingRate = 0.25f;
        }
        else if(pose.GetVelocity(inputHand).z > 0.1)
        {
            highestVelocity = 0.2f;
            axleInfo.wheels[i].wheelDampingRate = 0.25f;
        }
 
        

        axleInfo.wheels[i].motorTorque = maxTorque * highestVelocity; 

    }
}

enum Wheels
{
    RearLeft, RearRight, CasterLeft, CasterRight
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider[] wheels = new WheelCollider[4];
}
