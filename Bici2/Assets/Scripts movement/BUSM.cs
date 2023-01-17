using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUSM : MonoBehaviour
{
    public int frenar = 500;

    public WheelCollider wheel1;
    public WheelCollider wheel2;
    public WheelCollider wheel3;
    public WheelCollider wheel4;
    public Transform Wheel1;
    public Transform Wheel3;
    public int Velocity = 50;
    // Update is called once per frame
    void Update()
    {
       Wheel1.localEulerAngles = new Vector3(0, wheel2.steerAngle, 0);
        Wheel3.localEulerAngles = new Vector3(0, wheel4.steerAngle, 0);
    }
    private void FixedUpdate()
    {
        wheel1.motorTorque = Velocity * Input.GetAxis("Vertical");
        wheel3.motorTorque = Velocity * Input.GetAxis("Vertical");
        wheel2.steerAngle = -40 * Input.GetAxis("Horizontal");
        wheel4.steerAngle = -40 * Input.GetAxis("Horizontal");

        if (Input.GetAxis("Vertical") == 0)
        {
            wheel1.brakeTorque = frenar;
            wheel3.brakeTorque = frenar;
        }
        else
        {
            wheel1.brakeTorque = 0;
            wheel3.brakeTorque = 0;
        }
    }
}
