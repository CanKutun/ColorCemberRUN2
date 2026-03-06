using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButton : MonoBehaviour
{
    public GameObject devamButton;

    void Start()
    {
        if (PlayerPrefs.GetInt("SavedScore", 0) <= 0)
        {
            devamButton.SetActive(false);
        }
    }

    public void ContinueGame()
    {
        if (AdsManager.Instance != null && AdsManager.Instance.IsRewardedReady())
        {
            AdsManager.Instance.ShowRewarded(() =>
            {
                PlayerPrefs.SetInt("NeedAdForContinue", 0);
                LoadAraSahne();
            });
        }
        else if (AdsManager.Instance != null && AdsManager.Instance.IsInterstitialReady())
        {
            AdsManager.Instance.ShowInterstitial(() =>
            {
                PlayerPrefs.SetInt("NeedAdForContinue", 0);
                LoadAraSahne();
            });
        }
        else
        {
            Debug.Log("Reklam hazýr deðil");
        }
    }

    void LoadAraSahne()
    {
        SceneManager.LoadScene("AraSahne");
    }
}

