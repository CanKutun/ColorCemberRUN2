using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button btnMusic;
    public Button btnSfx;
    public Button btnLanguage;
    public Button btnBack;

    [Header("Texts")]
    public TextMeshProUGUI txtMusic;
    public TextMeshProUGUI txtSfx;
    public TextMeshProUGUI txtLanguage;
    public TextMeshProUGUI txtBack;

    bool musicOn;
    bool sfxOn;
    int language; // 0 = TR, 1 = EN

    void Start()
    {
        musicOn = PlayerPrefs.GetInt("MUSIC", 1) == 1;
        sfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;
        language = PlayerPrefs.GetInt("Lang", 0);

        if (btnMusic != null)
            btnMusic.onClick.AddListener(ToggleMusic);

        btnSfx.onClick.AddListener(ToggleSfx);
        btnLanguage.onClick.AddListener(ChangeLanguage);
        btnBack.onClick.AddListener(BackToMenu);

        ApplySfx();
        RefreshTexts();
    }

    void ToggleMusic()
    {
        musicOn = !musicOn;
        PlayerPrefs.SetInt("MUSIC", musicOn ? 1 : 0);

        // Sahnedeki müzik kaynaðýný bul ve uygula
        var musics = GameObject.FindGameObjectsWithTag("MSC");
        foreach (var m in musics)
        {
            AudioSource a = m.GetComponent<AudioSource>();
            if (a != null)
            {
                if (musicOn) a.Play();
                else a.Stop();
            }
        }

        RefreshTexts();
    }

    void ToggleSfx()
    {
        sfxOn = !sfxOn;
        PlayerPrefs.SetInt("SFX", sfxOn ? 1 : 0);

        ApplySfx();
        RefreshTexts();
    }

    void ApplySfx()
    {
        var sources = FindObjectsOfType<AudioSource>(true);
        foreach (var src in sources)
        {
            if (src != null && src.CompareTag("SFX"))
            {
                src.mute = !sfxOn;
            }
        }
    }

    void ChangeLanguage()
    {
        language = language == 0 ? 1 : 0;
        PlayerPrefs.SetInt("Lang", language);
        RefreshTexts();
    }

    void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void RefreshTexts()
    {
        if (language == 0) // Türkçe
        {
            txtMusic.text = "MÜZÝK: " + (musicOn ? "AÇIK" : "KAPALI");
            txtSfx.text = "EFEKT: " + (sfxOn ? "AÇIK" : "KAPALI");
            txtLanguage.text = "DÝL: TÜRKÇE";
            txtBack.text = "GERÝ";
        }
        else // English
        {
            txtMusic.text = "MUSIC: " + (musicOn ? "ON" : "OFF");
            txtSfx.text = "SFX: " + (sfxOn ? "ON" : "OFF");
            txtLanguage.text = "LANGUAGE: ENGLISH";
            txtBack.text = "BACK";
        }
    }
}