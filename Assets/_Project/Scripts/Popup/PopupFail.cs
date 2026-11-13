
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupFail : PopupBase
{
    [Header("Skip Buttons")]
    [SerializeField] private GameObject skipWithAdsButton;
    [SerializeField] private GameObject skipWithoutAdsButton;

    [Header("Transition")]
    [SerializeField] private Image blackOverlay;
    [SerializeField] private float fadeDuration = 0.25f;


    protected override void Start()
    {
        UpdateSkipButtons();
    }

    private void UpdateSkipButtons()
    {
        int currentLevel = LevelManager.Instance.CurrentLevel;
        int unlockedLevel = LevelData.LoadLevel();

        if (currentLevel < unlockedLevel)
        {
            skipWithAdsButton.SetActive(false);
            skipWithoutAdsButton.SetActive(true);

        }
        if( currentLevel == unlockedLevel )
        {

            skipWithAdsButton.SetActive(true);
            skipWithoutAdsButton.SetActive(false);
        }




        //if (skipWithAdsButton != null)
        //    skipWithAdsButton.SetActive(!canSkipWithoutAds);

        //if (skipWithoutAdsButton != null)
        //    skipWithoutAdsButton.SetActive(canSkipWithoutAds);
    }

    public void SkipWithADS()
    {
        if (AdsManager.Instance == null)
            return;

        if (AdsManager.Instance.IsAdsDisabledByRemoteConfig ||
            IAPManager.Instance.IsRemoveAdsPurchased)
        {
            SkipReward();
            return;
        }

        AdsManager.Instance.ShowRewarded(SkipReward);
    }

    private void SkipReward()
    {
        LevelData.SaveLevel(LevelManager.Instance.CurrentLevel + 1);
        PopupManager.Instance.ShowPopupNextGame();
        DestroyPopup();
    }

    public void SkipWithoutADS()
    {
     

        int unlockedLevel = LevelData.LoadLevel();

        blackOverlay.gameObject.SetActive(true);

        Color color = blackOverlay.color;
        color.a = 0f;
        blackOverlay.color = color;

        blackOverlay.raycastTarget = true;

        blackOverlay.DOFade(1f, fadeDuration)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                PopupManager.Instance.ShowPopupNextReplay(unlockedLevel);
                DestroyPopup();
            });
    }

    public void Replay()
    {
        int currentLevel = LevelManager.Instance.CurrentLevel;
        int unlockedLevel = LevelData.LoadLevel();

        if (currentLevel < unlockedLevel)
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
                    PopupManager.Instance.ShowPopupNextReplay(currentLevel);
                    DestroyPopup();
                });

        }
        if (currentLevel == unlockedLevel)
        {

            PopupManager.Instance.ShowPopupNextGame();
            DestroyPopup();

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
