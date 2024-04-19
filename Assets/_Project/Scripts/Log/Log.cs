using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.IO;
using System.Globalization;
using System.Text;
using System.Diagnostics;

public class Log : MonoBehaviour
{
    public Rigidbody rb;
    public Vector3 collisionBoxSize;
    public Vector3 collisionBoxRotation;
    public float dangerRadius;
    public float warningRadius;
    public float verticalRaySize;
    public float horizontalRaySize;
    public LayerMask trafficLayer;
    TextWriter file_Record;
    string nameFile;
    Stopwatch stopwatch = new Stopwatch();
    void Start()
    {
        stopwatch.Start();
        rb = GetComponent<Rigidbody>();
        string Dir = Directory.GetCurrentDirectory();
        string currentDirectory= Dir + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar + "_Project" + Path.DirectorySeparatorChar + "Logs" + Path.DirectorySeparatorChar;
        DateTime currentDate = DateTime.Now;
        int day = currentDate.Day;
        int month = currentDate.Month;
        int year = currentDate.Year;
        int hour = currentDate.Hour;
        int minute = currentDate.Minute;
        int seconds = currentDate.Second;
        string nameRecord= "Record_" + day + "_" + month + "_" +  year + "_" +  hour + "-" + minute + "-" + seconds + ".csv";

        nameFile= currentDirectory + nameRecord;
        file_Record= new StreamWriter(nameFile, true);
        string header = "Tiempo_Ejecucion(H:M:S),Riesgo_De_Accidente,Nombre_Vehiculo,Distancia,Posicion,Del,Vehiculo,Velocidad_Vehiculo,Posicion,De_La,Bicicleta,Velocidad_Bicicleta";
        string header2 =",,,,X,Y,Z,,X,Y,Z,";
        file_Record.WriteLine(header); 
        file_Record.WriteLine(header2); 
    }

    void Update()
    {
        CheckCollisions();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(rb.position, warningRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rb.position, dangerRadius);

        Gizmos.color = Color.white;
        Gizmos.matrix = Matrix4x4.TRS(rb.position, Quaternion.Euler(collisionBoxRotation), collisionBoxSize);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
    private void CheckCollisions()
    {
        Collider[] trafficObjects = Physics.OverlapSphere(transform.position, warningRadius, trafficLayer);

        foreach (Collider trafficObject in trafficObjects)
        {
            Vector3 directionToObject = trafficObject.transform.position - transform.position;
            RaycastHit hit;
            Vector3 bikePosition = rb.position;

            if (Physics.BoxCast(transform.position, transform.lossyScale / 2, directionToObject, out hit, Quaternion.identity, directionToObject.magnitude, trafficLayer))
            {
                if (hit.collider == trafficObject && trafficObject.CompareTag("Traffic"))
                {
                    float distance = hit.distance;
                    Vector3 vehiclePosition = hit.point;
                    string sedanName = trafficObject.transform.parent.parent.parent.gameObject.name;

                    if (distance <= dangerRadius && distance > Mathf.Max(collisionBoxSize.x, collisionBoxSize.y, collisionBoxSize.z) / 3f)
                    {
                        string records = $"{stopwatch.Elapsed},Alto,{sedanName},{distance:F4},{vehiclePosition.x:F2},{vehiclePosition.y:F2},{vehiclePosition.z:F2},{trafficObject.attachedRigidbody.velocity.magnitude:F2},{bikePosition.x:F2},{bikePosition.y:F2},{bikePosition.z:F2},{GetComponent<Rigidbody>().velocity.magnitude:F2}";
                        file_Record.WriteLine(records);
                    }
                    else if (distance <= warningRadius && distance > dangerRadius)
                    {
                        string records = $"{stopwatch.Elapsed},Bajo,{sedanName},{distance:F4},{vehiclePosition.x:F2},{vehiclePosition.y:F2},{vehiclePosition.z:F2},{trafficObject.attachedRigidbody.velocity.magnitude:F2},{bikePosition.x:F2},{bikePosition.y:F2},{bikePosition.z:F2},{GetComponent<Rigidbody>().velocity.magnitude:F2}";
                        file_Record.WriteLine(records);
                    }
                    else if (distance <= Mathf.Max(collisionBoxSize.x, collisionBoxSize.y, collisionBoxSize.z) / 3f)
                    {
                        string records = $"{stopwatch.Elapsed},Colision,{sedanName},{distance:F4},{vehiclePosition.x:F2},{vehiclePosition.y:F2},{vehiclePosition.z:F2},{trafficObject.attachedRigidbody.velocity.magnitude:F2},{bikePosition.x:F2},{bikePosition.y:F2},{bikePosition.z:F2},{GetComponent<Rigidbody>().velocity.magnitude:F2}";
                        file_Record.WriteLine(records);
                    }
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        file_Record.Close();
        stopwatch.Stop();
        stopwatch.Reset();
    }

}