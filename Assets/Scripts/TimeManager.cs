using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    public Timer timer;

    void Start()
    {
        timer.ResetTimer();
        timer.StartTimer();
    }

    void Update()
    {

    }
}
