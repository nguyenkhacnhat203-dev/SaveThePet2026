using UnityEngine;
using TMPro;

public class PopupWin : PopupBase
{
    [SerializeField] private GameObject[] stars;
    [SerializeField] private TMP_Text bonusCoinText;

    private int bonusCoin;
    private bool hasClaimedReward;

    protected override void Start()
    {
        UpdateStars(UiManager.Instance.CurrentStarCount);

        bonusCoin = LevelManager.Instance.CurrentBonusCoin;

        if (bonusCoinText != null)
            bonusCoinText.text = "+"+bonusCoin.ToString();
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
        if (hasClaimedReward)
            return;

        hasClaimedReward = true;

        ResourceManager.Instance.AddResource(ResourceType.Coin, bonusCoin);
    }

    public void ClaimDoubleReward()
    {
        if (hasClaimedReward)
            return;

        hasClaimedReward = true;

        int reward = bonusCoin * 2;

        ResourceManager.Instance.AddResource(ResourceType.Coin, reward);
    }
}