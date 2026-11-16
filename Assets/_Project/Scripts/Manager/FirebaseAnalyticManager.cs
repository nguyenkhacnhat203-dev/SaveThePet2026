
using Firebase.Analytics;
using UnityEngine;

public class FirebaseAnalyticManager : Singleton<FirebaseAnalyticManager>
{
    public void UploadScore()
    {
        if (FirebaseManager.Instance == null ||
            !FirebaseManager.Instance.IsInitialized)
            return;

        if (ScoreManager.Instance == null)
            return;

        int score = ScoreManager.Instance.Score;

        FirebaseAnalytics.LogEvent(
            "score_user",
            new Parameter("total_score", (long)score)
        );

        Debug.Log("Firebase Analytics - Upload Score: " + score);
    }
}
