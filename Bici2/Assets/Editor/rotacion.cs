using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class rotacion : MonoBehaviour
{
    public float minimoY;
    public float maximoY;
    float horizontalInput;

   
    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up ,horizontalInput);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y,minimoY, maximoY ), transform.position.z);
    }
    
}
