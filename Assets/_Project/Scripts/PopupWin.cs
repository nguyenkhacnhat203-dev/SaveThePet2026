
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupWin : PopupBase
{
    [Header("Stars & Reward")]
    [SerializeField] private GameObject[] stars;
    [SerializeField] private TMP_Text bonusCoinText;

    [Header("Transition")]
    [SerializeField] private Image blackOverlay;
    [SerializeField] private float fadeDuration = 0.25f;

    private int bonusCoin;

    protected override void Start()
    {
        UpdateStars(UiManager.Instance.CurrentStarCount);

        bonusCoin = LevelManager.Instance.CurrentBonusCoin;

        if (bonusCoinText != null)
            bonusCoinText.text = "+" + bonusCoin.ToString();
    }

    private void UpdateStars(int starCount)
    {
        if (stars == null)
            return;

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
                stars[i].SetActive(i < starCount);
        }
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
                    PopupManager.Instance.ShowPopupNextReplay(currentLevel+1);
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
        if (blackOverlay != null)
            DOTween.Kill(blackOverlay);
    }
}
