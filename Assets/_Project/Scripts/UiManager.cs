using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UiManager : Singleton<UiManager>
{
    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private TMP_Text checkingTimeText;

    [Header("Clock Shake Effect")]
    [SerializeField] private RectTransform clockRectTransform; // Kéo GameObject chứa đồng hồ vào đây trên Inspector

    private Coroutine clockShakeCoroutine;

    private void Start()
    {
        ShowCheckingTime();
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
        if (stars == null || stars.Length == 0)
            return;

        int visibleStarCount = 3;

        if (progress >= 1f)
        {
            visibleStarCount = 0;
        }
        else if (progress >= 0.67f)
        {
            visibleStarCount = 1;
        }
        else if (progress >= 0.33f)
        {
            visibleStarCount = 2;
        }

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
        if (stars == null)
            return;

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
            {
                stars[i].SetActive(i < starCount);
            }
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
        {
            StopCoroutine(clockShakeCoroutine);
        }

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

            clockRectTransform.localRotation = originalRotation * Quaternion.Euler(0f, 0f, zRot);
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

        checkingTimeText.text = LevelManager.Instance.checkingDuration.ToString();
    }

    public void HideCheckingTime()
    {
        if (clockRectTransform != null)
            clockRectTransform.gameObject.SetActive(false);
    }
}