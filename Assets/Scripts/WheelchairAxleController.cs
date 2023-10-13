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
    public int casterSolverIterations = 20;
    public float casterHingeSpringSpeedThreshold = 0.1f;
    public float hingeSpringForceMultiplier = 1.0f;

    private HingeJoint leftCasterHinge, rightCasterHinge;
    private Wheel leftWheel, rightWheel, leftCasterWheel, rightCasterWheel;


    public float wheelGripRadius = 0.15f; // the area in which you can grab the wheels
    public float wheelGripWidth = 0.2f; // the area in which you can grab the wheels
    public float breakingThreshold = 20.0f; // angular velocity (deg/s) at which the wheels start breaking
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
        rightCasterWheel = new Wheel(rightCaster, rightCaster.GetComponent<SphereCollider>(), rightCaster.GetComponent<Rigidbody>(), rightCaster.GetComponent<HingeJoint>());

        leftCasterHinge = leftHinge.GetComponent<HingeJoint>();
        rightCasterHinge = rightHinge.GetComponent<HingeJoint>();

        leftCasterWheel.rb.solverIterations = casterSolverIterations;
        rightCasterWheel.rb.solverIterations = casterSolverIterations;
        leftHinge.GetComponent<Rigidbody>().solverIterations = casterSolverIterations;
        rightHinge.GetComponent<Rigidbody>().solverIterations = casterSolverIterations;
    }

    void FixedUpdate()
    {
        Wheel[] wheels = { leftWheel, rightWheel };
        float wheelRadius = wheels[0].collider.radius;
        Vector2 wheelInput = Vector2.zero;
        bool[] gripping = new bool[2] { false, false };

        // Obtain wheel velocities from input
        if (keyboardControl)
        {
            wheelInput = 50.0f * new Vector2(Input.GetAxisRaw("Left wheel"), Input.GetAxisRaw("Right wheel"));
            gripping[0] = wheelInput.x != 0.0f;
            gripping[1] = wheelInput.y != 0.0f;
        }
        else
        {
            SteamVR_Action_Pose pose = SteamVR_Input.GetPoseAction("Pose");
            SteamVR_Action_Boolean grip = SteamVR_Input.GetBooleanAction("GrabGrip");

            SteamVR_Input_Sources[] inputHands = { SteamVR_Input_Sources.LeftHand, SteamVR_Input_Sources.RightHand };
            for (int i = 0; i < 2; i++)
            {
                SteamVR_Input_Sources inputHand = inputHands[i];

                // Check whether hands are close enough to wheels, both should be in the same local space as they are parented by wheelchair_root
                Vector3 handPos = player.transform.localPosition + pose.GetLocalPosition(inputHand);
                Vector3 wheelPos = wheels[i].collider.transform.localPosition;

                // Find distance from center of weel in the radial direction
                float wheelRadialDist = Vector2.Distance(new Vector2(handPos.y, handPos.z), new Vector2(wheelPos.y, wheelPos.z));
                // Find lateral distance
                float wheelLateralDist = Mathf.Abs(handPos.x - wheelPos.x);
                // Find tangent direction of wheel at hand position
                Vector3 wheelTangentDir = Vector3.Cross(Vector3.Normalize(handPos - wheelPos), -Vector3.right);
                // Find applied velocity from the controller in tangent direction
                float wheelTangentVel = Vector3.Dot(pose.GetVelocity(inputHand), wheelTangentDir);

                // Find current rb angular velocity and angular velocity apply from input tangent velocity
                float angularvel = wheelTangentVel / (2 * Mathf.PI * wheelRadialDist) * 360;

                if (Mathf.Abs(wheelRadialDist - wheelRadius) < wheelGripRadius && wheelLateralDist < wheelGripWidth)
                {
                    haptics.Execute(0, Time.fixedDeltaTime, hapticFrequency, hapticStrength, inputHands[i]);

                    if (grip.GetState(inputHand))
                    {
                        gripping[i] = true;
                        if (Mathf.Abs(angularvel) > breakingThreshold)
                            wheelInput[i] = angularvel;
                        else
                            wheelInput[i] = 0.0f;
                    }
                }
            }
        }


        for (int i = 0; i < 2; i++)
        {
            if (gripping[i])
            {
                wheels[i].joint.useMotor = true;
                JointMotor motor = wheels[i].joint.motor;

                float currAngularVel = Mathf.Rad2Deg * (wheels[i].wheel.transform.worldToLocalMatrix * wheels[i].rb.angularVelocity).x;
                motor.targetVelocity = currAngularVel + wheelInput[i];
                wheels[i].joint.motor = motor;
            }
            else
            {
                wheels[i].joint.useMotor = false;
            }
        }

        // Set caster wheel angle from estimated velocity in local zx-plane from wheel input
        Vector2 estVelocity = (new Vector2(wheelInput.x + wheelInput.y, wheelInput.x - wheelInput.y) / 360) * 2 * Mathf.PI * wheelRadius;
        float estSpeed = estVelocity.magnitude;
        float casterWheelAngle = Mathf.Rad2Deg * Mathf.Atan2(estVelocity.y, estVelocity.x);        

        HingeJoint[] hinges = { leftCasterHinge, rightCasterHinge };
        //leftCasterHinge.connectedAnchor = Vector3.zero;
        //rightCasterHinge.connectedAnchor = Vector3.zero;
        if (estSpeed > casterHingeSpringSpeedThreshold)
        {
            foreach (var hinge in hinges)
            {
                hinge.useSpring = true;
                JointSpring spring = hinge.spring;

                spring.targetPosition = casterWheelAngle;
                spring.spring = estSpeed * hingeSpringForceMultiplier;
                hinge.spring = spring;
            }
        }
        else
        {
            leftCasterHinge.useSpring = false;
            rightCasterHinge.useSpring = false;
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
