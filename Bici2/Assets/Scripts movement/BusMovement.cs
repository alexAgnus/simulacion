using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BusMovement : MonoBehaviour
{
    private float horizontalIput;
    private float verticalIput;
    private float steeringAngle;

    public WheelCollider wheel1, wheel3;
    public WheelCollider wheel2, wheel4;
    public Transform wheel1t, wheel3t;
    public Transform wheel2t, wheel4t;
    public float MaxsteerAngle = 45;
    public float motorForce = 50;

    public void GetInput()
    {
        horizontalIput = Input.GetAxis("Horizontal");
        verticalIput = Input.GetAxis("Vertical");
    }

    public void Steer()
    {
        steeringAngle = MaxsteerAngle * horizontalIput;
        wheel1.steerAngle = steeringAngle;
        wheel3.steerAngle = steeringAngle;
    }
    
    public void Acelerate()
    {
        wheel1.motorTorque = verticalIput * motorForce;
        wheel3.motorTorque = steeringAngle * motorForce;
    }

    public void UpdateWheelPoses()
    {
        UpdateWheelPose(wheel1, wheel1t);
        UpdateWheelPose(wheel3, wheel3t);
        UpdateWheelPose(wheel2, wheel2t);
        UpdateWheelPose(wheel4, wheel4t);
    }

    public void UpdateWheelPose(WheelCollider _collider, Transform _transform)
    {
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;
        _collider.GetWorldPose(out _pos, out _quat);
        _transform.position = _pos;
        _transform.rotation = _quat;
    }

    public void FixedUpdate()
    {
        GetInput();
        Steer();
        Acelerate();
        UpdateWheelPoses();
    }
}
