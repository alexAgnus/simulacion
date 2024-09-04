using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelBtn : MonoBehaviour
{
    public TMP_Text tmpText;
    public Button button;
    public string levelName;

    public void SetLevel(LevelConfig level)
    {
        tmpText.text = level.levelName;
        button.onClick.AddListener(delegate
        {
            PlayerPrefs.SetInt("nrOfVehicles", level.numberOfVehicles);
            PlayerPrefs.SetInt("minDistanceToAdd", level.minDistanceToAdd);
            SceneManager.LoadScene(levelName);
        });
    }
}
