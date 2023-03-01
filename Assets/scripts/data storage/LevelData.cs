using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    public PhaseData[] Phases { get; private set; }

    void Start()
    {
        Phases = gameObject.GetComponents<PhaseData>();
    }
}
