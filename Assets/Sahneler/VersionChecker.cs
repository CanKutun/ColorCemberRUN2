using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class VersionChecker : MonoBehaviour
{
    public GameObject updatePanel;
    public TextMeshProUGUI updateText;

    string versionURL = "https://raw.githubusercontent.com/CanKutun/ColorCemberRUN2/main/version.txt";

    void Start()
    {
        updatePanel.SetActive(false);
        StartCoroutine(CheckVersion());
    }

    IEnumerator CheckVersion()
    {
        UnityWebRequest www = UnityWebRequest.Get(versionURL);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string onlineVersion = www.downloadHandler.text.Trim();

            Debug.Log("Online Version: " + onlineVersion);
            Debug.Log("Local Version: " + Application.version);

            if (onlineVersion != Application.version)
            {
                SetLanguageText();
                updatePanel.SetActive(true);
                Time.timeScale = 0f;
            }
        }
        else
        {
            Debug.Log("Version check failed: " + www.error);
        }
    }

    void SetLanguageText()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Turkish:
                updateText.text = "Yeni sürüm mevcut.\nLütfen güncelleyin.";
                break;

            default:
                updateText.text = "A new version is available.\nPlease update.";
                break;
        }
    }
}