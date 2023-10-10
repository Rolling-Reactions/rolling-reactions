using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Valve.VR;
using Valve.VR.InteractionSystem;

public class WheelchairAxleController : MonoBehaviour
{
    public bool keyboardControl = false;
    public Player player;

    public GameObject left, right, leftCaster, rightCaster, leftHinge, rightHinge;
    private Wheel leftWheel, rightWheel, leftCasterWheel, rightCasterWheel;


    public float wheelGripRadius = 0.15f; // the area in which you can grab the wheels
    public float wheelGripWidth = 0.2f; // the area in which you can grab the wheels
    public float hapticFrequency = 100.0f;
    public float hapticStrength = 0.1f;
    public SteamVR_Action_Vibration haptics;

    private Rigidbody rb;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        leftWheel = new Wheel(left, left.GetComponent<SphereCollider>(), left.GetComponent<Rigidbody>(), left.GetComponent<HingeJoint>());
        rightWheel = new Wheel(right, right.GetComponent<SphereCollider>(), right.GetComponent<Rigidbody>(), right.GetComponent<HingeJoint>());
        leftCasterWheel = new Wheel(leftCaster, leftCaster.GetComponent<SphereCollider>(), leftCaster.GetComponent<Rigidbody>(), leftCaster.GetComponent<HingeJoint>());

        leftCasterWheel.rb.solverIterations = 50;
        rightCasterWheel.rb.solverIterations = 50;
        leftHinge.GetComponent<Rigidbody>().solverIterations = 50;
        rightHinge.GetComponent<Rigidbody>().solverIterations = 50;
    }

    void FixedUpdate()
    {
        Wheel[] wheels = { leftWheel, rightWheel };
        if (keyboardControl)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Left wheel"), Input.GetAxisRaw("Right wheel"));
            Debug.Log(input);
            for (int i = 0; i < 2; i++)
            {
                if (input[i] != 0.0f)
                {
                    wheels[i].joint.useMotor = true;
                    JointMotor motor = wheels[i].joint.motor;
                    motor.targetVelocity = 50.0f * input[i];
                    wheels[i].joint.motor = motor;
                }
                else
                {
                    wheels[i].joint.useMotor = false;
                }
            }
        }
        else
        {
            // Get input and move wheels
            SteamVR_Action_Pose pose = SteamVR_Input.GetPoseAction("Pose");
            SteamVR_Action_Boolean grip = SteamVR_Input.GetBooleanAction("GrabGrip");

            SteamVR_Input_Sources[] inputHands = { SteamVR_Input_Sources.LeftHand, SteamVR_Input_Sources.RightHand };
            for (int i = 0; i < 2; i++)
            {
                // Check whether hands are close enough to wheels, both should be in the same local space as they are parented by wheelchair_root
                Vector3 handPos = player.transform.localPosition + pose.GetLocalPosition(inputHands[i]);
                Vector3 wheelPos = wheels[i].collider.transform.localPosition;
                float wheelRadius = wheels[i].collider.radius;

                // Find distance in tangent plane
                float wheelTangentDist = Vector2.Distance(new Vector2(handPos.y, handPos.z), new Vector2(wheelPos.y, wheelPos.z));
                // Find lateral distance
                float wheelLateralDist = Mathf.Abs(handPos.x - wheelPos.x);

                SteamVR_Input_Sources inputHand = inputHands[i];
                Vector3 wheelTangentDir = Vector3.Cross(Vector3.Normalize(handPos - wheelPos), -Vector3.right);
                float wheelTangentVel = Vector3.Dot(pose.GetVelocity(inputHand), wheelTangentDir);
                float angularvel = wheelTangentVel / (2 * Mathf.PI * wheels[i].collider.radius) * 360;

                if (Mathf.Abs(wheelTangentDist - wheelRadius) < wheelGripRadius && wheelLateralDist < wheelGripWidth)
                {
                    haptics.Execute(0, Time.fixedDeltaTime, hapticFrequency, hapticStrength, inputHands[i]);

                    if (grip.GetState(inputHand))
                    {
                        wheels[i].joint.useMotor = true;
                        JointMotor motor = wheels[i].joint.motor;

                        if (Mathf.Abs(angularvel) > 20)
                            motor.targetVelocity = Mathf.Rad2Deg * (wheels[i].wheel.transform.worldToLocalMatrix * wheels[i].rb.angularVelocity).x + angularvel;
                        else
                            motor.targetVelocity = 0;

                        wheels[i].joint.motor = motor;
                    }
                    else
                    {
                        wheels[i].joint.useMotor = false;
                    }
                }
                else
                {
                    wheels[i].joint.useMotor = false;
                }
            }
        }
    }

    struct Wheel
    {
        public GameObject wheel;
        public SphereCollider collider;
        public Rigidbody rb;
        public HingeJoint joint;

        public Wheel(GameObject wheel, SphereCollider collider, Rigidbody rb, HingeJoint joint)
        {
            this.wheel = wheel;
            this.collider = collider;
            this.rb = rb;
            this.joint = joint;
        }
    }
}
