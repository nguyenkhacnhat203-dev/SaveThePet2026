
using TMPro;
using UnityEngine;

public class PopupWin : PopupBase
{
    [SerializeField] private GameObject[] stars;
    [SerializeField] private TMP_Text bonusCoinText;

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

        PopupManager.Instance.ShowPopupNextGame();

        Destroy(gameObject);
    }

    public void ClaimDoubleReward()
    {
        

  



        AdsManager.Instance.ShowRewarded(() =>
        {
            ScoreManager.Instance.CalculateWinScore(
       UiManager.Instance.CurrentStarCount,
       LevelManager.Instance.CurrentDifficulty
           );

            int reward = bonusCoin * 2;

            ResourceManager.Instance.AddResource(ResourceType.Coin, reward);
            PopupManager.Instance.ShowPopupNextGame();

            Destroy(gameObject);
        });

    }
}
