
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    public int Score;

    public DataScore Data { get; private set; }

    private void Start()
    {
        Data = new DataScore();
        Data.LoadData(this);

        UploadScoreAnalytics();
    }

    public void AddScore(int amount)
    {
        if (amount <= 0)
            return;

        Score += amount;

        Data.SaveData(this);

        UploadLeaderboard();
        UploadScoreAnalytics();
    }

    public void CalculateWinScore(int starCount, LevelDifficulty difficulty)
    {
        if (starCount <= 0)
            return;

        int currentLevel = LevelManager.Instance.CurrentLevel;

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

        int oldScore = Data.LoadLevelScore(currentLevel);

        Score -= oldScore;
        Score = Mathf.Max(0, Score);
        Score += earnedScore;

        Data.SaveLevelScore(currentLevel, earnedScore);
        Data.SaveData(this);

        UploadLeaderboard();
        UploadScoreAnalytics();

        Debug.Log(
            $"Level {currentLevel} | Old Score: {oldScore} | New Score: {earnedScore} | Total: {Score}"
        );
    }

    public void ResetScore()
    {
        Score = 0;

        Data.ResetLevelScores(LevelData.LoadLevel());
        Data.SaveData(this);

        UploadLeaderboard();
        UploadScoreAnalytics();
    }

    private void UploadLeaderboard()
    {
        if (FirebaseManager.Instance == null)
            return;

        FirebaseManager.Instance.UploadLeaderboard();
    }

    private void UploadScoreAnalytics()
    {
        if (FirebaseAnalyticManager.Instance == null)
            return;

        FirebaseAnalyticManager.Instance.UploadScore();
    }
}
