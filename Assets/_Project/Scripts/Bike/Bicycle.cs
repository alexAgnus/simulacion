using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
public class Bicycle : MonoBehaviour
{
    [SerializeField]
    private WheelCollider frontWheel;
    [SerializeField]
    private WheelCollider rearWheel;
    private Rigidbody ms_Rigidbody;

    [SerializeField]
    private float speedLimitInMetersPerSeconds = 8.0f;
    [SerializeField]
    private float torqueAccel;

    private float rbVelocityMagnitude;
    private float horizontalInput;
    private float verticalInput;
    private float medRPM;

    [SerializeField]
    private float maxSteerAngle = 25.0f;

    [SerializeField]
    private float lerpCoefficientToSteerAngle = 10.0f;

    [SerializeField]
    private float brakeForce = 10.0f;

    [SerializeField]
    public AnimationCurve accelerationCurve;

    [SerializeField]
    public float hillAssistTorqueMultiplier = 2.5f;
    [SerializeField]
    public float hillAssistAngleThreshold = 85.0f;

    private void Awake()
    {
        transform.rotation = Quaternion.identity;
        ms_Rigidbody = GetComponent<Rigidbody>();
        ms_Rigidbody.mass = 400;
        ms_Rigidbody.interpolation = RigidbodyInterpolation.Extrapolate;
        ms_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // centerOfMass
        var centerOfmassOBJ = new GameObject("centerOfMass");
        centerOfmassOBJ.transform.parent = transform;
        centerOfmassOBJ.transform.localPosition = new Vector3(0.0f, -0.3f, 0.0f);
        ms_Rigidbody.centerOfMass = transform.InverseTransformPoint(centerOfmassOBJ.transform.position);
    }

    void OnEnable()
    {
        WheelCollider wheelColliders = GetComponentInChildren<WheelCollider>();
        wheelColliders.ConfigureVehicleSubsteps(1000, 30, 30);
    }

    void Start()
    {
        // Inicialización opcional
    }

    void Update()
    { }

    private void FixedUpdate()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        medRPM = (frontWheel.rpm + rearWheel.rpm) / 2;
        rbVelocityMagnitude = ms_Rigidbody.velocity.magnitude;

        float accelFactor = accelerationCurve.Evaluate(rbVelocityMagnitude / speedLimitInMetersPerSeconds);
        float torque = verticalInput * ms_Rigidbody.mass * torqueAccel * accelFactor;
        // Debug.Log("❤️ accelFactor" + accelFactor + "❤️ torque" + torque + "❤️ rbVelocityMagnitude" + rbVelocityMagnitude + "❤️ speedLimit" + speedLimit + "❤️ medRPM" + medRPM + "❤️ verticalInput" + verticalInput + "❤️ horizontalInput" + horizontalInput + "❤️");

        float inclineAngle = Vector3.Angle(Vector3.up, transform.forward);
        bool isClimbing = inclineAngle < hillAssistAngleThreshold;

        // Debug.Log("❤️ inclineAngle" + inclineAngle + "❤️ hillAssistAngleThreshold" + hillAssistAngleThreshold + "❤️ isClimbing" + (isClimbing ? "✅" : "✖️"));
        if (isClimbing)
        {
            torque *= hillAssistTorqueMultiplier;
        }

        // motorTorque
        if (medRPM > 0)
        {
            if (ms_Rigidbody.velocity.magnitude * Mathf.Sign(verticalInput) < speedLimitInMetersPerSeconds)
                rearWheel.motorTorque = torque * accelFactor;
        }
        else
        {
            rearWheel.motorTorque = torque * accelFactor / 2.0f;
        }

        // steerAngle
        float nextAngle = horizontalInput * maxSteerAngle;
        float speedFactor = Mathf.Clamp01(rbVelocityMagnitude / 10.0f);
        float steerAngle = horizontalInput * Mathf.Lerp(maxSteerAngle, maxSteerAngle / 2.0f, speedFactor);
        frontWheel.steerAngle = Mathf.Lerp(
            frontWheel.steerAngle,
            steerAngle,
            Time.deltaTime * lerpCoefficientToSteerAngle
        );

        if (Mathf.Abs(rearWheel.rpm) > 10000)
        {
            rearWheel.motorTorque = 0.0f;
            rearWheel.brakeTorque = ms_Rigidbody.mass * 5;
        }

        if (verticalInput < 0 && rbVelocityMagnitude > 2.0f)
        {
            float _brakeForce = ms_Rigidbody.mass * brakeForce;
            rearWheel.brakeTorque = frontWheel.brakeTorque = Mathf.Abs(verticalInput) * _brakeForce;
        }
        else if (rbVelocityMagnitude < 1.0f && Mathf.Abs(verticalInput) < 0.1f)
        {
            rearWheel.brakeTorque = frontWheel.brakeTorque = ms_Rigidbody.mass * 2.0f;
        }
        else
        {
            rearWheel.brakeTorque = frontWheel.brakeTorque = 0.0f;
        }

        Stabilizer();
    }

    private void Stabilizer()
    {
        Vector3 axisFromRotate = Vector3.Cross(transform.up, Vector3.up);
        Vector3 torqueForce = axisFromRotate.normalized * axisFromRotate.magnitude * 50;
        torqueForce.x = torqueForce.x * 0.4f;
        torqueForce -= ms_Rigidbody.angularVelocity;
        ms_Rigidbody.AddTorque(torqueForce * ms_Rigidbody.mass * 0.03f, ForceMode.Impulse);

        float rpmSign = Mathf.Sign(medRPM) * 0.03f;
        if (rbVelocityMagnitude > 1.0f && frontWheel.isGrounded && rearWheel.isGrounded)
        {
            ms_Rigidbody.angularVelocity += new Vector3(0, horizontalInput * rpmSign, 0);
        }
    }
}

