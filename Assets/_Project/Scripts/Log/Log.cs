using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : MonoBehaviour
{
    public Rigidbody rb;
    public float dangerRadius;
    public float warningRadius;
    public float verticalRaySize;
    public float horizontalRaySize;
    public LayerMask layerCollision;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckCollisions();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dangerRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, warningRadius);
    }

    private void CheckCollisions()
    {
        Collider[] trafficColliders = Physics.OverlapSphere(transform.position, warningRadius, layerCollision.value);

        foreach (Collider collider in trafficColliders)
        {
            float distance = Vector3.Distance(transform.position, collider.bounds.max);
            
            if (distance <= dangerRadius)
                Debug.Log(
                    $"Riesgo de accidente alto: Auto: {collider.transform.parent.parent.parent.name}, Distancia {distance}, Posición del auto: {collider.transform.position} Velocidad del auto: {collider.transform.parent.parent.parent.GetComponent<Rigidbody>().velocity.magnitude}, Posición de Bicicleta: {transform.position}, Velocidad de Bicicleta {rb.velocity.magnitude}");
            else if (distance <= warningRadius && distance > dangerRadius)
                Debug.Log(
                    $"Riesgo de accidente bajo: Auto: {collider.transform.parent.parent.parent.name}, Distancia {distance}, Posición del auto: {collider.transform.position} Velocidad del auto: {collider.transform.parent.parent.parent.GetComponent<Rigidbody>().velocity.magnitude}, Posición de Bicicleta: {transform.position}, Velocidad de Bicicleta {rb.velocity.magnitude}");
            
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Traffic"))
            Debug.Log(
                $"Accidente: Auto: {other.gameObject.name}, Posición del auto: {other.gameObject.transform.position} Velocidad del auto: {other.gameObject.GetComponent<Rigidbody>().velocity.magnitude}, Posición de Bicicleta: {transform.position}, Velocidad de Bicicleta {rb.velocity.magnitude}");
    }
}