
using TMPro;
using UnityEngine;

public class PopupAdd : PopupBase
{
    public enum AddType
    {
        Coin,
        Gem
    }

    [Header("Popup Type")]
    [SerializeField] private AddType addType;

    [Header("Objects")]
    [SerializeField] private GameObject coinObject;
    [SerializeField] private GameObject gemObject;

    [Header("Add Amount")]
    [SerializeField] private int addCoinAmount = 50;
    [SerializeField] private int addGemAmount = 10;

    [Header("Texts")]
    [SerializeField] private TMP_Text coinAmountText;
    [SerializeField] private TMP_Text gemAmountText;
    [SerializeField] private TMP_Text titleText;

    private bool isRewardPending;

    public void SetPopupType(AddType type)
    {
        addType = type;
        GameManager.Instance.ChangeState(GameManager.GameState.Pause);

        switch (addType)
        {
            case AddType.Coin:
                coinObject.SetActive(true);
                gemObject.SetActive(false);

                coinAmountText.gameObject.SetActive(true);
                gemAmountText.gameObject.SetActive(false);

                titleText.text = "Free Coin";
                coinAmountText.text = "+" + addCoinAmount;
                break;

            case AddType.Gem:
                coinObject.SetActive(false);
                gemObject.SetActive(true);

                coinAmountText.gameObject.SetActive(false);
                gemAmountText.gameObject.SetActive(true);

                titleText.text = "Free Gem";
                gemAmountText.text = "+" + addGemAmount;
                break;
        }
    }

    public void AddResource()
    {
        if (isRewardPending)
            return;

        if (AdsManager.Instance == null)
            return;

        isRewardPending = true;

        if (AdsManager.Instance.IsAdsDisabledByRemoteConfig ||
            IAPManager.Instance.IsRemoveAdsPurchased)
        {
            ReceiveReward();
            return;
        }

        AdsManager.Instance.ShowRewarded(ReceiveReward);
    }

    private void ReceiveReward()
    {
        switch (addType)
        {
            case AddType.Coin:
                FxManager.Instance.PlayCoin(
       transform.position,
       FxManager.Instance.CoinTarget,
       UiManager.Instance.Coin.gameObject,
       10,
       addCoinAmount
   );
                break;

            case AddType.Gem:
                FxManager.Instance.PlayGem(
         transform.position,
         FxManager.Instance.GemTarget,
         UiManager.Instance.Gem.gameObject,
         10,
         addGemAmount
     );
                break;
        }
        DestroyPopup();

    }
    public override void DestroyPopup()
    {
        GameManager.Instance.ChangeState(GameManager.GameState.Playing);

        Destroy(gameObject);
    }
}
