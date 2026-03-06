using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
public class MainMenuButtonController : MonoBehaviour
{
    [Header("Name UI")]
    public GameObject namePanel;
    public TMP_InputField nameInput;
    public Button okButton;

    [Header("Menu Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button leaderboardButton;
    public Button exitButton;
    public Button continueButton;

    [Header("Scenes")]
    public string araSahneScene = "AraSahne";
    public string settingsScene = "Settings";
    public string leaderboardScene = "Tablo";

    public TMP_Text continueScoreText;

    void Start()
    {
        // 1️ İsim var mı?
        if (!PlayerPrefs.HasKey("PlayerName"))
        {
            namePanel.SetActive(true);
        }
        else
        {
            namePanel.SetActive(false);
        }

        // 2️ OK butonu (SADECE isim kaydeder)
        okButton.onClick.AddListener(SaveName);

        // 3️ Play butonu (OYUNU BAŞLATIR)
        playButton.onClick.AddListener(() =>
        {
            PlayerPrefs.DeleteKey("SavedScore");
            PlayerPrefs.DeleteKey("SavedPhase");
            PlayerPrefs.DeleteKey("SavedZ");

            PlayerPrefs.SetInt("ContinueGame", 0); // Yeni oyun başlatmak istediğimizi belirtiyoruz

            SceneManager.LoadScene(araSahneScene);
        });

        settingsButton.onClick.AddListener(() =>
            SceneManager.LoadScene(settingsScene));

        leaderboardButton.onClick.AddListener(() =>
            SceneManager.LoadScene(leaderboardScene));

        exitButton.onClick.AddListener(Application.Quit);

        // DEVAM butonu kontrolü
        int savedScore = PlayerPrefs.GetInt("SavedScore", 0);

        if (savedScore <= 0)
        {
            continueButton.gameObject.SetActive(false);
        }
        else
        {
            if (LocalizationSettings.SelectedLocale.Identifier.Code == "tr")
            {
                continueScoreText.text = "SKOR " + savedScore;
            }
            else
            {
                continueScoreText.text = "SCORE " + savedScore;
            }
        }

        // DEVAM butonuna basınca
        continueButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt("ContinueGame", 1); // Devam etmek istediğimizi belirtiyoruz
            SceneManager.LoadScene(araSahneScene);
        });
    }

    void SaveName()
    {
        if (string.IsNullOrEmpty(nameInput.text))
            return;

        string playerName = nameInput.text;

        // PlayerPrefs
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();

        // PlayFab DisplayName
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = playerName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(
            request,
            result => Debug.Log("PlayFab isim set edildi: " + playerName),
            error => Debug.LogError(error.GenerateErrorReport())
        );

        // Panel kapat
        namePanel.SetActive(false);
    }
}