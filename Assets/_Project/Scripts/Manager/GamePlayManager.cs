
using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
    [SerializeField] private DrawWall drawWall;
    [SerializeField] private BeeSpawn[] beeSpawners;

    private bool isInputLocked;

    private void Start()
    {
        if (beeSpawners == null || beeSpawners.Length == 0)
        {
            beeSpawners = FindObjectsOfType<BeeSpawn>();
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsPause ||
            GameManager.Instance.IsWin ||
            GameManager.Instance.IsLose ||
            Time.timeScale == 0f)
        {
            LockDrawInput();
            return;
        }

        if (isInputLocked)
        {
            if (!Input.GetMouseButton(0))
            {
                isInputLocked = false;
            }

            return;
        }

        if (GameManager.Instance.IsPlaying)
        {
            UpdateDrawWall();
        }
    }

    private void LockDrawInput()
    {
        if (!isInputLocked)
        {
            drawWall.CancelDrawing();
        }

        isInputLocked = true;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsWin || GameManager.Instance.IsLose)
        {
            BeeManager.Instance.Tick(Time.fixedUnscaledDeltaTime);
            Physics2D.Simulate(Time.fixedUnscaledDeltaTime);
            return;
        }

        if (GameManager.Instance.IsPause)
            return;

        if (GameManager.Instance.IsPlaying ||
            GameManager.Instance.IsChecking)
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
            drawWall.StartDrawing(Input.mousePosition);
            return;
        }

        if (Input.GetMouseButton(0))
        {
            drawWall.Draw(Input.mousePosition);
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
}
