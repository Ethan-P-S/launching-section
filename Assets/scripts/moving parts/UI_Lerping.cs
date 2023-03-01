using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Lerping : MonoBehaviour
{
    public static UI_Lerping instance;

    [SerializeField] Transform[] LerpPoints;

    public Transform[] Lerp_Points()
    {
        return LerpPoints;
    }

    void Start()
    {
        instance = this;    
    }

    public float LerpTheX(Transform left, Transform right, Transform input)
    {
        return Mathf.InverseLerp(left.position.x, right.position.x, input.position.x);
    }

    public float LerpTheY(Transform top, Transform bottom, Transform input)
    {
        return Mathf.InverseLerp(top.position.y, bottom.position.y, input.position.y);
    }

}
