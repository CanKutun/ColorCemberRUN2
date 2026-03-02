using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    [Header("AdMob IDs (Android)")]
    public string interstitialAdUnitId = "ca-app-pub-3601362388931534/8660143396";
    public string rewardedAdUnitId = "ca-app-pub-3601362388931534/5243642562";

    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    // 🔒 Reklam kilidi
    private bool adIsOpen = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            LoadInterstitial();
            LoadRewarded();
        });
    }

    // =======================
    // INTERSTITIAL
    // =======================

    void LoadInterstitial()
    {
        InterstitialAd.Load(interstitialAdUnitId, new AdRequest(),
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.Log("Interstitial yüklenemedi");
                    return;
                }

                interstitialAd = ad;
            });
    }

    public bool IsInterstitialReady()
    {
        return interstitialAd != null && interstitialAd.CanShowAd();
    }

    public void ShowInterstitial(Action onClosed = null)
    {
        if (!IsInterstitialReady())
        {
            onClosed?.Invoke();
            return;
        }

        InterstitialAd currentAd = interstitialAd;
        interstitialAd = null;

        currentAd.OnAdFullScreenContentOpened += () =>
        {
            if (adIsOpen) return;
            adIsOpen = true;

            Time.timeScale = 0f;
            AudioListener.pause = true;
        };

        currentAd.OnAdFullScreenContentClosed += () =>
        {
            if (!adIsOpen) return;
            adIsOpen = false;

            Time.timeScale = 1f;
            AudioListener.pause = false;

            LoadInterstitial();

            StartCoroutine(InvokeNextFrame(onClosed));
        };

        currentAd.Show();
    }

    // =======================
    // REWARDED
    // =======================

    void LoadRewarded()
    {
        RewardedAd.Load(rewardedAdUnitId, new AdRequest(),
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.Log("Rewarded yüklenemedi");
                    return;
                }

                rewardedAd = ad;
            });
    }

    public bool IsRewardedReady()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }

    public void ShowRewarded(Action onReward, Action onFail = null)
    {
        if (!IsRewardedReady())
        {
            onFail?.Invoke();
            return;
        }

        bool earned = false;

        RewardedAd currentAd = rewardedAd;
        rewardedAd = null;

        currentAd.OnAdFullScreenContentOpened += () =>
        {
            if (adIsOpen) return;
            adIsOpen = true;

            Time.timeScale = 0f;
            AudioListener.pause = true;
        };

        currentAd.OnAdFullScreenContentClosed += () =>
        {
            if (!adIsOpen) return;
            adIsOpen = false;

            Time.timeScale = 1f;
            AudioListener.pause = false;

            LoadRewarded();

            StartCoroutine(InvokeNextFrame(() =>
            {
                if (earned)
                    onReward?.Invoke();
                else
                    onFail?.Invoke();
            }));
        };

        currentAd.Show(reward =>
        {
            earned = true;
        });
    }

    IEnumerator InvokeNextFrame(Action action)
    {
        yield return null;
        action?.Invoke();
    }
}