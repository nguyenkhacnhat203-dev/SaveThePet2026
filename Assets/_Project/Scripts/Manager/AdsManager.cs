
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

    [Header("AdMob Test IDs")]
    [SerializeField] private string bannerID = "ca-app-pub-3940256099942544/6300978111";
    [SerializeField] private string interID = "ca-app-pub-3940256099942544/1033173712";
    [SerializeField] private string rewardID = "ca-app-pub-3940256099942544/5224354917";
    [SerializeField] private string appOpenID = "ca-app-pub-3940256099942544/9257395921";

    private IAPManager iapManager;

    private Action onRewardAndClosed;
    private Action onAppOpenClosed;
    private Action onInterstitialClosed;

    private bool rewardEarned;

    private bool isAdsInitializing;
    private bool isAdsInitialized;

    private bool isLoadingInterstitial;
    private bool isInterstitialPending;
    private bool isInterstitialShowing;

    private bool IsRemoveAdsPurchased =>
        iapManager != null && iapManager.IsRemoveAdsPurchased;

    private bool IsAdsDisabled =>
        IsRemoveAdsPurchased || IsAdsDisabledByRemoteConfig;

    private void Start()
    {
        iapManager = FindObjectOfType<IAPManager>();

        if (iapManager != null)
        {
            iapManager.OnPurchaseSuccess += HandlePurchaseSuccess;
        }

        RemoteConfigManager remoteConfig = RemoteConfigManager.Instance;

        if (remoteConfig != null)
        {
            remoteConfig.OnInitializedEvent += HandleRemoteConfigInitialized;
            remoteConfig.OnTimeReached += HandleTimeReached;

            if (remoteConfig.IsInitialized)
            {
                HandleRemoteConfigInitialized();
            }
        }

        RefreshAdsState();
    }

    private void Update()
    {
        if (isInterstitialPending && !isInterstitialShowing)
        {
            TryShowInterstitial();
        }
    }

    private void HandleRemoteConfigInitialized()
    {
        RemoteConfigManager remoteConfig = RemoteConfigManager.Instance;

        if (remoteConfig == null)
            return;

        IsAdsDisabledByRemoteConfig = !remoteConfig.adsEnabled;

        RefreshAdsState();
    }

    private void HandleTimeReached()
    {
        if (IsAdsDisabled)
            return;

        ShowInterstitial();
    }

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
            return;
        }

        InitializeAds();
    }

    private void InitializeAds()
    {
        if (IsAdsDisabled || isAdsInitialized || isAdsInitializing)
            return;

        isAdsInitializing = true;

        MobileAds.Initialize(init =>
        {
            isAdsInitializing = false;

            if (IsAdsDisabled)
                return;

            isAdsInitialized = true;

            LoadInterstitial();
            LoadRewarded();
            LoadAppOpen();
        });
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

        CancelInvoke(nameof(RetryLoadInterstitial));

        isLoadingInterstitial = false;
        isInterstitialPending = false;
        isInterstitialShowing = false;

        onInterstitialClosed = null;
        onRewardAndClosed = null;
        onAppOpenClosed = null;

        rewardEarned = false;
    }

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

    public void LoadInterstitial()
    {
        if (IsAdsDisabled || !isAdsInitialized ||
            isLoadingInterstitial || interstitial != null)
            return;

        isLoadingInterstitial = true;

        InterstitialAd.Load(
            interID,
            new AdRequest(),
            (ad, error) =>
            {
                isLoadingInterstitial = false;

                if (IsAdsDisabled)
                {
                    ad?.Destroy();
                    return;
                }

                if (error != null || ad == null)
                {
                    Debug.LogWarning("Interstitial Load Failed: " + error);
                    Invoke(nameof(RetryLoadInterstitial), 10f);
                    return;
                }

                interstitial?.Destroy();
                interstitial = ad;

                interstitial.OnAdFullScreenContentClosed += HandleInterstitialClosed;
                interstitial.OnAdFullScreenContentFailed += HandleInterstitialFailed;

                TryShowInterstitial();
            }
        );
    }

    private void RetryLoadInterstitial()
    {
        if (IsAdsDisabled)
            return;

        LoadInterstitial();
    }

    public void ShowInterstitial(Action onClosed = null)
    {
        if (IsAdsDisabled)
        {
            onClosed?.Invoke();
            return;
        }

        if (isInterstitialPending || isInterstitialShowing)
            return;

        onInterstitialClosed = onClosed;
        isInterstitialPending = true;

        if (interstitial == null)
        {
            LoadInterstitial();
            return;
        }

        TryShowInterstitial();
    }

    private void TryShowInterstitial()
    {
        if (IsAdsDisabled || !isInterstitialPending || isInterstitialShowing)
            return;

        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            return;

        if (interstitial == null || !interstitial.CanShowAd())
        {
            LoadInterstitial();
            return;
        }

        isInterstitialPending = false;
        isInterstitialShowing = true;

        interstitial.Show();
    }

    private void HandleInterstitialClosed()
    {
        isInterstitialShowing = false;

        Action callback = onInterstitialClosed;
        onInterstitialClosed = null;

        interstitial?.Destroy();
        interstitial = null;

        RemoteConfigManager remoteConfig = RemoteConfigManager.Instance;

        if (remoteConfig != null && remoteConfig.IsTimeReached)
        {
            remoteConfig.RestartTimer();
        }

        callback?.Invoke();

        LoadInterstitial();
    }

    private void HandleInterstitialFailed(AdError error)
    {
        Debug.LogWarning("Interstitial Show Failed: " + error);

        isInterstitialShowing = false;

        interstitial?.Destroy();
        interstitial = null;

        RemoteConfigManager remoteConfig = RemoteConfigManager.Instance;

        if (!IsAdsDisabled && remoteConfig != null && remoteConfig.IsTimeReached)
        {
            isInterstitialPending = true;
        }

        Invoke(nameof(RetryLoadInterstitial), 5f);
    }

    public void LoadRewarded()
    {
        if (IsAdsDisabled || !isAdsInitialized)
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

                if (error != null || ad == null)
                    return;

                rewardedAd?.Destroy();
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

    public void LoadAppOpen()
    {
        if (IsAdsDisabled || !isAdsInitialized)
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

                if (error != null || ad == null)
                    return;

                appOpenAd?.Destroy();
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

    private void OnDestroy()
    {
        if (iapManager != null)
        {
            iapManager.OnPurchaseSuccess -= HandlePurchaseSuccess;
        }

        if (RemoteConfigManager.Instance != null)
        {
            RemoteConfigManager.Instance.OnInitializedEvent -= HandleRemoteConfigInitialized;
            RemoteConfigManager.Instance.OnTimeReached -= HandleTimeReached;
        }

        DisableAllAds();
    }
}
