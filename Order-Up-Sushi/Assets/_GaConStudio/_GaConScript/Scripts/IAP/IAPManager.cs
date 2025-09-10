using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

[Serializable]
public class CoinProduct
{
    public string productIdAndroid;
    public string productIdIOS;
    public int value;

    public string GetProductIdForCurrentPlatform()
    {
        if (Application.platform == RuntimePlatform.Android)
            return productIdAndroid;
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            return productIdIOS;
        return null;
    }
}

public class IAPManager : MonoBehaviour
{
    private StoreController m_StoreController;

    [Header("Consumable coin/pack products")]
    [SerializeField] private List<CoinProduct> consumableProducts;

    [Header("Non-consumable products (e.g., NoAds)")]
    [SerializeField] private List<string> nonConsumableProductIds;

    private Dictionary<string, int> coinProductMap;

    void Awake()
    {
        // Khởi tạo map productId -> value cho nền tảng hiện tại
        coinProductMap = consumableProducts
            .Select(p => new { id = p.GetProductIdForCurrentPlatform(), value = p.value })
            .Where(x => !string.IsNullOrEmpty(x.id))
            .ToDictionary(x => x.id, x => x.value);

        InitializeIAP();
    }

    async void InitializeIAP()
    {
        m_StoreController = UnityIAPServices.StoreController();

        m_StoreController.OnPurchasePending += OnPurchasePending;
        m_StoreController.OnPurchaseConfirmed += OnPurchaseConfirmed;
        m_StoreController.OnPurchaseFailed += OnPurchaseFailed;
        m_StoreController.OnStoreDisconnected += OnStoreDisconnected;

        Debug.Log("Connecting to store...");
        await m_StoreController.Connect();

        m_StoreController.OnProductsFetched += OnProductsFetched;
        m_StoreController.OnProductsFetchFailed += OnProductsFetchedFailed;

        FetchProducts();
    }

    void FetchProducts()
    {
        var productDefs = new List<ProductDefinition>();

        // Consumable
        foreach (var p in consumableProducts)
        {
            string id = p.GetProductIdForCurrentPlatform();
            if (!string.IsNullOrEmpty(id))
                productDefs.Add(new ProductDefinition(id, ProductType.Consumable));
        }

        // Non-consumable
        foreach (var id in nonConsumableProductIds)
            productDefs.Add(new ProductDefinition(id, ProductType.NonConsumable));

        m_StoreController.FetchProducts(productDefs);
    }

    public void BuyProduct(string productId)
    {
        if (string.IsNullOrEmpty(productId))
        {
            Debug.LogWarning("BuyProduct called with empty ID.");
            return;
        }

        Debug.Log($"Attempting purchase: {productId}");
        m_StoreController.PurchaseProduct(productId);
    }

    void OnPurchasePending(PendingOrder order)
    {
        var product = GetFirstProductInOrder(order);
        if (product == null)
        {
            Debug.LogError("Pending order has no product!");
            return;
        }

        Debug.Log($"Purchase pending: {product.definition.id}");

        GrantProduct(product.definition.id);

        m_StoreController.ConfirmPurchase(order);
    }

    void OnPurchaseConfirmed(Order order)
    {
        var product = GetFirstProductInOrder(order);
        Debug.Log($"Purchase confirmed: {product?.definition.id}");
    }

    void OnPurchaseFailed(FailedOrder order)
    {
        var product = GetFirstProductInOrder(order);
        Debug.LogError($"Purchase failed - Product: {product?.definition.id}, Reason: {order.FailureReason}");
    }

    void OnStoreDisconnected(StoreConnectionFailureDescription desc)
    {
        Debug.LogError($"Store disconnected: {desc.message}");
    }

    void OnProductsFetched(List<Product> products)
    {
        Debug.Log($"Products fetched: {products.Count}");
        foreach (var p in products)
            Debug.Log($"- {p.definition.id} | {p.metadata.localizedTitle} | {p.metadata.localizedPriceString}");
    }

    void OnProductsFetchedFailed(ProductFetchFailed failure)
    {
        Debug.LogError($"Products fetch failed: {failure.FailureReason}");
    }

    Product GetFirstProductInOrder(Order order)
    {
        return order.CartOrdered.Items().FirstOrDefault()?.Product;
    }

    /// <summary>
    /// Grant coin hoặc unlock non-consumable
    /// </summary>
    void GrantProduct(string productId)
    {
        // Consumable
        if (coinProductMap.TryGetValue(productId, out int value))
        {
            SaveManager.Instance.AddCoin(value);
            Debug.Log($"Granted {value} coins for {productId}");
        }
        // Non-consumable
        else if (nonConsumableProductIds.Contains(productId))
        {
            PlayerPrefs.SetInt(productId, 1);
            Debug.Log($"{productId} unlocked!");
        }
        else
        {
            Debug.LogWarning($"Unknown productId: {productId}");
        }
    }

    /// <summary>
    /// Restore non-consumable purchases (iOS only) với callback
    /// </summary>
    /// <param name="callback">Action<bool, string>: bool = success, string = message</param>
    public void RestoreTransactions(Action<bool, string> callback)
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            callback?.Invoke(false, "RestoreTransactions chỉ khả dụng trên iOS.");
            return;
        }

        Debug.Log("Restoring transactions...");

        m_StoreController.RestoreTransactions((success, message) =>
        {
            if (success)
            {
                Debug.Log("RestoreTransactions thành công.");
                // Grant tất cả non-consumable đã mua
                foreach (var id in nonConsumableProductIds)
                {
                    PlayerPrefs.SetInt(id, 1);
                    Debug.Log($"{id} restored/unlocked!");
                }
            }
            else
            {
                Debug.LogError($"RestoreTransactions thất bại: {message}");
            }

            callback?.Invoke(success, message);
        });
    }
}
