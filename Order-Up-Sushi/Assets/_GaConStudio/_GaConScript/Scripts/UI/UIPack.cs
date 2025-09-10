using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIPack : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyButton;

    private ShopPackData packData;

    public void Init(ShopPackData data)
    {
        packData = data;

        // Set UI
        if (titleText != null) titleText.text = data.titleName;
        if (priceText != null) priceText.text = data.priceText;

        // Setup button event
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnClickBuy);
    }

    private void OnClickBuy()
    {
        switch (packData.packType)
        {
            case ShopPackType.NoAds:
                Debug.Log("Mua gói No Ads");
                // TODO: gọi hàm IAP mua No Ads
                UICommon.Instance.MessageBoxShow("Purchase NoAds!");
                break;

            case ShopPackType.RestoreLifeAd:
                Debug.Log("Xem quảng cáo hồi mạng");
                // TODO: gọi AdsManager.ShowRewarded(...)
                UICommon.Instance.ShowAds(() =>
                {
                    LifeManager.Instance.RestoreLife((int)packData.value);
                });

                break;

            case ShopPackType.CoinPack:
                Debug.Log($"Mua gói coin: {packData.value}");
                // TODO: gọi IAP hoặc add coin
                UICommon.Instance.MessageBoxShow("Purchase Coin!");
                SaveManager.Instance.AddCoin(packData.value);
                break;
        }
    }
}

public enum ShopPackType
{
    NoAds,
    RestoreLifeAd,
    CoinPack
}

[System.Serializable]
public class ShopPackData
{
    public ShopPackType packType;
    public string titleName;
    public string priceText;    // VD: "49.000đ"
    public float value;    // VD: "+500 vàng"
}