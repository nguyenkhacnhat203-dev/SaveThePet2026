using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    public void BuyNoAds()
    {
        IAPManager.Instance.BuyProduct("remove_ads");
    }
}