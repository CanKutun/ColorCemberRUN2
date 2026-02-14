using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;

public class BannerAdManager : MonoBehaviour
{
    private BannerView bannerView;

    void Start()
    {
        MobileAds.Initialize(initStatus => { });

        // Sadece oyun sahnesinde banner baþlat
        if (SceneManager.GetActiveScene().name == "oyun")
        {
            StartCoroutine(ShowBannerAfterDelay());
        }
    }

    IEnumerator ShowBannerAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        RequestBanner();
    }

    void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3601362388931534/3962350486";
#else
        string adUnitId = "unused";
#endif

        int width = AdSize.FullWidth;
        AdSize adaptiveSize =
            AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(width);

        bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);

        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
    }

    void OnDestroy()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
    }
}