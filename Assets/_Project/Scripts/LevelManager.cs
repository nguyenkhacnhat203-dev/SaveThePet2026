using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private List<GameObject> levelPrefabs;
    [SerializeField] private Transform levelParent;
    public float checkingDuration = 5f;

    private GameObject currentLevel;

    private float checkingTimer;
    private bool wasChecking;
    private int lastDisplayedTime = -1;

    public int CurrentLevel { get; private set; }

    private void Start()
    {
        CreateLevel(LevelData.LoadLevel());
    }

    private void Update()
    {
        bool isChecking = GameManager.Instance.IsChecking;

        if (isChecking && !wasChecking)
        {
            StartChecking();
        }

        wasChecking = isChecking;

        if (isChecking)
        {
            UpdateChecking();
        }
    }

    public void CreateLevel(int level)
    {
        if (level <= 0 || level > levelPrefabs.Count)
        {
            Debug.LogError($"Level {level} does not exist!");
            return;
        }

        if (currentLevel != null)
            Destroy(currentLevel);

        currentLevel = Instantiate(levelPrefabs[level - 1], levelParent);

        CurrentLevel = level;

        LevelData.SaveLevel(CurrentLevel);
    }

    public void NextLevel()
    {
        CreateLevel(CurrentLevel + 1);
    }

    private void StartChecking()
    {
        checkingTimer = checkingDuration;
        lastDisplayedTime = -1;

        //UiManager.Instance.ShowCheckingTime();
        UpdateCheckingTimeDisplay();
    }

    private void UpdateChecking()
    {
        checkingTimer -= Time.deltaTime;

        UpdateCheckingTimeDisplay();

        if (checkingTimer <= 0f)
        {
            CheckResult();
        }
    }

    private void UpdateCheckingTimeDisplay()
    {
        int displayedTime = Mathf.CeilToInt(checkingTimer);

        if (displayedTime == lastDisplayedTime)
            return;

        lastDisplayedTime = displayedTime;

        UiManager.Instance.UpdateCheckingTime(displayedTime);
    }

    private void CheckResult()
    {
        Dog[] dogs = currentLevel.GetComponentsInChildren<Dog>(true);

        if (dogs.Length == 0)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.Lose);
            return;
        }

        foreach (Dog dog in dogs)
        {
            if (dog.CurrentState == Dog.DogState.Pain)
            {
                GameManager.Instance.ChangeState(GameManager.GameState.Lose);
                return;
            }
        }

        GameManager.Instance.ChangeState(GameManager.GameState.Win);
    }
}
