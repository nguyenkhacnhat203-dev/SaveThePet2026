using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private List<LevelSetup> levelPrefabs;
    [SerializeField] private Transform levelParent;

    public float checkingDuration = 5f;

    private GameObject currentLevel;
    private LevelSetup currentLevelSetup;

    private float checkingTimer;
    private bool wasChecking;
    private bool isFailing;
    private bool isCheckingFinished;
    private int lastDisplayedTime = -1;

    public int CurrentLevel { get; private set; }
    public LevelDifficulty CurrentDifficulty;
    public int CurrentBonusCoin;
    private bool hasRecordedWinRewards;
    public bool IsFailing => isFailing;

    private void Start()
    {
        CreateLevel(LevelData.LoadLevel());
    }

    private void Update()
    {
        bool isChecking = GameManager.Instance.IsChecking;

        if (isChecking && !wasChecking)
            StartChecking();

        wasChecking = isChecking;

        if (isChecking && !isFailing && !isCheckingFinished)
            UpdateChecking();
    }

    public void CreateLevel(int level)
    {
        if (level <= 0 || level > levelPrefabs.Count)
        {
            Debug.LogError($"Level {level} does not exist!");
            return;
        }

        if (levelPrefabs[level - 1] == null)
        {
            Debug.LogError($"Level {level} prefab is missing!");
            return;
        }

        ResetLevelInfo();

        if (currentLevel != null)
            Destroy(currentLevel);

        currentLevel = Instantiate(levelPrefabs[level - 1].gameObject, levelParent);

        currentLevelSetup = currentLevel.GetComponentInChildren<LevelSetup>(true);

        if (currentLevelSetup == null)
        {
            Debug.LogError($"Level {level} does not have LevelSetup!");
            return;
        }

        CurrentLevel = level;

        CurrentDifficulty = currentLevelSetup.difficulty;
        CurrentBonusCoin = currentLevelSetup.bonnusCoin;

        UiManager.Instance.ResetHealthBar();
        UiManager.Instance.Level_Text.text = "Level " + CurrentLevel.ToString();

        LevelData.SaveLevel(CurrentLevel);
    }

    private void ResetLevelInfo()
    {
        CurrentDifficulty = LevelDifficulty.Easy;
        CurrentBonusCoin = 0;
        hasRecordedWinRewards = false;
        currentLevelSetup = null;

        checkingTimer = 0f;
        wasChecking = false;
        isFailing = false;
        isCheckingFinished = false;
        lastDisplayedTime = -1;
    }

    public void NextLevel()
    {
        CreateLevel(CurrentLevel + 1);
    }

    private void StartChecking()
    {
        isFailing = false;
        isCheckingFinished = false;
        checkingTimer = checkingDuration;
        lastDisplayedTime = -1;

        UiManager.Instance.ShowCheckingTime();
        UpdateCheckingTimeDisplay();
    }

    private void UpdateChecking()
    {
        if (HasDogBeenHit())
        {
            StartFailSequence();
            return;
        }

        checkingTimer -= Time.deltaTime;

        UpdateCheckingTimeDisplay();

        if (checkingTimer <= 0f)
        {
            isCheckingFinished = true;

            StartCoroutine(WaitAndCheckResult(0.5f));
        }
    }

    private IEnumerator WaitAndCheckResult(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        CheckResult();
    }

    private bool HasDogBeenHit()
    {
        if (currentLevel == null)
            return false;

        Dog[] dogs = currentLevel.GetComponentsInChildren<Dog>(true);

        foreach (Dog dog in dogs)
        {
            if (dog != null && dog.CurrentState == Dog.DogState.Pain)
                return true;
        }

        return false;
    }

    private void StartFailSequence()
    {
        if (isFailing)
            return;

        isFailing = true;

        UiManager.Instance.HideCheckingTime();

        UiManager.Instance.PlayLoseXAnimation(() =>
        {
            GameManager.Instance.ChangeState(GameManager.GameState.Lose);
        });
    }

    private void UpdateCheckingTimeDisplay()
    {
        int displayedTime = Mathf.Max(0, Mathf.CeilToInt(checkingTimer));

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
                StartFailSequence();
                return;
            }
        }

        foreach (Dog dog in dogs)
        {
            dog.StartSatisfied();
        }

        StartWinSequence();
    }

    private void RecordWinRewards()
    {
        if (hasRecordedWinRewards)
            return;

        hasRecordedWinRewards = true;

        int starCount = UiManager.Instance.CurrentStarCount;

        ResourceManager.Instance.AddResource(ResourceType.Star, starCount);

        int difficultyMultiplier = 1;

        switch (CurrentDifficulty)
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

        ScoreManager.Instance.AddScore(earnedScore);
    }
    private void StartWinSequence()
    {
        UiManager.Instance.HideCheckingTime();

        UiManager.Instance.PlayWinCheckmarkAnimation(() =>
        {
            GameManager.Instance.ChangeState(GameManager.GameState.Win);

            RecordWinRewards();

            LevelData.SaveLevel(CurrentLevel + 1);

            StartCoroutine(ShowPopupWinDelay());
        });
    }

    private IEnumerator ShowPopupWinDelay()
    {
        yield return new WaitForSeconds(1.25f);

        PopupManager.Instance.ShowPopupWin();
    }
}