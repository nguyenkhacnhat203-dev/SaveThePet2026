using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        Playing,
        Checking,
        Win,
        Lose,
        Pause
    }

    [SerializeField] private GameState currentState;

    public GameState CurrentState => currentState;

    public bool IsPlaying => currentState == GameState.Playing;
    public bool IsChecking => currentState == GameState.Checking;
    public bool IsWin => currentState == GameState.Win;
    public bool IsLose => currentState == GameState.Lose;
    public bool IsPause => currentState == GameState.Pause;

    private void Start()
    {
        ChangeState(GameState.Playing);
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        Time.timeScale = (currentState == GameState.Pause) ? 0f : 1f;
    }
}