
using Firebase.Analytics;
using UnityEngine;

public class FirebaseAnalyticsData
{
    public bool IsInitialized { get; private set; }

    public FirebaseAnalyticsData()
    {
        Initialize();
    }

    private void Initialize()
    {
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        IsInitialized = true;

        Debug.Log("Firebase Analytics initialized successfully!");
    }
}
