using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine.XR;

[RequireComponent(typeof(InputData))]
public class WheelchairAxleController : MonoBehaviour
{
    public bool keyboardControl = false;
    public GameObject player;

    private InputData inputData;

    public GameObject left, right, leftCaster, rightCaster, leftHinge, rightHinge;
    public Transform visualLeftCaster, visualRightCaster;
    public int casterSolverIterations = 20;
    public float casterHingeSpringSpeedThreshold = 0.1f;
    public float hingeSpringForceMultiplier = 1.0f;

    private HingeJoint leftCasterHinge, rightCasterHinge;
    private Wheel leftWheel, rightWheel, leftCasterWheel, rightCasterWheel;

    private Wheel[] wheels;
    private float wheelRadius;
    HingeJoint[] hinges;



    public float wheelGripRadius = 0.15f; // the area in which you can grab the wheels
    public float wheelGripWidth = 0.2f; // the area in which you can grab the wheels
    public float breakingThreshold = 20.0f; // angular velocity (deg/s) at which the wheels start breaking
    public float hapticFrequency = 100.0f;
    public float hapticStrength = 0.1f;

    private Rigidbody rb;

    void Start()
    {
        inputData = this.GetComponent<InputData>();
        rb = this.GetComponent<Rigidbody>();
        leftWheel = new Wheel(left, left.GetComponent<SphereCollider>(), left.GetComponent<Rigidbody>(), left.GetComponent<HingeJoint>());
        rightWheel = new Wheel(right, right.GetComponent<SphereCollider>(), right.GetComponent<Rigidbody>(), right.GetComponent<HingeJoint>());
        leftCasterWheel = new Wheel(leftCaster, leftCaster.GetComponent<SphereCollider>(), leftCaster.GetComponent<Rigidbody>(), leftCaster.GetComponent<HingeJoint>());
        rightCasterWheel = new Wheel(rightCaster, rightCaster.GetComponent<SphereCollider>(), rightCaster.GetComponent<Rigidbody>(), rightCaster.GetComponent<HingeJoint>());

        wheels = new Wheel[2] { leftWheel, rightWheel };
        wheelRadius = wheels[0].collider.radius;

        if (leftHinge && rightHinge)
        {
            leftCasterHinge = leftHinge.GetComponent<HingeJoint>();
            rightCasterHinge = rightHinge.GetComponent<HingeJoint>();

            leftCasterWheel.rb.solverIterations = casterSolverIterations;
            rightCasterWheel.rb.solverIterations = casterSolverIterations;
            leftHinge.GetComponent<Rigidbody>().solverIterations = casterSolverIterations;
            rightHinge.GetComponent<Rigidbody>().solverIterations = casterSolverIterations;

            hinges = new HingeJoint[2] { leftCasterHinge, rightCasterHinge };
        }
    }

    void FixedUpdate()
    {
        Vector2 wheelInput;
        bool[] gripping = new bool[2] { false, false };

        HandleWheelInput(out wheelInput, ref gripping);
        ApplyWheelForces(ref wheelInput, ref gripping);
        if (leftHinge && rightHinge)
        {
            UpdateCastersSpring();
        }
        else
        {
            UpdateCastersVisual();
        }

    }

    private void HandleWheelInput(out Vector2 wheelInput, ref bool[] gripping)
    {
        wheelInput = Vector2.zero;
        if (keyboardControl)
        {
            wheelInput = 50.0f * new Vector2(Input.GetAxisRaw("Left wheel"), Input.GetAxisRaw("Right wheel"));
            gripping[0] = wheelInput.x != 0.0f;
            gripping[1] = wheelInput.y != 0.0f;
        }
        else
        {
            inputData._leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftPos);
            inputData._leftController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 leftVel);
            inputData._leftController.TryGetFeatureValue(CommonUsages.gripButton, out bool leftGrip);
            inputData._rightController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightPos);
            inputData._rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 rightVel);
            inputData._rightController.TryGetFeatureValue(CommonUsages.gripButton, out bool rightGrip);
            bool[] grip = new bool[2] { leftGrip, rightGrip };
            Vector3[] handPositions = new Vector3[2] { leftPos, rightPos };
            Vector3[] handVels = new Vector3[2] { leftVel, rightVel };

            if (grip[0] || grip[1])
            {
                Debug.Log("Pos " + handPositions[0].ToString() + " " + handPositions[1].ToString());
                Debug.Log("Vel: " + handVels[0].ToString() + " " + handVels[1].ToString());
            }

            for (int i = 0; i < 2; i++)
            {
                // Check whether hands are close enough to wheels, both should be in the same local space as they are parented by wheelchair_root
                Vector3 handPos = player.transform.localPosition + handPositions[i];
                Vector3 wheelPos = wheels[i].collider.transform.localPosition;

                // Find distance from center of weel in the radial direction
                float wheelRadialDist = Vector2.Distance(new Vector2(handPos.y, handPos.z), new Vector2(wheelPos.y, wheelPos.z));
                // Find lateral distance
                float wheelLateralDist = Mathf.Abs(handPos.x - wheelPos.x);
                // Find tangent direction of wheel at hand position
                Vector3 wheelTangentDir = Vector3.Cross(Vector3.Normalize(handPos - wheelPos), -Vector3.right);
                // Find applied velocity from the controller in tangent direction
                float wheelTangentVel = Vector3.Dot(handVels[i], wheelTangentDir);

                // Find current rb angular velocity and angular velocity apply from input tangent velocity
                float angularvel = wheelTangentVel / (2 * Mathf.PI * wheelRadialDist) * 360;

                if (Mathf.Abs(wheelRadialDist - wheelRadius) < wheelGripRadius && wheelLateralDist < wheelGripWidth)
                {
                    //haptics.Execute(0, Time.fixedDeltaTime, hapticFrequency, hapticStrength, inputHands[i]);

                    if (grip[i])
                    {
                        gripping[i] = true;
                        if (Mathf.Abs(angularvel) > breakingThreshold)
                            wheelInput[i] = angularvel;
                        else
                        {
                            wheelInput[i] = 0.0f;
                        }
                    }
                }
            }
        }
    }

    private void ApplyWheelForces(ref Vector2 wheelInput, ref bool[] gripping)
    {
        Vector2 angularVel = Mathf.Rad2Deg * new Vector2((wheels[0].wheel.transform.worldToLocalMatrix * wheels[0].rb.angularVelocity).x, (wheels[1].wheel.transform.worldToLocalMatrix * wheels[1].rb.angularVelocity).x);
        //Debug.Log("Velocity: " + angularVel);
        for (int i = 0; i < 2; i++)
        {
            if (gripping[i])
            {
                wheels[i].joint.useMotor = true;
                JointMotor motor = wheels[i].joint.motor;

                motor.targetVelocity = (wheelInput[i] == 0.0f) ? 0.0f : angularVel[i] + wheelInput[i];
                wheels[i].joint.motor = motor;
            }
            else
            {
                wheels[i].joint.useMotor = false;
            }
        }

    }

    private void UpdateCastersSpring()
    {
        Vector2 angularVel = Mathf.Rad2Deg * new Vector2((wheels[0].wheel.transform.worldToLocalMatrix * wheels[0].rb.angularVelocity).x, (wheels[1].wheel.transform.worldToLocalMatrix * wheels[1].rb.angularVelocity).x);

        // Set caster wheel angle from estimated velocity in local zx-plane from wheel input
        Vector2 estVelocity = (new Vector2((angularVel.x + angularVel.y) / 2, (angularVel.x - angularVel.y) / 2) / 360) * 2 * Mathf.PI * wheelRadius;
        float estSpeed = estVelocity.magnitude;
        float casterWheelAngle = Mathf.Rad2Deg * Mathf.Atan2(estVelocity.y, estVelocity.x);

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

    private void UpdateCastersVisual()
    {
        Vector2 angularVel = Mathf.Rad2Deg * new Vector2((wheels[0].wheel.transform.worldToLocalMatrix * wheels[0].rb.angularVelocity).x, (wheels[1].wheel.transform.worldToLocalMatrix * wheels[1].rb.angularVelocity).x);

        // Set caster wheel angle from estimated velocity in local zx-plane from wheel input
        Vector2 estVelocity = (new Vector2((angularVel.x + angularVel.y) / 2, (angularVel.x - angularVel.y) / 2) / 360) * 2 * Mathf.PI * wheelRadius;
        float estSpeed = estVelocity.magnitude;
        float casterWheelAngle = Mathf.Rad2Deg * Mathf.Atan2(estVelocity.y, estVelocity.x);

        visualLeftCaster.localRotation = Quaternion.Euler(Vector3.Slerp(Vector3.up * visualLeftCaster.localRotation.y, new Vector3(0.0f, casterWheelAngle, 0.0f), 0.5f));
        visualRightCaster.localRotation = Quaternion.Euler(Vector3.Slerp(Vector3.up * visualRightCaster.localRotation.y, new Vector3(0.0f, casterWheelAngle, 0.0f), 0.5f));
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
