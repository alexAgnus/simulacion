using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelManager : MonoBehaviour
{
    public GameObject VelocimeterObj;
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

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            speed += acceleration;
        }else{
            speed -= acceleration / 2; 

        }
        Debug.Log(speed);

        if(speed <= 0){
            speed = 0; 
        }
        if(speed >= velocimeter.MaxSpeed){
            speed = velocimeter.MaxSpeed;
        }
        velocimeter.SetVelocityTo(speed);
    }
}
