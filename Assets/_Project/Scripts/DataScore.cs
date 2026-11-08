using UnityEngine;

[System.Serializable]
public class DataScore
{
    private const string SCORE_KEY = "Player_Score";

    public void LoadData(ScoreManager manager)
    {
        manager.Score = PlayerPrefs.GetInt(SCORE_KEY, 0);
    }

    public void SaveData(ScoreManager manager)
    {
        PlayerPrefs.SetInt(SCORE_KEY, manager.Score);
        PlayerPrefs.Save();
    }
}