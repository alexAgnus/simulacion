using System.Collections;
using System.Collections.Generic;
using GleyTrafficSystem;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public TrafficComponent trafficComponent;
    // Start is called before the first frame update
    void Awake()
    {
        trafficComponent.nrOfVehicles = PlayerPrefs.GetInt("nrOfVehicles", 20);
        trafficComponent.minDistanceToAdd = PlayerPrefs.GetInt("minDistanceToAdd", 100);
    }
}
