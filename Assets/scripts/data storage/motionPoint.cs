using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class motionPoint : MonoBehaviour
{
    public int INDEX;
    public Collider2D col;
    public float progressValue = 1;

    public void SetFloats(float newDist)
    {
        progressValue = newDist;   
    }

    public int ExpectedTicks(float speed)
    {
        return Mathf.RoundToInt((progressValue / speed) * 50);
    }
}
