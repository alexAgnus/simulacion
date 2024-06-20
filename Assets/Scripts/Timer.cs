using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public int startTime = 0;  // Tiempo de inicio en segundos
    public bool displayHrs = true;  // Mostrar horas
    public bool displayMins = true; // Mostrar minutos
    public bool timerRunning = false; // Indica si el cronómetro está en marcha

    [SerializeField]
    private TMP_Text _text; // Referencia al componente TMP_Text para mostrar el tiempo

    private float _elapsedTime; // Tiempo transcurrido en segundos

    void Start()
    {
        _elapsedTime = startTime;
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (timerRunning)
        {
            _elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        int hours = Mathf.FloorToInt(_elapsedTime / 3600);
        int minutes = Mathf.FloorToInt((_elapsedTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(_elapsedTime % 60);

        string timeString = "";

        if (displayHrs)
        {
            timeString += hours.ToString("00") + ":";
        }

        if (displayMins || displayHrs)
        {
            timeString += minutes.ToString("00") + ":";
        }

        timeString += seconds.ToString("00");

        _text.text = timeString;
    }

    public void StartTimer()
    {
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void ResetTimer()
    {
        _elapsedTime = startTime;
        UpdateTimerDisplay();
    }
}
