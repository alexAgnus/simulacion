using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bikemove2 : MonoBehaviour
{
    
    Rigidbody rb;
    Vector3 Inputmove;
    public float speed = 19f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Inputmove = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
        {
            Inputmove.x= 1;
        }
            else if (Input.GetKey(KeyCode.S))
        {
            Inputmove.x= -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Inputmove.z = -1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Inputmove.z = 1;
        }
    }

    protected void FixedUpdate()
    {
        move(Inputmove);
    }

    void move (Vector3 direction)
    {
        rb.AddForce(direction.normalized * speed, ForceMode.Acceleration);
    }
}
