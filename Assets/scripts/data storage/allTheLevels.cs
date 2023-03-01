using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class allTheLevels : MonoBehaviour
{
    public static allTheLevels instance;
    public LevelData[] LEVELS { get; private set; }
    
    void Start()
    {
        instance = this;
        LEVELS = GetComponentsInChildren<LevelData>();
    }

    public void LoadALevel(int index)
    {
        TargetsManager.instance.LoadLevel(LEVELS[index]);
    }
}
