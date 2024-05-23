using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Level")]
public class LevelConfig : ScriptableObject
{
    public string levelName;
    public int numberOfVehicles;
    public int minDistanceToAdd;
}
