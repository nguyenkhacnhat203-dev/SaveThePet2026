
using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using UnityEngine;

public class RemoteConfigManager : Singleton<RemoteConfigManager>
{
    private const string ADS_ENABLED = "ads_enabled";
    private const string FULLSCREEN_ADS_INTERVAL = "fullscreen_ads_interval";

    public float time = 120f;
    public bool adsEnabled = true;

    [SerializeField] private float currentTime;

    public bool IsInitialized { get; private set; }
    public bool IsTimeReached { get; private set; }

    public event Action OnInitializedEvent;
    public event Action OnTimeReached;

    private bool isCounting;

    private void Start()
    {
        InitializeFirebase();
    }

    private void Update()
    {
        if (!IsInitialized || !isCounting || !adsEnabled || IsTimeReached)
            return;

        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            return;

        currentTime += Time.deltaTime;

        if (currentTime >= time)
        {
            currentTime = time;
            IsTimeReached = true;
            isCounting = false;

            OnTimeReached?.Invoke();
        }
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FetchRemoteConfig();
            }
            else
            {
                Debug.LogError("Firebase dependencies error: " + task.Result);
                CompleteInitialization();
            }
        });
    }

    private void FetchRemoteConfig()
    {
        Dictionary<string, object> defaults = new Dictionary<string, object>
        {
            { ADS_ENABLED, true },
            { FULLSCREEN_ADS_INTERVAL, 120 }
        };

        FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Remote Config Set Defaults Failed");
                    CompleteInitialization();
                    return;
                }

                FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero)
                    .ContinueWithOnMainThread(fetchTask =>
                    {
                        if (fetchTask.IsFaulted)
                        {
                            Debug.LogError("Remote Config Fetch Failed");
                            CompleteInitialization();
                            return;
                        }

                        FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                            .ContinueWithOnMainThread(activateTask =>
                            {
                                if (activateTask.IsFaulted)
                                {
                                    Debug.LogError("Remote Config Activate Failed");
                                    CompleteInitialization();
                                    return;
                                }

                                LoadRemoteConfig();
                                CompleteInitialization();
                            });
                    });
            });
    }

    private void LoadRemoteConfig()
    {
        FirebaseRemoteConfig remoteConfig = FirebaseRemoteConfig.DefaultInstance;

        adsEnabled = remoteConfig.GetValue(ADS_ENABLED).BooleanValue;

        time = Mathf.Max(
            1f,
            (float)remoteConfig.GetValue(FULLSCREEN_ADS_INTERVAL).DoubleValue
        );

        currentTime = 0f;
        IsTimeReached = false;

        Debug.Log("Ads Enabled: " + adsEnabled);
        Debug.Log("Fullscreen Ads Interval: " + time);
    }

    private void CompleteInitialization()
    {
        if (IsInitialized)
            return;

        IsInitialized = true;
        OnInitializedEvent?.Invoke();
    }

    public void StartTimer()
    {
        if (!adsEnabled || isCounting || IsTimeReached)
            return;

        currentTime = 0f;
        isCounting = true;

        Debug.Log("Countdown Started: " + time);
    }

    public void RestartTimer()
    {
        if (!adsEnabled)
            return;

        currentTime = 0f;
        IsTimeReached = false;
        isCounting = true;

        Debug.Log("Countdown Restarted: " + time);
    }

    public void ResetTimer()
    {
        RestartTimer();
    }

    public void StopTimer()
    {
        isCounting = false;
    }

    private void OnDestroy()
    {
        OnInitializedEvent = null;
        OnTimeReached = null;
    }
}
