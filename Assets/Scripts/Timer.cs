using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public int remainingTime = 0;
    public bool displayHrs = true;
    public bool displayMins = true;
    public bool timerRunning = false;
    [SerializeField]
    private TMP_Text text;
    private IEnumerator TicToc(){
        while(remainingTime > 0 && timerRunning){
            remainingTime -= 1;
            UpdateText();
            yield return new WaitForSeconds(1);

        }
    }


    public void ResetTimer(int hrs = 0, int mins = 0, int secs = 0){
        remainingTime = (hrs * 3600) + (mins * 60) + secs;
        
        UpdateText();
    }

    public void StartTimer(){
        timerRunning = true;
        StartCoroutine("TicToc");
    }

    public void PauseTimer(){
        timerRunning = false;
    }

    private void UpdateText(){
        int temp = remainingTime;
        string timerText = "";

        if(displayHrs && displayMins){
            int hrs = temp / 3600; 
            temp = temp - (hrs * 3600);

            if(hrs < 10) timerText += "0";
            timerText += $"{hrs}:";
        }
        if(displayMins){
            int mins = temp / 60;
            temp = temp - (mins * 60);
            if(mins < 10) timerText += "0";
            timerText += $"{mins}:";
        }
        
        int secs = temp;

        if(secs < 10) timerText += "0";
        timerText += $"{secs}";

        text.text = timerText;
    }
}
