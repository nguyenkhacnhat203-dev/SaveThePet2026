using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
    [SerializeField] private DrawWall drawWall;
    [SerializeField] private BeeSpawn[] beeSpawners;

    private void Start()
    {
        GameManager.Instance.ChangeState(
            GameManager.GameState.Playing
        );

        if (beeSpawners == null || beeSpawners.Length == 0)
        {
            beeSpawners = FindObjectsOfType<BeeSpawn>();
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsPause || GameManager.Instance.IsWin || GameManager.Instance.IsLose)
            return;

        if (GameManager.Instance.IsPlaying)
        {
            UpdateDrawWall();
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsPause || GameManager.Instance.IsWin || GameManager.Instance.IsLose)
            return;

        if (GameManager.Instance.IsPlaying || GameManager.Instance.IsChecking)
        {
            foreach (var spawner in beeSpawners)
            {
                spawner.Tick(Time.fixedDeltaTime);
            }

            BeeManager.Instance.Tick(Time.fixedDeltaTime);
        }
    }

    private void UpdateDrawWall()
    {
        if (Input.GetMouseButtonDown(0))
        {
            drawWall.StartDrawing(
                Input.mousePosition
            );
        }

        if (Input.GetMouseButton(0))
        {
            drawWall.Draw(
                Input.mousePosition
            );
        }

        if (Input.GetMouseButtonUp(0))
        {
            drawWall.StopDrawing();
        }
    }

    public void OnWallCreated(GameObject wall)
    {
        GameManager.Instance.ChangeState(
            GameManager.GameState.Checking
        );

        foreach (var spawner in beeSpawners)
        {
            spawner.StartSpawn();
        }
    }

    public void CheckResult(bool win)
    {
        if (win)
        {
            GameManager.Instance.ChangeState(
                GameManager.GameState.Win
            );
        }
        else
        {
            GameManager.Instance.ChangeState(
                GameManager.GameState.Lose
            );
        }
    }

    public void PauseGame()
    {
        if (!GameManager.Instance.IsPlaying &&
            !GameManager.Instance.IsChecking)
            return;

        GameManager.Instance.ChangeState(
            GameManager.GameState.Pause
        );
    }

    public void ResumeGame()
    {
        GameManager.Instance.ChangeState(
            GameManager.GameState.Playing
        );
    }
}