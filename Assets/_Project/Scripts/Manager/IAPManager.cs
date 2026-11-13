
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : Singleton<IAPManager>
{
    private const string REMOVE_ADS_KEY = "IAP_RemoveAds";

    private StoreController storeController;

    private readonly Dictionary<string, ProductType> products = new()
    {
        { "gem_1000", ProductType.Consumable },
        { "gem_5000", ProductType.Consumable },
        { "coin_5000", ProductType.Consumable },
        { "remove_ads", ProductType.NonConsumable }
    };

    public bool IsInitialized { get; private set; }

    public bool IsRemoveAdsPurchased =>
        PlayerPrefs.GetInt(REMOVE_ADS_KEY, 0) == 1;

    public event Action<string> OnPurchaseSuccess;
    public event Action<string> OnPurchaseFailed;

    protected override async void Awake()
    {
        base.Awake();

        if (Instance != this)
            return;

        await InitializeIAP();
    }

    private async Task InitializeIAP()
    {
        try
        {
            storeController = UnityIAPServices.StoreController();

            storeController.OnStoreDisconnected += OnStoreDisconnected;
            storeController.OnProductsFetched += OnProductsFetched;
            storeController.OnProductsFetchFailed += OnProductsFetchFailed;
            storeController.OnPurchasesFetched += OnPurchasesFetched;
            storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
            storeController.OnPurchasePending += OnPurchasePending;
            storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
            storeController.OnPurchaseFailed += HandlePurchaseFailed;

            await storeController.Connect();

            List<ProductDefinition> productDefinitions = new();

            foreach (var product in products)
            {
                productDefinitions.Add(
                    new ProductDefinition(product.Key, product.Value)
                );
            }

            storeController.FetchProducts(productDefinitions);
        }
        catch (Exception e)
        {
            Debug.LogError("[IAP] Initialize Error: " + e.Message);
        }
    }

    private void OnProductsFetched(List<Product> productList)
    {
        Debug.Log("[IAP] Products Fetched: " + productList.Count);

        storeController.FetchPurchases();
    }

    private void OnProductsFetchFailed(ProductFetchFailed failure)
    {
        Debug.LogError("[IAP] Fetch Products Failed: " + failure.FailureReason);
    }

    private void OnPurchasesFetched(Orders orders)
    {
        IsInitialized = true;

        Debug.Log("[IAP] Initialized");

        foreach (ConfirmedOrder order in orders.ConfirmedOrders)
        {
            CheckRestoredOrder(order);
        }
    }

    private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failure)
    {
        Debug.LogError("[IAP] Fetch Purchases Failed: " + failure.Message);
    }

    private void OnStoreDisconnected(StoreConnectionFailureDescription failure)
    {
        IsInitialized = false;

        Debug.LogError("[IAP] Disconnected: " + failure.Message);
    }

    public void BuyProduct(string productId)
    {
        if (!IsInitialized)
        {
            Debug.LogWarning("[IAP] Not Initialized");
            return;
        }

        if (!products.ContainsKey(productId))
        {
            Debug.LogError("[IAP] Product Not Found: " + productId);
            return;
        }

        Product product = storeController.GetProductById(productId);

        if (product == null || !product.availableToPurchase)
        {
            Debug.LogWarning("[IAP] Product Unavailable: " + productId);
            return;
        }

        storeController.PurchaseProduct(productId);
    }

    private void OnPurchasePending(PendingOrder order)
    {
        foreach (var item in order.CartOrdered.Items())
        {
            string productId = item.Product.definition.id;

            if (!products.ContainsKey(productId))
                continue;

            GrantProduct(productId);
        }

        storeController.ConfirmPurchase(order);
    }

    private void OnPurchaseConfirmed(Order order)
    {
        Debug.Log("[IAP] Purchase Confirmed");
    }

    private void HandlePurchaseFailed(FailedOrder order)
    {
        string productId = "";

        foreach (var item in order.CartOrdered.Items())
        {
            productId = item.Product.definition.id;
            break;
        }

        Debug.LogError("[IAP] Purchase Failed: " + productId);

        OnPurchaseFailed?.Invoke(productId);
    }

    private void GrantProduct(string productId)
    {
        switch (productId)
        {
            case "gem_1000":
                ResourceManager.Instance.Gem += 1000;
                break;

            case "gem_5000":
                ResourceManager.Instance.Gem += 5000;
                break;

            case "coin_5000":
                ResourceManager.Instance.Coin += 5000;
                break;

            case "remove_ads":
                SetRemoveAdsPurchased();
                break;

            default:
                Debug.LogWarning("[IAP] Unknown Product: " + productId);
                return;
        }

        Debug.Log("[IAP] Purchase Success: " + productId);

        OnPurchaseSuccess?.Invoke(productId);
    }

    private void CheckRestoredOrder(ConfirmedOrder order)
    {
        foreach (var item in order.CartOrdered.Items())
        {
            string productId = item.Product.definition.id;

            if (productId == "remove_ads")
            {
                SetRemoveAdsPurchased();
            }
        }
    }

    private void SetRemoveAdsPurchased()
    {
        PlayerPrefs.SetInt(REMOVE_ADS_KEY, 1);
        PlayerPrefs.Save();
    }

    public bool HasPurchasedRemoveAds()
    {
        return IsRemoveAdsPurchased;
    }

    protected virtual void OnDisable()
    {
        if (storeController == null)
            return;

        storeController.OnStoreDisconnected -= OnStoreDisconnected;
        storeController.OnProductsFetched -= OnProductsFetched;
        storeController.OnProductsFetchFailed -= OnProductsFetchFailed;
        storeController.OnPurchasesFetched -= OnPurchasesFetched;
        storeController.OnPurchasesFetchFailed -= OnPurchasesFetchFailed;
        storeController.OnPurchasePending -= OnPurchasePending;
        storeController.OnPurchaseConfirmed -= OnPurchaseConfirmed;
        storeController.OnPurchaseFailed -= HandlePurchaseFailed;
    }
}
