using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public static Sounds instance;
    [SerializeField] AudioClip[] theClips;
    AudioSource aud;
    void Start()
    {
        instance = this;
        aud = GetComponent<AudioSource>();
    }

    public AudioClip GetClip(int index)
    {
        index = Mathf.Abs(index);
        if (index < theClips.Length)
        {
            return theClips[index];
        }
        else return null;
    }

    public void PlayClip(int index)
    {
        aud.PlayOneShot(GetClip(index));
    }
}
