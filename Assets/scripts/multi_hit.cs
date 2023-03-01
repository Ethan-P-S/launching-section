using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class multi_hit : MonoBehaviour
{
    [SerializeField] Sprite[] images;
    int hitCount = 0;
    public void Hit()
    {
        if (hitCount < images.Length)
        {
            GetComponent<SpriteRenderer>().sprite = images[hitCount];
        }
            hitCount++;
        if (hitCount >= images.Length)
        {
            Destroy(this);
        }
    }
}
