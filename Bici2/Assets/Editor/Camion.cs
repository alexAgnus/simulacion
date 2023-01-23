using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camion : MonoBehaviour
{
    public float velocidad;

    private void Update()
    {
        transform.position += new Vector3(0, 0, velocidad);
        if(transform.position.z >= 1f)
        {
            velocidad *= 1;
        }
        if (transform.position.z <= -1f)
        {
            velocidad *= -1;
        }

    }
}
