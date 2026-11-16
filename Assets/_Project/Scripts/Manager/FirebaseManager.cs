
using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;

public class FirebaseManager : Singleton<FirebaseManager>
{
    public FirebaseID FirebaseID { get; private set; }
    public FirebaseLeaderboardData LeaderboardData { get; private set; }
    public FirebaseAnalyticsData AnalyticsData { get; private set; }

    public bool IsInitialized { get; private set; }

    public string PlayerName { get; private set; }

    private readonly List<Action<string>> nameCallbacks = new List<Action<string>>();

    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
            return;

        FirebaseID = new FirebaseID();

        Debug.Log("Device ID: " + FirebaseID.DeviceID);

        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        Debug.Log("Starting Firebase initialization...");

        FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogError("Firebase initialization failed: " + task.Exception);
                    return;
                }

                if (task.Result != DependencyStatus.Available)
                {
                    Debug.LogError("Firebase dependencies unavailable: " + task.Result);
                    return;
                }

                try
                {
                    var app = FirebaseApp.DefaultInstance;
                    LeaderboardData = new FirebaseLeaderboardData();
                    AnalyticsData = new FirebaseAnalyticsData();

                    IsInitialized = true;

                    Debug.Log("<color=green>Firebase connected successfully!</color>");
                    FirebaseAnalyticManager.Instance?.UploadScore();

                    InitializePlayerName();
                }
                catch (Exception e)
                {
                    Debug.LogError("Firebase setup exception: " + e);
                }
            });
    }

    private void InitializePlayerName()
    {
        LeaderboardData.EnsurePlayerName(
            FirebaseID.DeviceID,
            playerName =>
            {
                if (string.IsNullOrEmpty(playerName))
                    return;

                PlayerName = playerName;

                Debug.Log("Current Player Name: " + PlayerName);

                for (int i = 0; i < nameCallbacks.Count; i++)
                    nameCallbacks[i]?.Invoke(PlayerName);

                nameCallbacks.Clear();

                UploadLeaderboard();
                UploadCurrentDogTheme();
            }
        );
    }

    public void GetPlayerName(Action<string> onComplete)
    {
        if (!string.IsNullOrEmpty(PlayerName))
        {
            onComplete?.Invoke(PlayerName);
            return;
        }

        nameCallbacks.Add(onComplete);
    }

    public void UploadLeaderboard()
    {
        if (!IsInitialized || string.IsNullOrEmpty(PlayerName))
            return;

        if (ScoreManager.Instance == null || ResourceManager.Instance == null)
            return;

        LeaderboardData.UploadData(
            FirebaseID.DeviceID,
            PlayerName,
            ScoreManager.Instance.Score,
            ResourceManager.Instance.Star
        );
    }


    public void ChangePlayerName(string newName, System.Action<bool> onComplete)
    {
        if (!IsInitialized || string.IsNullOrEmpty(FirebaseID.DeviceID))
        {
            onComplete?.Invoke(false);
            return;
        }

        LeaderboardData.ChangePlayerName(
            FirebaseID.DeviceID,
            newName,
            success =>
            {
                if (!success)
                {
                    onComplete?.Invoke(false);
                    return;
                }

                PlayerName = newName.Trim();

                UploadLeaderboard();
                if (ThemeManager.Instance != null)
                {
                    UploadDogTheme(
                        ThemeManager.Instance.GetSelectedTheme(ThemeCategory.Dog)
                    );
                }
                onComplete?.Invoke(true);
            }
        );
    }
    public void UploadDogTheme(int themeIndex)
    {
        if (!IsInitialized)
            return;

        LeaderboardData.UploadDogTheme(
            FirebaseID.DeviceID,
            themeIndex
        );
    }


    private void UploadCurrentDogTheme()
    {
        if (ThemeManager.Instance == null)
            return;

        int dogThemeIndex = ThemeManager.Instance.GetSelectedTheme(ThemeCategory.Dog);

        LeaderboardData.UploadDogTheme(
            FirebaseID.DeviceID,
            dogThemeIndex
        );
    }


    public void GetLeaderboard(Action<List<LeaderboardPlayerData>> onComplete)
    {
        if (!IsInitialized || LeaderboardData == null)
        {
            Debug.LogWarning("Firebase chưa khởi tạo xong!");
            onComplete?.Invoke(new List<LeaderboardPlayerData>());
            return;
        }

        LeaderboardData.GetLeaderboard(onComplete);
    }

}
