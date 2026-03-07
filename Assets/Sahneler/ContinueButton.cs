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

        InvokeRepeating("CheckAdReady", 0f, 0.5f);
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
        else
        {
            Debug.Log("Rewarded reklam hazýr deðil");
        }
    }

    void LoadAraSahne()
    {
        SceneManager.LoadScene("AraSahne");
    }

    void CheckAdReady()
    {
        if (AdsManager.Instance != null && AdsManager.Instance.IsRewardedReady())
        {
            devamButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
        else
        {
            devamButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
    }


}




