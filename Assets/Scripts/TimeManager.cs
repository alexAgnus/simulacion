using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    public Timer timer;
    // Start is called before the first frame update
    void Start()
    {
        timer.ResetTimer(10);
        timer.StartTimer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
