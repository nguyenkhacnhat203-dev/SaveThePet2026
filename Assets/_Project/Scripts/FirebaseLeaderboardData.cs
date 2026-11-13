
using System;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

[Serializable]
public class LeaderboardPlayerData
{
    public string PlayerID;
    public int Score;
    public int Star;
}

public class FirebaseLeaderboardData
{
    private DatabaseReference databaseReference;

    public FirebaseLeaderboardData()
    {
        FirebaseDatabase database = FirebaseDatabase.GetInstance(
            Firebase.FirebaseApp.DefaultInstance,
            "https://savethepet2026-default-rtdb.asia-southeast1.firebasedatabase.app"
        );

        databaseReference = database.RootReference;
    }

    public void UploadData(string playerID, int score, int star)
    {
        if (string.IsNullOrEmpty(playerID))
        {
            Debug.LogError("Player ID is empty!");
            return;
        }

        LeaderboardPlayerData playerData = new LeaderboardPlayerData
        {
            PlayerID = playerID,
            Score = score,
            Star = star
        };

        string json = JsonUtility.ToJson(playerData);

        databaseReference
            .Child("Leaderboard")
            .Child(playerID)
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Upload leaderboard failed: " + task.Exception);
                    return;
                }

                Debug.Log("Upload leaderboard successfully!");
            });
    }
}
