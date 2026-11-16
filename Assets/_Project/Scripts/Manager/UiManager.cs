
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiManager : Singleton<UiManager>
{
    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private TMP_Text checkingTimeText;
    [SerializeField] private RectTransform clockRectTransform;
    [SerializeField] private RectTransform loseX;
    [SerializeField] private RectTransform winCheckmark;

    public TMP_Text Level_Text;

    private Coroutine clockShakeCoroutine;
    private Coroutine loseXCoroutine;
    private Coroutine winCheckmarkCoroutine;

    public int CurrentStarCount { get; private set; } = 3;


    public List<GameObject> Game;
    public List<GameObject> Home;
    [SerializeField] private Image blackOverlay;


    public TMP_Text Coin;
    public TMP_Text Gem;

    private void Start()
    {
        ResetLevelUI();
        ShowHomeUi();

        UpdateResourceUI(
            ResourceManager.Instance.Coin,
            ResourceManager.Instance.Gem
        );
    }
    private void OnEnable()
    {
        GameAction.OnResourceChanged += UpdateResourceUI;
    }

    private void OnDisable()
    {
        GameAction.OnResourceChanged -= UpdateResourceUI;
    }

    private void UpdateResourceUI(int coin, int gem)
    {
        if (Coin != null)
            Coin.text = coin.ToString();

        if (Gem != null)
            Gem.text = gem.ToString();
    }

    public void RePlayLevel()
    {
        if (GameManager.Instance.IsPause || GameManager.Instance.IsWin || GameManager.Instance.IsLose)
            return;
        AudioManager.Instance.StopAllSound();

        AudioManager.Instance.PlaySound("TapButton");

        int currentLevel = LevelManager.Instance.CurrentLevel;
        int unlockedLevel = LevelData.LoadLevel();

        if (currentLevel < unlockedLevel)
        {

            GameManager.Instance.ChangeState(GameManager.GameState.Pause);

            blackOverlay.gameObject.SetActive(true);

            Color color = blackOverlay.color;
            color.a = 0f;
            blackOverlay.color = color;

            blackOverlay.raycastTarget = true;

            blackOverlay.DOFade(1f, 0.25f)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    PopupManager.Instance.ShowPopupNextReplay(currentLevel); blackOverlay.gameObject.SetActive(false);

                });
        }
        if (currentLevel == unlockedLevel)
        {

         
            PopupManager.Instance.ShowPopupNextGame();


        }

    }



    public void ShowHomeUi()
    {
        foreach (GameObject go in Game)
        {
            if (go != null)
                go.SetActive(false);
        }

        foreach (GameObject go in Home)
        {
            if (go != null)
                go.SetActive(true);
        }


    }

    public void ShowGameUi()
    {
        foreach (GameObject go in Game)
        {
            if (go != null)
                go.SetActive(true);
        }

        foreach (GameObject go in Home)
        {
            if (go != null)
                go.SetActive(false);
        }


    }











    public void ResetLevelUI()
    {
        ResetHealthBar();

        StopCheckingTimeAnimation();

        if (clockRectTransform != null)
        {
            clockRectTransform.gameObject.SetActive(true);
        }

        if (checkingTimeText != null)
        {
            checkingTimeText.text = Mathf.CeilToInt(
                LevelManager.Instance.checkingDuration
            ).ToString();
        }

        if (loseXCoroutine != null)
        {
            StopCoroutine(loseXCoroutine);
            loseXCoroutine = null;
        }

        if (loseX != null)
        {
            loseX.gameObject.SetActive(false);
            loseX.localScale = Vector3.one;
        }

        if (winCheckmarkCoroutine != null)
        {
            StopCoroutine(winCheckmarkCoroutine);
            winCheckmarkCoroutine = null;
        }

        if (winCheckmark != null)
        {
            winCheckmark.gameObject.SetActive(false);
            winCheckmark.localScale = Vector3.one;
        }
    }

    public void UpdateHealthBar(float currentLength, float maxLength)
    {
        if (healthBar == null || maxLength <= 0f)
            return;

        float progress = Mathf.Clamp01(currentLength / maxLength);
        float health = 1f - progress;

        healthBar.fillAmount = Mathf.Clamp01(health);

        UpdateStarsByProgress(progress);
    }

    private void UpdateStarsByProgress(float progress)
    {
        int visibleStarCount = 3;

        if (progress >= 0.67f)
            visibleStarCount = 1;
        else if (progress >= 0.33f)
            visibleStarCount = 2;

        UpdateStars(visibleStarCount);
    }

    public void ResetHealthBar()
    {
        if (healthBar != null)
            healthBar.fillAmount = 1f;

        UpdateStars(3);
    }

    public void UpdateStars(int starCount)
    {
        CurrentStarCount = Mathf.Clamp(starCount, 0, 3);

        if (stars == null)
            return;

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
                stars[i].SetActive(i < CurrentStarCount);
        }
    }

    public void UpdateCheckingTime(int time)
    {
        if (checkingTimeText == null)
            return;

        checkingTimeText.text = time.ToString();

        TriggerClockShake();
    }

    private void TriggerClockShake()
    {
        if (clockRectTransform == null)
            return;

        if (clockShakeCoroutine != null)
            StopCoroutine(clockShakeCoroutine);

        clockShakeCoroutine = StartCoroutine(ClockShakeRoutine());
    }

    private IEnumerator ClockShakeRoutine()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        Quaternion originalRotation = clockRectTransform.localRotation;
        float shakeAngle = 15f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float percent = elapsed / duration;
            float zRot = Mathf.Sin(elapsed * 25f) * shakeAngle * (1f - percent);

            clockRectTransform.localRotation =
                originalRotation * Quaternion.Euler(0f, 0f, zRot);

            yield return null;
        }

        clockRectTransform.localRotation = originalRotation;
        clockShakeCoroutine = null;
    }

    public void ShowCheckingTime()
    {
        if (clockRectTransform != null)
            clockRectTransform.gameObject.SetActive(true);

        if (checkingTimeText == null)
            return;

        checkingTimeText.text = Mathf.CeilToInt(
            LevelManager.Instance.checkingDuration
        ).ToString();
    }

    public void HideCheckingTime()
    {
        if (clockRectTransform != null)
            clockRectTransform.gameObject.SetActive(false);
    }

    public void PlayLoseXAnimation(Action onComplete)
    {
        HideCheckingTime();

        if (loseXCoroutine != null)
            StopCoroutine(loseXCoroutine);

        if (loseX == null)
        {
            onComplete?.Invoke();
            return;
        }

        loseX.gameObject.SetActive(true);
        loseX.localScale = Vector3.one;

        loseXCoroutine = StartCoroutine(
            LoseXAnimationRoutine(onComplete)
        );
    }

    private IEnumerator LoseXAnimationRoutine(Action onComplete)
    {
        for (int i = 0; i < 2; i++)
        {
            yield return ScaleLoseX(Vector3.one * 1.2f, 0.2f);
            yield return ScaleLoseX(Vector3.one, 0.2f);
        }

        loseX.localScale = Vector3.one;
        loseXCoroutine = null;

        onComplete?.Invoke();
    }

    private IEnumerator ScaleLoseX(Vector3 targetScale, float duration)
    {
        Vector3 startScale = loseX.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            loseX.localScale = Vector3.Lerp(
                startScale,
                targetScale,
                progress
            );

            yield return null;
        }

        loseX.localScale = targetScale;
    }

    public void PlayWinCheckmarkAnimation(Action onComplete)
    {
        HideCheckingTime();

        if (winCheckmarkCoroutine != null)
            StopCoroutine(winCheckmarkCoroutine);

        if (winCheckmark == null)
        {
            onComplete?.Invoke();
            return;
        }

        winCheckmark.gameObject.SetActive(true);
        winCheckmark.localScale = Vector3.one;

        winCheckmarkCoroutine = StartCoroutine(
            WinCheckmarkAnimationRoutine(onComplete)
        );
    }

    private IEnumerator WinCheckmarkAnimationRoutine(Action onComplete)
    {
        for (int i = 0; i < 2; i++)
        {
            yield return ScaleWinCheckmark(Vector3.one * 1.2f, 0.2f);
            yield return ScaleWinCheckmark(Vector3.one, 0.2f);
        }

        winCheckmark.localScale = Vector3.one;
        winCheckmarkCoroutine = null;

        onComplete?.Invoke();
    }

    private IEnumerator ScaleWinCheckmark(Vector3 targetScale, float duration)
    {
        Vector3 startScale = winCheckmark.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            winCheckmark.localScale = Vector3.Lerp(
                startScale,
                targetScale,
                progress
            );

            yield return null;
        }

        winCheckmark.localScale = targetScale;
    }

    public void StopCheckingTimeAnimation()
    {
        if (clockShakeCoroutine != null)
        {
            StopCoroutine(clockShakeCoroutine);
            clockShakeCoroutine = null;
        }

        if (clockRectTransform != null)
        {
            clockRectTransform.localRotation = Quaternion.identity;
        }
    }
}
