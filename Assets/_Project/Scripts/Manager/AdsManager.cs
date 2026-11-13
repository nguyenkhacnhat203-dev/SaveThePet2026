
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsManager : Singleton<AdsManager>
{
    private BannerView banner;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;
    private AppOpenAd appOpenAd;

    [Header("Ads Control")]
    public bool IsAdsDisabledByRemoteConfig;

    private IAPManager iapManager;

    private bool IsRemoveAdsPurchased =>
        iapManager != null && iapManager.IsRemoveAdsPurchased;

    private bool IsAdsDisabled =>
        IsRemoveAdsPurchased || IsAdsDisabledByRemoteConfig;

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
        iapManager = FindObjectOfType<IAPManager>();

        if (iapManager != null)
        {
            iapManager.OnPurchaseSuccess += HandlePurchaseSuccess;
        }

        RefreshAdsState();

        if (!IsAdsDisabled)
        {
            InitializeAds();
        }
    }

    private void InitializeAds()
    {
        if (IsAdsDisabled)
            return;

        MobileAds.Initialize(init =>
        {
            if (IsAdsDisabled)
                return;

            LoadBanner();
            LoadInterstitial();
            LoadRewarded();
            LoadAppOpen();
        });
    }

    // =========================================================
    // ADS CONTROL
    // =========================================================

    private void HandlePurchaseSuccess(string productId)
    {
        if (productId != "remove_ads")
            return;

        RefreshAdsState();
    }

    public void RefreshAdsState()
    {
        if (IsAdsDisabled)
        {
            DisableAllAds();
        }
    }

    private void DisableAllAds()
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

        onRewardAndClosed = null;
        onAppOpenClosed = null;
        rewardEarned = false;
    }

    // =========================================================
    // BANNER
    // =========================================================

    public void LoadBanner()
    {
        if (IsAdsDisabled)
            return;

        if (banner != null)
            banner.Destroy();

        banner = new BannerView(
            bannerID,
            AdSize.Banner,
            AdPosition.Bottom
        );

        banner.LoadAd(new AdRequest());
    }

    public void ShowBanner()
    {
        if (IsAdsDisabled || banner == null)
            return;

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
        if (IsAdsDisabled)
            return;

        InterstitialAd.Load(
            interID,
            new AdRequest(),
            (ad, error) =>
            {
                if (IsAdsDisabled)
                {
                    ad?.Destroy();
                    return;
                }

                if (error != null)
                    return;

                if (interstitial != null)
                    interstitial.Destroy();

                interstitial = ad;

                interstitial.OnAdFullScreenContentClosed += () =>
                {
                    interstitial?.Destroy();
                    interstitial = null;

                    LoadInterstitial();
                };

                interstitial.OnAdFullScreenContentFailed += error =>
                {
                    interstitial?.Destroy();
                    interstitial = null;

                    LoadInterstitial();
                };
            }
        );
    }

    public void ShowInterstitial()
    {
        if (IsAdsDisabled)
            return;

        if (interstitial != null && interstitial.CanShowAd())
        {
            interstitial.Show();
        }
    }

    // =========================================================
    // REWARDED
    // =========================================================

    public void LoadRewarded()
    {
        if (IsAdsDisabled)
            return;

        RewardedAd.Load(
            rewardID,
            new AdRequest(),
            (ad, error) =>
            {
                if (IsAdsDisabled)
                {
                    ad?.Destroy();
                    return;
                }

                if (error != null)
                    return;

                if (rewardedAd != null)
                    rewardedAd.Destroy();

                rewardedAd = ad;

                rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    if (rewardEarned)
                        onRewardAndClosed?.Invoke();

                    onRewardAndClosed = null;
                    rewardEarned = false;

                    rewardedAd?.Destroy();
                    rewardedAd = null;

                    LoadRewarded();
                };

                rewardedAd.OnAdFullScreenContentFailed += error =>
                {
                    onRewardAndClosed = null;
                    rewardEarned = false;

                    rewardedAd?.Destroy();
                    rewardedAd = null;

                    LoadRewarded();
                };
            }
        );
    }

    public void ShowRewarded(Action onRewardAndClosed)
    {
        if (IsAdsDisabled)
            return;

        if (rewardedAd == null || !rewardedAd.CanShowAd())
            return;

        this.onRewardAndClosed = onRewardAndClosed;
        rewardEarned = false;

        rewardedAd.Show(reward =>
        {
            rewardEarned = true;
        });
    }

    // =========================================================
    // APP OPEN
    // =========================================================

    public void LoadAppOpen()
    {
        if (IsAdsDisabled)
            return;

        AppOpenAd.Load(
            appOpenID,
            new AdRequest(),
            (ad, error) =>
            {
                if (IsAdsDisabled)
                {
                    ad?.Destroy();
                    return;
                }

                if (error != null)
                    return;

                if (appOpenAd != null)
                    appOpenAd.Destroy();

                appOpenAd = ad;

                appOpenAd.OnAdFullScreenContentClosed += () =>
                {
                    onAppOpenClosed?.Invoke();
                    onAppOpenClosed = null;

                    appOpenAd?.Destroy();
                    appOpenAd = null;

                    LoadAppOpen();
                };

                appOpenAd.OnAdFullScreenContentFailed += error =>
                {
                    onAppOpenClosed?.Invoke();
                    onAppOpenClosed = null;

                    appOpenAd?.Destroy();
                    appOpenAd = null;

                    LoadAppOpen();
                };
            }
        );
    }

    public void ShowAppOpen(Action onClosed = null)
    {
        if (IsAdsDisabled)
        {
            onClosed?.Invoke();
            return;
        }

        if (appOpenAd != null && appOpenAd.CanShowAd())
        {
            onAppOpenClosed = onClosed;
            appOpenAd.Show();
        }
        else
        {
            onClosed?.Invoke();
        }
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDestroy()
    {
        if (iapManager != null)
        {
            iapManager.OnPurchaseSuccess -= HandlePurchaseSuccess;
        }

        DisableAllAds();
    }
}
