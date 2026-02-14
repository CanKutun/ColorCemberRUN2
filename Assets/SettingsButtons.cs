using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButtons : MonoBehaviour
{
    /*public void ToggleMusic()
    {
        if (SettingsAudioManager.Instance != null)
            SettingsAudioManager.Instance.ToggleMusic();
    }*/

    public void ToggleSFX()
    {
        if (SettingsAudioManager.Instance != null)
            SettingsAudioManager.Instance.ToggleSFX();
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}