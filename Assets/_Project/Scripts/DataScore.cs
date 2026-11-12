
using UnityEngine;

[System.Serializable]
public class DataScore
{
    private const string SCORE_KEY = "Player_Score";

    private string GetLevelScoreKey(int level)
    {
        return $"LevelScore_{level}";
    }

    public void LoadData(ScoreManager manager)
    {
        manager.Score = PlayerPrefs.GetInt(SCORE_KEY, 0);
    }

    public void SaveData(ScoreManager manager)
    {
        PlayerPrefs.SetInt(SCORE_KEY, manager.Score);
        PlayerPrefs.Save();
    }

    public int LoadLevelScore(int level)
    {
        return PlayerPrefs.GetInt(GetLevelScoreKey(level), 0);
    }

    public void SaveLevelScore(int level, int score)
    {
        PlayerPrefs.SetInt(GetLevelScoreKey(level), score);
        PlayerPrefs.Save();
    }

    public void ResetLevelScores(int currentLevel)
    {
        for (int i = 1; i <= currentLevel; i++)
        {
            PlayerPrefs.DeleteKey(GetLevelScoreKey(i));
        }

        PlayerPrefs.Save();
    }
}
