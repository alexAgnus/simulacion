using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Velocimeter : MonoBehaviour
{
    [SerializeField] private GameObject velNeedle;
    [SerializeField] private TextMeshProUGUI velText;
    

    public float MaxSpeed;
    private float angle;
    void Start()
    {
        velNeedle.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        if(MaxSpeed <= 0){
            MaxSpeed = 1;
        }
    }


    void Update(){
        velNeedle.transform.rotation = Quaternion.Euler(0f, 0f, 90f - angle);

    }

    public void SetVelocityTo(float speed){
        angle = (speed / MaxSpeed) * 180f;
        velText.text = ((int)speed).ToString();
    }
}
