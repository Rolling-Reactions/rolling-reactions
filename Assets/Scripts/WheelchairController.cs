using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Valve.VR;
using Valve.VR.InteractionSystem;

public class WheelchairController : MonoBehaviour
{
    public Player player;

    public AxleInfo axleInfo; // the information about each individual axle
    public float maxTorque; // maximum torque the motor can apply to wheel
    public float maxBrakeTorque;

    public float wheelGripRadius = 0.15f; // the area in which you can grab the wheels
    public float wheelGripWidth = 0.2f; // the area in which you can grab the wheels
    public float hapticFrequency = 100.0f;
    public float hapticStrength = 0.1f;
    public SteamVR_Action_Vibration haptics;

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
        // Get input and move wheels
        SteamVR_Action_Pose pose = SteamVR_Input.GetPoseAction("Pose");
        SteamVR_Action_Boolean grip = SteamVR_Input.GetBooleanAction("GrabGrip");

        SteamVR_Input_Sources[] inputHands = { SteamVR_Input_Sources.LeftHand, SteamVR_Input_Sources.RightHand };
        for (int i = 0; i < 2; i++)
        {
            // Check whether hands are close enough to wheels, both should be in the same local space as they are parented by wheelchair_root
            Vector3 handPos = player.transform.localPosition + pose.GetLocalPosition(inputHands[i]);
            Vector3 wheelPos = axleInfo.wheels[i].transform.localPosition;
            float wheelRadius = axleInfo.wheels[i].radius;

            // Find distance in tangent plane
            float wheelTangentDist = Vector2.Distance(new Vector2(handPos.y, handPos.z), new Vector2(wheelPos.y, wheelPos.z));
            // Find lateral distance
            float wheelLateralDist = Mathf.Abs(handPos.x - wheelPos.x);

            Debug.Log(inputHands[i] + " " + Mathf.Abs(wheelTangentDist - wheelRadius) + " " + wheelLateralDist);

            if (Mathf.Abs(wheelTangentDist - wheelRadius) < wheelGripRadius && wheelLateralDist < wheelGripWidth)
            {
                haptics.Execute(0, Time.fixedDeltaTime, hapticFrequency, hapticStrength, inputHands[i]);

                float torque = 0;
                float brakeTorque = 0;
                SteamVR_Input_Sources inputHand = inputHands[i];
                float velocity = pose.GetVelocity(inputHand).z;

                if (grip.GetState(inputHand))
                {
                    if (Math.Abs(velocity) > 0.05f)
                    {
                        torque = maxTorque * velocity;
                    }
                    else
                    {
                        //brakeTorque = maxBrakeTorque;
                    }
                }

                axleInfo.wheels[i].motorTorque = torque;
                axleInfo.wheels[i].brakeTorque = brakeTorque;
            }
        }
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
