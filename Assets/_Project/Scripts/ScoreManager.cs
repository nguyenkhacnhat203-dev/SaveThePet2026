using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    public int Score { get; set; }

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

    public void ResetScore()
    {
        Score = 0;
        Data.SaveData(this);
    }
}