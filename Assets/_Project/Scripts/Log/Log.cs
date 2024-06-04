using System.Collections;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class Log : MonoBehaviour
{
    public Rigidbody rb;
    public float dangerRadius;
    public float warningRadius;
    [Range(0.1f, 2.5f)] public float logWait;
    public LayerMask trafficLayer;

    [NonSerialized] public static string logCreationDate;
    [NonSerialized] public static string logOutputPath;
    private StreamWriter _logger;
    private Coroutine _coroutine;
    private float _startTime;
    private ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
    private HashSet<string> _loggedLines = new HashSet<string>();

    private void Awake()
    {
        logCreationDate = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        CreateCSV();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _startTime = Time.realtimeSinceStartup;
        _coroutine = StartCoroutine(CheckCollisions());
        Task.Run(() => ProcessLogQueue());
    }

    private void CreateCSV()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject filesDir = currentActivity.Call<AndroidJavaObject>("getExternalFilesDir", "Documents");
            string androidPath = filesDir.Call<string>("getAbsolutePath");
            
            logOutputPath = Path.Combine(androidPath, "Bike AR\\Logs");
            if (!Directory.Exists(logOutputPath))
            {
                Directory.CreateDirectory(logOutputPath);
            }
        }
#else
        logOutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            $"My Games\\Bike AR\\Logs\\Log_{logCreationDate}");
        if (!Directory.Exists(logOutputPath))
        {
            Directory.CreateDirectory(logOutputPath);
        }
#endif
        _logger = new StreamWriter($"{logOutputPath}\\{logCreationDate}.csv", true, Encoding.UTF8);
        string header =
            "Tiempo_Ejecucion(HH:mm:ss:fff),Riesgo_De_Accidente,Distancia,Nombre_Vehiculo,Posicion_Vehiculo,Velocidad_Vehiculo,Posicion_Bicicleta,Velocidad_Bicicleta";
        _logger.WriteLine(header);
        _logger.Flush();
    }

    private float Velocity(Rigidbody rbVehicle)
    {
        float speed = rbVehicle.velocity.magnitude;
        float direction = Vector3.Dot(rbVehicle.velocity.normalized, transform.forward);

        return 3.6f * (direction >= 0 ? speed : -speed);
    }

    private IEnumerator CheckCollisions()
    {
        while (true)
        {
            string lineRecord;
            float distance;
            string vehicleName;
            Rigidbody VehicleRB;
            string vehiclePosition;
            float vehicleSpeed;
            string bikePosition;
            float bikeSpeed;
            string accidentRisk;

            Collider[] trafficObjects = Physics.OverlapSphere(transform.position, warningRadius, trafficLayer);

            if (trafficObjects.Length == 0)
            {
                bikePosition = $"{rb.position.x:F8}/{rb.position.y:F8}/{rb.position.z:F8}";
                bikeSpeed = Velocity(rb);
                TimeSpan elapsedTime = TimeSpan.FromSeconds(Time.realtimeSinceStartup);
                lineRecord = $@"{elapsedTime.ToString(@"hh\:mm\:ss\.fff")},,,,,,{bikePosition},{bikeSpeed:F8}";
                EnqueueLogLine(lineRecord);
            }
            else
            {
                foreach (Collider trafficObject in trafficObjects)
                {
                    if (trafficObject.name != "BodyCollider") continue;

                    if (!Physics.Raycast(transform.position, trafficObject.transform.position - transform.position,
                            out var hit, warningRadius, trafficLayer)) continue;

                    distance = hit.distance;
                    vehicleName = trafficObject.transform.parent.parent.parent.gameObject.name;
                    VehicleRB = trafficObject.attachedRigidbody;
                    vehiclePosition = $"{VehicleRB.position.x:F8}/{VehicleRB.position.y:F8}/{VehicleRB.position.z:F8}";
                    vehicleSpeed = Velocity(VehicleRB);
                    bikePosition = $"{rb.position.x:F8}/{rb.position.y:F8}/{rb.position.z:F8}";
                    bikeSpeed = Velocity(rb);
                    accidentRisk = dangerRadius < distance && distance <= warningRadius ? "Bajo" : "Alto";
                    TimeSpan elapsedTime = TimeSpan.FromSeconds(Time.realtimeSinceStartup);
                    lineRecord =
                        $@"{elapsedTime.ToString(@"hh\:mm\:ss\.fff")},{accidentRisk},{distance:F8},{vehicleName},{vehiclePosition},{vehicleSpeed:F8},{bikePosition},{bikeSpeed:F8}";

                    EnqueueLogLine(lineRecord);
                }
            }

            yield return new WaitForSeconds(logWait);
        }
    }

    private void EnqueueLogLine(string line)
    {
        if (_loggedLines.Contains(line)) return;
        _logQueue.Enqueue(line);
        _loggedLines.Add(line);
    }

    private void ProcessLogQueue()
    {
        while (true)
        {
            if (_logQueue.TryDequeue(out string logLine))
            {
                _logger.WriteLine(logLine);
                _logger.Flush();
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.name == "BodyCollider")
        {
            string vehicleName = other.gameObject.name;
            Rigidbody VehicleRB = other.collider.attachedRigidbody;
            string vehiclePosition = $"{VehicleRB.position.x:F8}/{VehicleRB.position.y:F8}/{VehicleRB.position.z:F8}";
            float vehicleSpeed = Velocity(VehicleRB);
            string bikePosition = $"{rb.position.x:F8}/{rb.position.y:F8}/{rb.position.z:F8}";
            float bikeSpeed = Velocity(rb);
            TimeSpan elapsedTime = TimeSpan.FromSeconds(Time.realtimeSinceStartup);
            string lineRecord =
                $@"{elapsedTime.ToString(@"hh\:mm\:ss\.fff")},Colisión,0,{vehicleName},{vehiclePosition},{vehicleSpeed:F8},{bikePosition},{bikeSpeed:F8}";
            EnqueueLogLine(lineRecord);
        }
    }

    private void OnDestroy()
    {
        _logger?.Close();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && _coroutine != null)
        {
            StopCoroutine(_coroutine);
            _logger?.Close();
        }
        else
        {
            _logger ??= new StreamWriter($"{logOutputPath}\\{logCreationDate}.csv", true);
            _coroutine = StartCoroutine(CheckCollisions());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(rb.position, warningRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rb.position, dangerRadius);
    }
}
