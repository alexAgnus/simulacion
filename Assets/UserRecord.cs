using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserRecord : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userRecordText;
    public int userId;
    public string formattedCurrentDateTime;

    void Awake()
    {
        userId = Random.Range(100000, 999999);
    }

    void Start()
    {
        formattedCurrentDateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        userRecordText.text = "[#" + userId + "] " + formattedCurrentDateTime;
    }

    void FixedUpdate()
    {
        formattedCurrentDateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        userRecordText.text = "[#" + userId + "] " + formattedCurrentDateTime;
    }
}
