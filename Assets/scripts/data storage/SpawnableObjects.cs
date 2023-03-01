using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableObjects : MonoBehaviour
{
    public static SpawnableObjects instance;
    void Start()
    {
        instance = this;
    }

    [SerializeField] GameObject[] targets, walls, arrows;

    public GameObject GetObject(int array, int index)
    {
        array = Mathf.Clamp(array, 0, 2);

        switch (array)
        {
            case 0:
                return targets[index];
            case 1:
                return walls[index];
            case 2:
                return arrows[index];
            default:
                return targets[0];
        }
    }
}

