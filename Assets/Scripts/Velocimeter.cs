using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Velocimeter : MonoBehaviour
{
    [SerializeField] private GameObject velNeedle;
    [SerializeField] private TextMeshProUGUI velText;
    

    public float maxSpeed;
    private float angle;
    void Start()
    {
        velNeedle.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        if(maxSpeed <= 0){
            maxSpeed = 1;
        }
    }

    void Update() {
        velNeedle.transform.rotation = Quaternion.Euler(
            velNeedle.transform.eulerAngles.x,
            velNeedle.transform.eulerAngles.y,
            angle + 270f
        );
    }

    public void SetVelocityTo(float speed){
        velText.text = ((int)speed).ToString();
        angle = _CalculateAngle(speed);
    }

    private float _CalculateAngle(float currentSpeed)
    {
        // currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        float angle = 180 * (1 - (currentSpeed / maxSpeed));

        if (angle < 0)
        {
            angle = 0;
        }

        if (angle > 180)
        {
            angle = 180;
        }

        return angle;
    }
}
