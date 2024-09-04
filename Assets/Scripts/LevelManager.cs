using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour
{
    public List<LevelConfig> levels;
    public LevelBtn levelBtnPref;
    public RectTransform container;
    // Start is called before the first frame update
    void Start()
    {
        var _eventSystem = EventSystem.current;
        foreach (LevelConfig level in levels)
        {
            var btn = Instantiate(levelBtnPref, container);
            btn.SetLevel(level);
            _eventSystem.SetSelectedGameObject(btn.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
