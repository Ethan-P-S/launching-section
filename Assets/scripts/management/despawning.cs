using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class despawning : MonoBehaviour
{
    [SerializeField] float elapsedTime;
    float count = 0;
    void Update()
    {
        count += Time.deltaTime;
        if(count > elapsedTime)
        {
            Destroy(gameObject);
        }
    }
}
