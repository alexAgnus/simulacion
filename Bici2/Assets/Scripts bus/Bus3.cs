using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bus3 : MonoBehaviour
{
    public WheelCollider wheel1;
    public WheelCollider wheel2;
    public WheelCollider wheel3;
    public WheelCollider wheel4;

    public Transform wheel1_trans;
    public Transform wheel2_trans;
    public Transform wheel3_trans;
    public Transform wheel4_trans;

    public float frenar;
    public float Force;
    public float Speed;
    public float ActualSpeed;
    public float AngleDirection;
    public float Turn;
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ActualSpeed = 2 * Mathf.PI * wheel1.rpm * 60 / 1000;
        {
            wheel1.motorTorque = Force * Input.GetAxis("Vertical");
            wheel2.motorTorque = Force * Input.GetAxis("Vertical");
            wheel3.steerAngle = -30 * Input.GetAxis("Horizontal");
            wheel4.steerAngle = -40 * Input.GetAxis("Horizontal");

            if (Input.GetAxis("Vertical") == 0)
            {
                wheel1.brakeTorque = frenar;
                wheel2.brakeTorque = frenar;
            }
            else
            {
                wheel1.brakeTorque = 0;
                wheel2.brakeTorque = 0;
            }
        }
        Speed = GetComponent<Rigidbody>().velocity.magnitude * 15;

        Turn = AngleDirection * Input.GetAxis("Horizontal");
        wheel1.steerAngle = Turn;
        wheel2.steerAngle = Turn;
    }
    // Update is called once per frame
    void Update()
    {
        wheel1_trans.Rotate(0, 0, wheel1.rpm / 60 * 360 * Time.deltaTime);
        wheel2_trans.Rotate(0, 0, wheel2.rpm / 60 * 360 * Time.deltaTime);

        Vector3 WheelDirection = wheel1_trans.localEulerAngles;
        WheelDirection.y = Turn + 90;
        wheel1_trans.localEulerAngles = WheelDirection;
        wheel2_trans.localEulerAngles = WheelDirection;
    }
}
