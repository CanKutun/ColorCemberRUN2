using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MusicToggleHelper : MonoBehaviour
{
    public void ToggleMusic()
    {
        bool musicOn = PlayerPrefs.GetInt("MUSIC", 1) == 1;
        musicOn = !musicOn;

        PlayerPrefs.SetInt("MUSIC", musicOn ? 1 : 0);

        GameObject holder = GameObject.FindWithTag("MSC");
        if (holder != null)
        {
            MusicController mc = holder.GetComponent<MusicController>();
            if (mc != null)
                mc.SetMusic(musicOn);
        }
    }
}