using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelManager : MonoBehaviour
{
    public GameObject VelocimeterObj;
    public Rigidbody bikeRB;
    
    private Velocimeter velocimeter;

    private float speed = 0; 
    private float acceleration = 0.2f;
    void Start()
    {
        velocimeter = VelocimeterObj.GetComponent<Velocimeter>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocimeter.SetVelocityTo(
            bikeRB.velocity.magnitude * 3.6f
        );
    }
}
