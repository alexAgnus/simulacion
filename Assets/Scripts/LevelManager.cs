using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<LevelConfig> levels;
    public LevelBtn levelBtnPref;
    public RectTransform container;
    // Start is called before the first frame update
    void Start()
    {
        foreach (LevelConfig level in levels)
        {
            var btn = Instantiate(levelBtnPref, container);
            btn.SetLevel(level);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
