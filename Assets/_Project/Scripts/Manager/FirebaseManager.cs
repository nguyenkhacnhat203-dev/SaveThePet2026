
using UnityEngine;
using Firebase;
using Firebase.Extensions;

public class FirebaseManager : Singleton<FirebaseManager>
{
    public FirebaseID FirebaseID { get; private set; }
    public FirebaseLeaderboardData LeaderboardData { get; private set; }

    public bool IsInitialized { get; private set; }

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
                if (task.IsCanceled)
                {
                    Debug.LogError("Firebase initialization was canceled!");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("Firebase initialization exception: " + task.Exception);
                    return;
                }

                Debug.Log("Firebase Dependency Status: " + task.Result);

                if (task.Result != DependencyStatus.Available)
                {
                    Debug.LogError("Firebase dependencies unavailable: " + task.Result);
                    return;
                }

                try
                {
                    LeaderboardData = new FirebaseLeaderboardData();

                    IsInitialized = true;

                    Debug.Log("<color=green>Firebase connected successfully!</color>");

                    UploadLeaderboard();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Firebase setup exception: " + e);
                }
            });
    }


    public void UploadLeaderboard()
    {
        if (!IsInitialized)
        {
            Debug.Log("Firebase is not initialized yet!");
            return;
        }

        if (ScoreManager.Instance == null || ResourceManager.Instance == null)
        {
            Debug.LogWarning("ScoreManager or ResourceManager is not ready!");
            return;
        }

        LeaderboardData.UploadData(
            FirebaseID.DeviceID,
            ScoreManager.Instance.Score,
            ResourceManager.Instance.Star
        );
    }
}
