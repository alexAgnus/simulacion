using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Consulted MS Motorcycle test - Marcos Schultz (www.schultzgames.com)
[RequireComponent(typeof(Rigidbody))]
public class Bicycle : MonoBehaviour
{
    [SerializeField]
    private WheelCollider frontWheel;
    [SerializeField]
    private WheelCollider rearWheel;
    Rigidbody ms_Rigidbody;
 
    float rbVelocityMagnitude;
    float horizontalInput;
    float verticalInput;
    float medRPM;

    private void Awake()
    {
        transform.rotation = Quaternion.identity;
        ms_Rigidbody = GetComponent<Rigidbody> ();
        ms_Rigidbody.mass = 400;
        ms_Rigidbody.interpolation = RigidbodyInterpolation.Extrapolate;
        ms_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        //centerOfMass
        var centerOfmassOBJ = new GameObject ("centerOfmass");
        centerOfmassOBJ.transform.parent = transform;
        centerOfmassOBJ.transform.localPosition = new Vector3 (0.0f, -0.3f, 0.0f);
        ms_Rigidbody.centerOfMass = transform.InverseTransformPoint(centerOfmassOBJ.transform.position);
    }
    
    void OnEnable(){
        WheelCollider WheelColliders = GetComponentInChildren<WheelCollider>();
        WheelColliders.ConfigureVehicleSubsteps(1000, 30, 30);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        horizontalInput = Input.GetAxis ("Horizontal");
        verticalInput = Input.GetAxis ("Vertical");
        medRPM = (frontWheel.rpm + rearWheel.rpm) / 2;
        rbVelocityMagnitude = ms_Rigidbody.velocity.magnitude;
 
        //motorTorque
        if (medRPM > 0) {
            rearWheel.motorTorque = verticalInput * ms_Rigidbody.mass * 4.0f;
        } else {
            rearWheel.motorTorque = verticalInput * ms_Rigidbody.mass * 1.5f;
        }
 
        //steerAngle
        float nextAngle = horizontalInput * 35.0f;
        frontWheel.steerAngle = Mathf.Lerp (frontWheel.steerAngle, nextAngle, 0.125f);
 
 
        if(Mathf.Abs(rearWheel.rpm) > 10000){
            rearWheel.motorTorque = 0.0f;
            rearWheel.brakeTorque = ms_Rigidbody.mass * 5;
        }
        //
        if (rbVelocityMagnitude < 1.0f && Mathf.Abs (verticalInput) < 0.1f) {
            rearWheel.brakeTorque = frontWheel.brakeTorque = ms_Rigidbody.mass * 2.0f;
        } else {
            rearWheel.brakeTorque = frontWheel.brakeTorque = 0.0f;
        }
        //
        Stabilizer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void Stabilizer(){
        Vector3 axisFromRotate = Vector3.Cross (transform.up, Vector3.up);
        Vector3 torqueForce = axisFromRotate.normalized * axisFromRotate.magnitude * 50;
        torqueForce.x = torqueForce.x * 0.4f;
        torqueForce -= ms_Rigidbody.angularVelocity;
        ms_Rigidbody.AddTorque (torqueForce * ms_Rigidbody.mass * 0.02f, ForceMode.Impulse);
 
        float rpmSign = Mathf.Sign (medRPM) * 0.02f;
        if (rbVelocityMagnitude > 1.0f && frontWheel.isGrounded && rearWheel.isGrounded) {
            ms_Rigidbody.angularVelocity += new Vector3 (0, horizontalInput * rpmSign, 0);
        }
    }
}
