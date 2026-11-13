
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsManager : Singleton<AdsManager>
{
    private BannerView banner;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;
    private AppOpenAd appOpenAd;

    [Header("AdMob Test IDs")]
    [SerializeField] private string bannerID = "ca-app-pub-3940256099942544/6300978111";
    [SerializeField] private string interID = "ca-app-pub-3940256099942544/1033173712";
    [SerializeField] private string rewardID = "ca-app-pub-3940256099942544/5224354917";
    [SerializeField] private string appOpenID = "ca-app-pub-3940256099942544/9257395921";

    private Action onRewardAndClosed;
    private Action onAppOpenClosed;

    private bool rewardEarned;

    private void Start()
    {
        InitializeAds();
    }

    private void InitializeAds()
    {
        MobileAds.Initialize(init =>
        {
            //Debug.Log("AdMob Initialized");

            LoadBanner();
            LoadInterstitial();
            LoadRewarded();
            LoadAppOpen();
        });
    }

    // =========================================================
    // BANNER
    // =========================================================

    public void LoadBanner()
    {
        if (banner != null)
            banner.Destroy();

        banner = new BannerView(
            bannerID,
            AdSize.Banner,
            AdPosition.Bottom
        );

        banner.LoadAd(new AdRequest());

        //Debug.Log("Banner Load Requested");
    }

    public void ShowBanner()
    {
        if (banner != null)
            banner.Show();
    }

    public void HideBanner()
    {
        if (banner != null)
            banner.Hide();
    }

    // =========================================================
    // INTERSTITIAL
    // =========================================================

    public void LoadInterstitial()
    {
        InterstitialAd.Load(
            interID,
            new AdRequest(),
            (ad, error) =>
            {
                if (error != null)
                {
                    //Debug.LogWarning("Interstitial Load Failed: " + error);
                    return;
                }

                if (interstitial != null)
                    interstitial.Destroy();

                interstitial = ad;

                //Debug.Log("Interstitial Loaded");

                interstitial.OnAdFullScreenContentClosed += () =>
                {
                    //Debug.Log("Interstitial Closed");

                    interstitial.Destroy();
                    interstitial = null;

                    LoadInterstitial();
                };

                interstitial.OnAdFullScreenContentFailed += error =>
                {
                    //Debug.LogWarning("Interstitial Show Failed: " + error);

                    interstitial.Destroy();
                    interstitial = null;

                    LoadInterstitial();
                };
            }
        );
    }

    public void ShowInterstitial()
    {
        if (interstitial != null && interstitial.CanShowAd())
        {
            interstitial.Show();
            //Debug.Log("Showing Interstitial");
        }
        else
        {
            //Debug.Log("Interstitial is not ready.");
        }
    }

    // =========================================================
    // REWARDED
    // =========================================================

    public void LoadRewarded()
    {
        RewardedAd.Load(
            rewardID,
            new AdRequest(),
            (ad, error) =>
            {
                if (error != null)
                {
                    //Debug.LogWarning("Rewarded Load Failed: " + error);
                    return;
                }

                if (rewardedAd != null)
                    rewardedAd.Destroy();

                rewardedAd = ad;

                //Debug.Log("Rewarded Loaded");

                rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    //Debug.Log("Rewarded Closed");

                    if (rewardEarned)
                        onRewardAndClosed?.Invoke();

                    onRewardAndClosed = null;
                    rewardEarned = false;

                    rewardedAd.Destroy();
                    rewardedAd = null;

                    LoadRewarded();
                };

                rewardedAd.OnAdFullScreenContentFailed += error =>
                {
                    //Debug.LogWarning("Rewarded Show Failed: " + error);

                    onRewardAndClosed = null;
                    rewardEarned = false;

                    rewardedAd.Destroy();
                    rewardedAd = null;

                    LoadRewarded();
                };
            }
        );
    }

    public void ShowRewarded(Action onRewardAndClosed)
    {
        if (rewardedAd == null || !rewardedAd.CanShowAd())
        {
            //Debug.Log("Rewarded is not ready.");
            return;
        }

        this.onRewardAndClosed = onRewardAndClosed;
        rewardEarned = false;

        rewardedAd.Show(reward =>
        {
            rewardEarned = true;
            //Debug.Log("Reward Earned");
        });
    }

    // =========================================================
    // APP OPEN
    // =========================================================

    public void LoadAppOpen()
    {
        AppOpenAd.Load(
            appOpenID,
            new AdRequest(),
            (ad, error) =>
            {
                if (error != null)
                {
                    //Debug.LogWarning("AppOpen Load Failed: " + error);
                    return;
                }

                if (appOpenAd != null)
                    appOpenAd.Destroy();

                appOpenAd = ad;

                //Debug.Log("AppOpen Loaded");

                appOpenAd.OnAdFullScreenContentClosed += () =>
                {
                    //Debug.Log("AppOpen Closed");

                    onAppOpenClosed?.Invoke();
                    onAppOpenClosed = null;

                    appOpenAd.Destroy();
                    appOpenAd = null;

                    LoadAppOpen();
                };

                appOpenAd.OnAdFullScreenContentFailed += error =>
                {
                    //Debug.LogWarning("AppOpen Show Failed: " + error);

                    onAppOpenClosed?.Invoke();
                    onAppOpenClosed = null;

                    appOpenAd.Destroy();
                    appOpenAd = null;

                    LoadAppOpen();
                };
            }
        );
    }

    public void ShowAppOpen(Action onClosed = null)
    {
        if (appOpenAd != null && appOpenAd.CanShowAd())
        {
            onAppOpenClosed = onClosed;
            appOpenAd.Show();
            //Debug.Log("Showing AppOpen");
        }
        else
        {
            //Debug.Log("AppOpen is not ready.");
            onClosed?.Invoke();
        }
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDestroy()
    {
        if (banner != null)
        {
            banner.Destroy();
            banner = null;
        }

        if (interstitial != null)
        {
            interstitial.Destroy();
            interstitial = null;
        }

        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }
    }
}
