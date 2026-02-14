using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    AudioSource src;

    void Start()
    {
        src = GetComponent<AudioSource>();

        bool musicOn = PlayerPrefs.GetInt("MUSIC", 1) == 1;

        if (musicOn)
            src.Play();
        else
            src.Stop();
    }

    public void SetMusic(bool on)
    {
        if (on)
            src.Play();
        else
            src.Stop();
    }
}