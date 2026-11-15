
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupWin : PopupBase
{
    [Header("Stars & Reward")]
    [SerializeField] private TMP_Text bonusCoinText;

    [Header("Star Animation")]
    [SerializeField] private RectTransform star1;
    [SerializeField] private RectTransform star2;
    [SerializeField] private RectTransform star3;

    [SerializeField] private RectTransform cellStar1;
    [SerializeField] private RectTransform cellStar2;
    [SerializeField] private RectTransform cellStar3;

    [SerializeField] private float starMoveDuration = 0.4f;
    [SerializeField] private float cellScaleDuration = 0.1f;

    [Header("Fireworks")]
    [SerializeField] private RectTransform leftTrumpet;
    [SerializeField] private RectTransform rightTrumpet;

    [Header("Transition")]
    [SerializeField] private Image blackOverlay;
    [SerializeField] private float fadeDuration = 0.25f;

    private int bonusCoin;
    private Sequence starSequence;

    protected override void Start()
    {
        bonusCoin = LevelManager.Instance.CurrentBonusCoin;

        if (bonusCoinText != null)
            bonusCoinText.text = "+" + bonusCoin.ToString();

        PlayStarAnimation();
        PlayWinFireworks();
    }

    private void PlayStarAnimation()
    {
        int starCount = Mathf.Clamp(
            UiManager.Instance.CurrentStarCount,
            0,
            3
        );

        RectTransform[] stars =
        {
            star1,
            star2,
            star3
        };

        RectTransform[] cells =
        {
            cellStar1,
            cellStar2,
            cellStar3
        };

        starSequence = DOTween.Sequence();

        for (int i = 0; i < starCount; i++)
        {
            if (stars[i] == null || cells[i] == null)
                continue;

            RectTransform currentStar = stars[i];
            RectTransform currentCell = cells[i];

            Vector3 targetPosition = currentCell.position;
            Vector3 originalScale = currentCell.localScale;

            starSequence.Append(
                currentStar.DOMove(
                    targetPosition,
                    starMoveDuration
                ).SetEase(Ease.OutQuad)
            );

            starSequence.Append(
                currentCell.DOScale(
                    originalScale * 1.2f,
                    cellScaleDuration
                )
            );

            starSequence.Append(
                currentCell.DOScale(
                    originalScale,
                    cellScaleDuration
                )
            );
        }
    }

    private void PlayWinFireworks()
    {
        if (leftTrumpet == null || rightTrumpet == null)
            return;

        Vector3 leftWorldPosition = GetWorldPosition(leftTrumpet);
        Vector3 rightWorldPosition = GetWorldPosition(rightTrumpet);

        FxManager.Instance.PlayFireworks(
            CreatePoint(leftWorldPosition),
            CreatePoint(rightWorldPosition)
        );
    }

    private Vector3 GetWorldPosition(RectTransform rect)
    {
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(
            GetUICamera(),
            rect.position
        );

        screenPosition.z = Mathf.Abs(Camera.main.transform.position.z);

        return Camera.main.ScreenToWorldPoint(screenPosition);
    }

    private Camera GetUICamera()
    {
        Canvas canvas = GetComponentInParent<Canvas>();

        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            return canvas.worldCamera;

        return null;
    }

    private Transform CreatePoint(Vector3 position)
    {
        GameObject point = new GameObject("FireworkPoint");

        point.transform.position = position;
        point.transform.SetParent(transform);

        return point.transform;
    }

    public void Claim()
    {
        ScoreManager.Instance.CalculateWinScore(
            UiManager.Instance.CurrentStarCount,
            LevelManager.Instance.CurrentDifficulty
        );

        ResourceManager.Instance.AddResource(ResourceType.Coin, bonusCoin);

        NextLevel();
    }

    public void ClaimDoubleReward()
    {
        if (AdsManager.Instance == null)
            return;

        if (AdsManager.Instance.IsAdsDisabledByRemoteConfig ||
            IAPManager.Instance.IsRemoveAdsPurchased)
        {
            ReceiveDoubleReward();
            return;
        }

        AdsManager.Instance.ShowRewarded(ReceiveDoubleReward);
    }

    private void ReceiveDoubleReward()
    {
        ScoreManager.Instance.CalculateWinScore(
            UiManager.Instance.CurrentStarCount,
            LevelManager.Instance.CurrentDifficulty
        );

        int reward = bonusCoin * 2;

        ResourceManager.Instance.AddResource(ResourceType.Coin, reward);

        NextLevel();
    }

    private void NextLevel()
    {
        int currentLevel = LevelManager.Instance.CurrentLevel;
        int unlockedLevel = LevelData.LoadLevel();

        if (currentLevel == unlockedLevel)
        {
            PopupManager.Instance.ShowPopupNextGame();
            DestroyPopup();
        }
        else if (currentLevel < unlockedLevel)
        {
            blackOverlay.gameObject.SetActive(true);

            Color color = blackOverlay.color;
            color.a = 0f;
            blackOverlay.color = color;

            blackOverlay.raycastTarget = true;

            blackOverlay.DOFade(1f, fadeDuration)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    PopupManager.Instance.ShowPopupNextReplay(currentLevel + 1);
                    DestroyPopup();
                });
        }
    }

    public override void DestroyPopup()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (starSequence != null)
            starSequence.Kill();

        if (blackOverlay != null)
            DOTween.Kill(blackOverlay);
    }
}
