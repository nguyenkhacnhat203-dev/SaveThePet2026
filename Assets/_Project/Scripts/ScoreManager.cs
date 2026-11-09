
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    public int Score;

    public DataScore Data { get; private set; }

    private void Start()
    {
        Data = new DataScore();
        Data.LoadData(this);
    }

    public void AddScore(int amount)
    {
        if (amount <= 0)
            return;

        Score += amount;
        Data.SaveData(this);
    }

    public void CalculateWinScore(int starCount, LevelDifficulty difficulty)
    {
        if (starCount <= 0)
            return;

        int difficultyMultiplier = 1;

        switch (difficulty)
        {
            case LevelDifficulty.Easy:
                difficultyMultiplier = 1;
                break;

            case LevelDifficulty.Medium:
                difficultyMultiplier = 2;
                break;

            case LevelDifficulty.Hard:
                difficultyMultiplier = 3;
                break;
        }

        int earnedScore = starCount * difficultyMultiplier;

        AddScore(earnedScore);
    }

    public void ResetScore()
    {
        Score = 0;
        Data.SaveData(this);
    }
}
