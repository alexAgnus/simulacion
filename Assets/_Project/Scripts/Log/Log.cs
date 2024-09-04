using System.Collections;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;

public class Log : MonoBehaviour
{
    public Rigidbody rb;
    public float dangerRadius;
    public float warningRadius;
    [Range(0.1f, 2.5f)] public float logWait;
    public LayerMask trafficLayer;

    [NonSerialized] public static string logCreationDate;
    [NonSerialized] public static string fullLogOutputPath;
    private StreamWriter _logger;
    // private Coroutine _coroutine;
    // private float _startTime;
    // private ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
    private HashSet<string> _loggedLines = new HashSet<string>();

    public GameObject userRecordObj;
    private UserRecord _userRecord;

    private string[] _headers = new string[] {
        "currentDateTime",
        "time",
        "userId",
        "risk",
        "distance",
        "vehicleId",
        "vehicleName",
        "vehiclePositionX",
        "vehiclePositionY",
        "vehiclePositionZ",
        "vehicleSpeed",
        "bikePositionX",
        "bikePositionY",
        "bikePositionZ",
        "bikeSpeed"
    };

    private void Awake() { }

    void Start()
    {
        _userRecord = userRecordObj.GetComponent<UserRecord>();
        Debug.Log($" [✅]Start User record # {_userRecord.userId}");
        logCreationDate = DateTime.Now.ToString("yyyy-MM-dd");
        _SaveCSVFile();

        rb = GetComponent<Rigidbody>();
        // _startTime = Time.realtimeSinceStartup;
        // _coroutine = StartCoroutine(CheckCollisions());
        // Task.Run(() => _ProcessLogQueue());
    }

    private void _SaveCSVFile()
    {
        string userId = _userRecord.userId.ToString();
        string path = "";
        Debug.Log($" [✅] Log file initialization started at #{userId} {logCreationDate}");

        string separator = "\\";
#if !UNITY_EDITOR && UNITY_ANDROID
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject filesDir = currentActivity.Call<AndroidJavaObject>("getExternalFilesDir", "Documents");
            string androidPath = filesDir.Call<string>("getAbsolutePath");

            separator = "/";
            
            path = Path.Combine(androidPath, $"Bike AR{separator}Logs{separator}Log_{logCreationDate}");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
#else
        path = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments
            ),
            $"My Games{separator}Bike AR{separator}Logs{separator}Log_{logCreationDate}"
        );
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
#endif
        fullLogOutputPath = $"{path}{separator}n{userId}_{logCreationDate}.csv";
        _logger = new StreamWriter(fullLogOutputPath, true, Encoding.UTF8);
        // string header =
        //     "Tiempo_Ejecucion(HH:mm:ss:fff),Riesgo_De_Accidente,Distancia,Nombre_Vehiculo,Posicion_Vehiculo,Velocidad_Vehiculo,Posicion_Bicicleta,Velocidad_Bicicleta";
        // _logger.WriteLine(header);
        // _logger.Flush();

        _LogLine(
            string.Join(",", _headers)
        );

        Debug.Log($" [✅] Log file created at {fullLogOutputPath}");
    }

    private float _GetFormattedVelocityInKmHFromRB(Rigidbody rbVehicle)
    {
        float speed = rbVehicle.velocity.magnitude;
        return 3.6f * speed;
    }

    // private IEnumerator CheckCollisions()
    // {
    //     while (true)
    //     {
    //         string lineRecord;
    //         float distance;
    //         string vehicleName;
    //         Rigidbody VehicleRB;
    //         string vehiclePosition;
    //         float vehicleSpeed;
    //         string bikePosition;
    //         float bikeSpeed;
    //         string accidentRisk;

    //         Collider[] warningTrafficObjects = Physics.OverlapSphere(transform.position, warningRadius, trafficLayer);

    //         if (warningTrafficObjects.Length == 0)
    //         {
    //             bikePosition = $"{rb.position.x:F8}/{rb.position.y:F8}/{rb.position.z:F8}";
    //             bikeSpeed = _GetFormattedVelocityInKmHFromRB(rb);
    //             TimeSpan elapsedTime = TimeSpan.FromSeconds(Time.realtimeSinceStartup);
    //             lineRecord = $@"{elapsedTime.ToString(@"hh\:mm\:ss\.fff")},,,,,,{bikePosition},{bikeSpeed:F8}";
    //             _EnqueueLogLine(lineRecord);
    //         }
    //         else
    //         {
    //             foreach (Collider trafficObject in warningTrafficObjects)
    //             {
    //                 if (trafficObject.name != "BodyCollider") continue;

    //                 if (!Physics.Raycast(transform.position, trafficObject.transform.position - transform.position,
    //                         out var hit, warningRadius, trafficLayer)) continue;

    //                 distance = hit.distance;
    //                 vehicleName = trafficObject.transform.parent.parent.parent.gameObject.name;
    //                 VehicleRB = trafficObject.attachedRigidbody;
    //                 vehiclePosition = $"{VehicleRB.position.x:F8}/{VehicleRB.position.y:F8}/{VehicleRB.position.z:F8}";
    //                 vehicleSpeed = _GetFormattedVelocityInKmHFromRB(VehicleRB);
    //                 bikePosition = $"{rb.position.x:F8}/{rb.position.y:F8}/{rb.position.z:F8}";
    //                 bikeSpeed = _GetFormattedVelocityInKmHFromRB(rb);
    //                 accidentRisk = dangerRadius < distance && distance <= warningRadius ? "Bajo" : "Alto";
    //                 TimeSpan elapsedTime = TimeSpan.FromSeconds(Time.realtimeSinceStartup);
    //                 lineRecord =
    //                     $@"{elapsedTime.ToString(@"hh\:mm\:ss\.fff")},{accidentRisk},{distance:F8},{vehicleName},{vehiclePosition},{vehicleSpeed:F8},{bikePosition},{bikeSpeed:F8}";

    //                 _EnqueueLogLine(lineRecord);
    //             }
    //         }

    //         yield return new WaitForSeconds(logWait);
    //     }
    // }

    void FixedUpdate() {
        Collider[] warningTrafficObjects = Physics.OverlapSphere(
            transform.position,
            warningRadius,
            trafficLayer
        );

        if(warningTrafficObjects.Length == 0) {
            return;
        }

        foreach(Collider trafficObject in warningTrafficObjects) {
            if(trafficObject.name != "BodyCollider") {
                continue;
            }

            // TODO: To remove. Commented out because it's not working as expected, the measurements are not accurate
            // if(!Physics.Raycast(
            //     transform.position,
            //     trafficObject.transform.position - transform.position,
            //     out var hit,
            //     warningRadius,
            //     trafficLayer
            // )) {
            //     continue;
            // }
            // Rigidbody VehicleRB = trafficObject.attachedRigidbody;
            // float distance = hit.distance;
            // string vehicleName = trafficObject.transform.parent.parent.parent.gameObject.name;
            // string vehiclePositionX = VehicleRB.position.x.ToString("F8");
            // string vehiclePositionY = VehicleRB.position.y.ToString("F8");
            // string vehiclePositionZ = VehicleRB.position.z.ToString("F8");
            // float vehicleSpeed = _GetFormattedVelocityInKmHFromRB(VehicleRB);
            // string bikePositionX = rb.position.x.ToString("F8");
            // string bikePositionY = rb.position.y.ToString("F8");
            // string bikePositionZ = rb.position.z.ToString("F8");
            // float bikeSpeed = _GetFormattedVelocityInKmHFromRB(rb);
            // string accidentRisk = dangerRadius < distance && distance <= warningRadius ? "warning" : "risk";

            float APPROXIMATE_OBJECT_WIDTH = 1.5f;
            float distance = Vector3.Distance(rb.position, trafficObject.attachedRigidbody.position) - APPROXIMATE_OBJECT_WIDTH;

            string lineRecord = _FormatLogLine(
                accidentRisk: dangerRadius < distance && distance <= warningRadius ? "warning" : "danger",
                distance: distance,
                vehicleId: trafficObject.attachedRigidbody.GetInstanceID().ToString(),
                vehicleName: trafficObject.transform.parent.parent.parent.gameObject.name,
                vehiclePositionX: trafficObject.attachedRigidbody.position.x,
                vehiclePositionY: trafficObject.attachedRigidbody.position.y,
                vehiclePositionZ: trafficObject.attachedRigidbody.position.z,
                vehicleSpeed: _GetFormattedVelocityInKmHFromRB(trafficObject.attachedRigidbody),
                bikePositionX: rb.position.x,
                bikePositionY: rb.position.y,
                bikePositionZ: rb.position.z,
                bikeSpeed: _GetFormattedVelocityInKmHFromRB(rb)
            );

            _LogLine(lineRecord);
        }
    }

    private string _FormatLogLine(
        string accidentRisk,
        float distance,
        string vehicleId,
        string vehicleName,
        float vehiclePositionX,
        float vehiclePositionY,
        float vehiclePositionZ,
        float vehicleSpeed,
        float bikePositionX,
        float bikePositionY,
        float bikePositionZ,
        float bikeSpeed
    )
    {
        string currentDateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string elapsedTime = TimeSpan.FromSeconds(Time.realtimeSinceStartup).ToString(@"hh\:mm\:ss\.fff");

        string line = string.Join(",", new string[] {
            currentDateTime,
            elapsedTime,
            _userRecord.userId.ToString(),
            accidentRisk,
            distance.ToString(CultureInfo.InvariantCulture),
            vehicleId,
            vehicleName,
            vehiclePositionX.ToString(CultureInfo.InvariantCulture),
            vehiclePositionY.ToString(CultureInfo.InvariantCulture),
            vehiclePositionZ.ToString(CultureInfo.InvariantCulture),
            vehicleSpeed.ToString(CultureInfo.InvariantCulture),
            bikePositionX.ToString(CultureInfo.InvariantCulture),
            bikePositionY.ToString(CultureInfo.InvariantCulture),
            bikePositionZ.ToString(CultureInfo.InvariantCulture),
            bikeSpeed.ToString(CultureInfo.InvariantCulture),
        });

        // Debug.Log($" [✅] {line}");

        return line;
    }

    private void _LogLine(string line)
    {
        _logger.WriteLine(line);
        _logger.Flush();
        // if (_loggedLines.Contains(line)) return;
        // _logQueue.Enqueue(line);
        // _loggedLines.Add(line);
    }

    // private void _ProcessLogQueue()
    // {
    //     while (true)
    //     {
    //         if (_logQueue.TryDequeue(out string logLine))
    //         {
    //             _logger.WriteLine(logLine);
    //             _logger.Flush();
    //         }
    //     }
    // }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.name != "BodyCollider")
        {
            return;
        }

        string lineRecord = _FormatLogLine(
            accidentRisk: "collision",
            distance: 0,
            vehicleId: other.collider.attachedRigidbody.GetInstanceID().ToString(),
            vehicleName: other.gameObject.name,
            vehiclePositionX: other.collider.attachedRigidbody.position.x,
            vehiclePositionY: other.collider.attachedRigidbody.position.y,
            vehiclePositionZ: other.collider.attachedRigidbody.position.z,
            vehicleSpeed: _GetFormattedVelocityInKmHFromRB(other.collider.attachedRigidbody),
            bikePositionX: rb.position.x,
            bikePositionY: rb.position.y,
            bikePositionZ: rb.position.z,
            bikeSpeed: _GetFormattedVelocityInKmHFromRB(rb)
        );

        _LogLine(lineRecord);
    }

    private void OnDestroy()
    {
        _logger?.Close();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // if (!pauseStatus && _coroutine != null)
        // {
        //     StopCoroutine(_coroutine);
        //     _logger?.Close();
        // }
        // else
        // {
        //     _logger ??= new StreamWriter(fullLogOutputPath, true);
        //     _coroutine = StartCoroutine(CheckCollisions());
        // }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(rb.position, warningRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rb.position, dangerRadius);
    }
}
