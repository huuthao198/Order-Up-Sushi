using UnityEngine;
using System;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    // adUnitId / revenueValue / networkName

#if USE_MAX
    [Header("AppLovin MAX Settings")]
    [SerializeField] private string maxSdkKey = "YOUR_MAX_SDK_KEY";
    [SerializeField] private string bannerAdUnitId = "YOUR_MAX_BANNER";
    [SerializeField] private string interstitialAdUnitId = "YOUR_MAX_INTERSTITIAL";
    [SerializeField] private string rewardedAdUnitId = "YOUR_MAX_REWARDED";
#elif USE_IRONSOURCE
    [Header("ironSource LevelPlay Settings")]
    [SerializeField] private string ironSourceAppKey = "YOUR_IRONSOURCE_APP_KEY";
#endif

    private Action onRewardedSuccess;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitAds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitAds()
    {
#if USE_MAX
        InitMax();
#elif USE_IRONSOURCE
        InitIronSource();
#endif
    }

    // ================= MAX SDK =================
#if USE_MAX
    private void InitMax()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration config) =>
        {
            Debug.Log("[MAX] SDK Initialized");

            LoadInterstitial();
            LoadRewarded();
            InitBanner();
        };

        MaxSdk.SetSdkKey(maxSdkKey);
        MaxSdk.InitializeSdk();

        RegisterMaxCallbacks();
    }

    private void RegisterMaxCallbacks()
    {
        // Banner
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += (adUnitId, adInfo) =>
        {
            //OnAdRevenuePaid?.Invoke(adUnitId, adInfo.Revenue, adInfo.NetworkName);
            Debug.Log($"[MAX] Banner Revenue: {adInfo.Revenue} from {adInfo.NetworkName}");
        };

        // Interstitial
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += (adUnitId, adInfo) =>
        {
            Debug.Log("[MAX] Interstitial Loaded");
        };
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += (adUnitId, errorInfo) =>
        {
            Debug.LogError("[MAX] Interstitial Load Failed: " + errorInfo.Message);
        };
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += (adUnitId, adInfo) =>
        {
            Debug.Log("[MAX] Interstitial Closed");
            LoadInterstitial();
        };
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += (adUnitId, adInfo) =>
        {
            //OnAdRevenuePaid?.Invoke(adUnitId, adInfo.Revenue, adInfo.NetworkName);
            Debug.Log($"[MAX] Interstitial Revenue: {adInfo.Revenue} from {adInfo.NetworkName}");
        };

        // Rewarded
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += (adUnitId, adInfo) =>
        {
            Debug.Log("[MAX] Rewarded Loaded");
        };
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += (adUnitId, errorInfo) =>
        {
            Debug.LogError("[MAX] Rewarded Load Failed: " + errorInfo.Message);
        };
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += (adUnitId, adInfo) =>
        {
            Debug.Log("[MAX] Rewarded Closed");
            LoadRewarded();
        };
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += (adUnitId, reward, adInfo) =>
        {
            Debug.Log("[MAX] Reward Granted");
            onRewardedSuccess?.Invoke();
            onRewardedSuccess = null;
        };
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += (adUnitId, adInfo) =>
        {
            //OnAdRevenuePaid?.Invoke(adUnitId, adInfo.Revenue, adInfo.NetworkName);
            Debug.Log($"[MAX] Rewarded Revenue: {adInfo.Revenue} from {adInfo.NetworkName}");
        };
    }

    private void InitBanner()
    {
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.black);
        MaxSdk.ShowBanner(bannerAdUnitId);
    }

    public void ShowBanner() => MaxSdk.ShowBanner(bannerAdUnitId);
    public void HideBanner() => MaxSdk.HideBanner(bannerAdUnitId);

    public void LoadInterstitial() => MaxSdk.LoadInterstitial(interstitialAdUnitId);

    public void ShowInterstitial()
    {
        if (MaxSdk.IsInterstitialReady(interstitialAdUnitId))
            MaxSdk.ShowInterstitial(interstitialAdUnitId);
        else
            LoadInterstitial();
    }

    public void LoadRewarded() => MaxSdk.LoadRewardedAd(rewardedAdUnitId);

    public void ShowRewarded(Action onSuccess)
    {
        onRewardedSuccess = onSuccess;
        if (MaxSdk.IsRewardedAdReady(rewardedAdUnitId))
            MaxSdk.ShowRewardedAd(rewardedAdUnitId);
        else
            LoadRewarded();
    }
#endif

    // ================= ironSource LevelPlay =================
#if USE_IRONSOURCE
    private void InitIronSource()
    {
        IronSource.Agent.init(ironSourceAppKey);
        IronSource.Agent.validateIntegration();

        RegisterIronSourceCallbacks();

        Debug.Log("[IronSource] SDK Initialized");
    }

    private void RegisterIronSourceCallbacks()
    {
        //adUnit → "BANNER", "INTERSTITIAL", "REWARDED_VIDEO"
        //revenue → doanh thu ước lượng (USD)
        //network → network nào fill quảng cáo (AdMob, UnityAds, v.v.)
        //country → quốc gia user
        // Impression revenue
        IronSourceEvents.onImpressionDataReadyEvent += (impressionData) =>
        {
            if (impressionData == null) return;

            Debug.Log($"[IronSource Revenue] AdUnit: {impressionData.adUnit} | " +
                      $"Revenue: {impressionData.revenue} | " +
                      $"Network: {impressionData.adNetwork}");

            // ví dụ: log về Firebase / Appsflyer
            // FirebaseManager.Instance?.LogEvent("ad_impression", new Dictionary<string, object>
            // {
            //     { "ad_unit", impressionData.adUnit },
            //     { "revenue", impressionData.revenue },
            //     { "network", impressionData.adNetwork }
            // });
        };

        // Interstitial
        IronSourceEvents.onInterstitialAdReadyEvent += () => Debug.Log("[IronSource] Interstitial Ready");
        IronSourceEvents.onInterstitialAdLoadFailedEvent += (err) => Debug.LogError("[IronSource] Interstitial Load Failed: " + err.getDescription());
        IronSourceEvents.onInterstitialAdClosedEvent += () => LoadInterstitial();

        // Rewarded
        IronSourceEvents.onRewardedVideoAdRewardedEvent += (placement) =>
        {
            Debug.Log("[IronSource] Reward Granted");
            onRewardedSuccess?.Invoke();
            onRewardedSuccess = null;
        };
        IronSourceEvents.onRewardedVideoAdClosedEvent += () => Debug.Log("[IronSource] Rewarded Closed");
    }

    public void ShowBanner() => IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    public void HideBanner() => IronSource.Agent.destroyBanner();

    public void LoadInterstitial() => IronSource.Agent.loadInterstitial();

    public void ShowInterstitial()
    {
        if (IronSource.Agent.isInterstitialReady())
            IronSource.Agent.showInterstitial();
        else
            LoadInterstitial();
    }

    public void ShowRewarded(Action onSuccess)
    {
        onRewardedSuccess = onSuccess;
        if (IronSource.Agent.isRewardedVideoAvailable())
            IronSource.Agent.showRewardedVideo();
    }
#endif
}