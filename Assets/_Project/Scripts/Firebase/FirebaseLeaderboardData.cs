
using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

[Serializable]
public class LeaderboardPlayerData
{
    public string PlayerID;
    public string PlayerName;
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


    public void EnsurePlayerName(string playerID, Action<string> onComplete)
    {
        if (string.IsNullOrEmpty(playerID))
        {
            Debug.LogError("Player ID is empty!");
            onComplete?.Invoke(null);
            return;
        }

        DatabaseReference namesReference = databaseReference.Child("PlayerNames");

        namesReference.RunTransaction(mutableData =>
        {
            object currentValue = mutableData.Child(playerID).Value;

            if (currentValue != null)
                return TransactionResult.Success(mutableData);

            HashSet<string> usedNames = new HashSet<string>();

            foreach (MutableData child in mutableData.Children)
            {
                if (child.Value != null)
                    usedNames.Add(child.Value.ToString());
            }

            int playerNumber = 1;
            string playerName;

            do
            {
                playerName = "Player" + playerNumber;
                playerNumber++;
            }
            while (usedNames.Contains(playerName));

            mutableData.Child(playerID).Value = playerName;

            return TransactionResult.Success(mutableData);
        })
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Assign player name failed: " + task.Exception);
                onComplete?.Invoke(null);
                return;
            }

            DataSnapshot snapshot = task.Result;

            string playerName = snapshot.Child(playerID).Value?.ToString();

            Debug.Log("Player name: " + playerName);

            onComplete?.Invoke(playerName);
        });
    }


    public void UploadData(string playerID, string playerName, int score, int star)
    {
        if (string.IsNullOrEmpty(playerID))
            return;

        LeaderboardPlayerData playerData = new LeaderboardPlayerData
        {
            PlayerID = playerID,
            PlayerName = playerName,
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



    public void ChangePlayerName(string playerID, string newName, Action<bool> onComplete)
    {
        if (string.IsNullOrEmpty(playerID) || string.IsNullOrWhiteSpace(newName))
        {
            onComplete?.Invoke(false);
            return;
        }

        newName = newName.Trim();

        DatabaseReference namesReference = databaseReference.Child("PlayerNames");

        namesReference.RunTransaction(mutableData =>
        {
            foreach (MutableData child in mutableData.Children)
            {
                if (child.Key == playerID || child.Value == null)
                    continue;

                string existingName = child.Value.ToString();

                if (string.Equals(
                    existingName,
                    newName,
                    System.StringComparison.OrdinalIgnoreCase))
                {
                    return TransactionResult.Abort();
                }
            }

            mutableData.Child(playerID).Value = newName;

            return TransactionResult.Success(mutableData);
        })
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Change player name failed: " + task.Exception);
                onComplete?.Invoke(false);
                return;
            }

            if (task.Result == null ||
                task.Result.Child(playerID).Value?.ToString() != newName)
            {
                onComplete?.Invoke(false);
                return;
            }

            onComplete?.Invoke(true);
        });
    }


    public void UploadDogTheme(string playerID, int themeIndex)
    {
        if (string.IsNullOrEmpty(playerID))
            return;

        databaseReference
            .Child("PlayerThemes")
            .Child(playerID)
            .Child("DogTheme")
            .SetValueAsync(themeIndex)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Upload Dog Theme failed: " + task.Exception);
                    return;
                }

                Debug.Log("Upload Dog Theme successfully: " + themeIndex);
            });
    }

}
