
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private List<LevelSetup> levelPrefabs;
    [SerializeField] private Transform levelParent;

    public float checkingDuration = 5f;

    public LevelSetup currentLevelSetup;

    private float checkingTimer;
    private bool wasChecking;
    private bool isFailing;
    private bool isCheckingFinished;
    private int lastDisplayedTime = -1;

    private Coroutine checkingResultCoroutine;

    public int CurrentLevel { get; private set; }
    public LevelDifficulty CurrentDifficulty;
    public int CurrentBonusCoin;


    public bool IsFailing => isFailing;

  

    private void Update()
    {
        bool isChecking = GameManager.Instance.IsChecking;

        if (isChecking && !wasChecking)
            StartChecking();

        wasChecking = isChecking;

        if (isChecking && !isFailing && !isCheckingFinished)
            UpdateChecking();
    }

    public void CreateLevel()
    {
        StartCoroutine(CreateLevelCoroutine());
    }

    private IEnumerator CreateLevelCoroutine()
    {
        int level = LevelData.LoadLevel();

        if (level <= 0 || level > levelPrefabs.Count)
        {
            yield break;
        }

        if (levelPrefabs[level - 1] == null)
        {
            yield break;
        }

        StopChecking();

        ResetLevelInfo();

        if (currentLevelSetup != null)
        {
            Destroy(currentLevelSetup.transform.root.gameObject);
            currentLevelSetup = null;
        }

        for (int i = levelParent.childCount - 1; i >= 0; i--)
        {
            Destroy(levelParent.GetChild(i).gameObject);
        }

        yield return null;

        GameObject newLevel = Instantiate(
            levelPrefabs[level - 1].gameObject,
            levelParent
        );

        currentLevelSetup = newLevel.GetComponentInChildren<LevelSetup>(true);

        if (currentLevelSetup == null)
        {
            Debug.LogError($"Level {level} does not have LevelSetup!");
            Destroy(newLevel);
            yield break;
        }

        CurrentLevel = level;
        CurrentDifficulty = currentLevelSetup.difficulty;
        CurrentBonusCoin = currentLevelSetup.bonnusCoin;

        UiManager.Instance.ResetLevelUI();
        UiManager.Instance.Level_Text.text = "Level " + CurrentLevel;

        LevelData.SaveLevel(CurrentLevel);
    }

    private void StopChecking()
    {
        if (checkingResultCoroutine != null)
        {
            StopCoroutine(checkingResultCoroutine);
            checkingResultCoroutine = null;
        }

        checkingTimer = 0f;
        wasChecking = false;
        isFailing = false;
        isCheckingFinished = false;
        lastDisplayedTime = -1;

        UiManager.Instance.StopCheckingTimeAnimation();
        UiManager.Instance.HideCheckingTime();

        GameManager.Instance.ChangeState(GameManager.GameState.Playing);
    }
    public void ResetLevel()
    {
        Destroy(currentLevelSetup.gameObject); 
    }

    private void ResetLevelInfo()
    {
        CurrentDifficulty = LevelDifficulty.Easy;
        CurrentBonusCoin = 0;
        currentLevelSetup = null;

        checkingTimer = 0f;
        wasChecking = false;
        isFailing = false;
        isCheckingFinished = false;
        lastDisplayedTime = -1;
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
            checkingTimer = 0f;
            isCheckingFinished = true;

            checkingResultCoroutine = StartCoroutine(
                WaitAndCheckResult(0.5f)
            );
        }
    }

    private IEnumerator WaitAndCheckResult(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        checkingResultCoroutine = null;

        CheckResult();
    }

    private bool HasDogBeenHit()
    {
        if (currentLevelSetup == null)
            return false;

        Dog[] dogs = currentLevelSetup.GetComponentsInChildren<Dog>(true);

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
            StartCoroutine(ShowPopupFailDelay());

        });
    }

    private void UpdateCheckingTimeDisplay()
    {
        int displayedTime = Mathf.Max(
            0,
            Mathf.CeilToInt(checkingTimer)
        );

        if (displayedTime == lastDisplayedTime)
            return;

        lastDisplayedTime = displayedTime;

        UiManager.Instance.UpdateCheckingTime(displayedTime);
    }

    private void CheckResult()
    {
        if (currentLevelSetup == null)
            return;

        Dog[] dogs = currentLevelSetup.GetComponentsInChildren<Dog>(true);

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



    private void StartWinSequence()
    {
        UiManager.Instance.HideCheckingTime();
        GameManager.Instance.ChangeState(GameManager.GameState.Win);

        UiManager.Instance.PlayWinCheckmarkAnimation(() =>
        {
            int earnedStars = UiManager.Instance.CurrentStarCount;

            LevelData.SaveLevelStars(CurrentLevel, earnedStars);

            LevelData.SaveLevel(CurrentLevel + 1);

            //Debug.Log(
            //    $"[LevelManager] WIN LEVEL {CurrentLevel} | " +
            //    $"Earned Stars: {earnedStars}"
            //);

            StartCoroutine(ShowPopupWinDelay());
        });
    }


    private IEnumerator ShowPopupWinDelay()
    {
        yield return new WaitForSeconds(1f);

        PopupManager.Instance.ShowPopupWin();
    }
    private IEnumerator ShowPopupFailDelay()
    {
        yield return new WaitForSeconds(1f);

        PopupManager.Instance.ShowPopupFail();
    }
}
