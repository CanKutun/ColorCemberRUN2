using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsAudioManager : MonoBehaviour
{
    public static SettingsAudioManager Instance;

    public bool musicOn = true;
    public bool sfxOn = true;
    AudioSource[] allSources;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // FPS SABÝTLEME
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        LoadSettings();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySettings();
    }

    void RefreshSources()
    {
        allSources = FindObjectsOfType<AudioSource>(true);
    }

    public void ToggleSFX()
    {
        sfxOn = !sfxOn;
        PlayerPrefs.SetInt("SFX", sfxOn ? 1 : 0);
        ApplySettings();
    }

    public void ToggleMUSIC()
    {
        musicOn = !musicOn;
        PlayerPrefs.SetInt("MUSIC", musicOn ? 1 : 0);
        ApplySettings();
    }


    void LoadSettings()
    {
        sfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;
        musicOn = PlayerPrefs.GetInt("MUSIC", 1) == 1;
    }

    public void ApplySettings()
    {
        RefreshSources();

        foreach (var src in allSources)
        {
            if (src == null) continue;

            if (src.CompareTag("SFX"))
            {
                src.mute = !sfxOn;

                if (!sfxOn && src.isPlaying)
                    src.Stop();
            }
        }
    }
}