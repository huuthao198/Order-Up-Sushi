using System;
using UnityEngine;
#if USE_GOOGLEMOBILEADS
using GoogleMobileAds.Api;
#endif
public class AdsGoogleMobileManager : MonoBehaviour
{
    public static AdsGoogleMobileManager Instance;

#if USE_GOOGLEMOBILEADS
    [Header("General Settings")]
    [SerializeField] private bool isTest = true;

    [Header("Ad Unit IDs (Real)")]
    [SerializeField] private string bannerAdUnitId = "ca-app-pub-XXXXXXXXXXXXXXXX/BBBBBBBBBB";
    [SerializeField] private string interstitialAdUnitId = "ca-app-pub-XXXXXXXXXXXXXXXX/IIIIIIIIII";
    [SerializeField] private string rewardedAdUnitId = "ca-app-pub-XXXXXXXXXXXXXXXX/RRRRRRRRRR";

    [Header("Ad Unit IDs (Test)")]
    private string testBannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
    private string testInterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
    private string testRewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
    private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;
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
#if USE_GOOGLEMOBILEADS
        Debug.Log("Initializing Google Mobile Ads...");
        MobileAds.Initialize(initStatus => { });

        // chọn ID test nếu bật isTest
        if (isTest)
        {
            bannerAdUnitId = testBannerAdUnitId;
            interstitialAdUnitId = testInterstitialAdUnitId;
            rewardedAdUnitId = testRewardedAdUnitId;
            Debug.Log("Google Ads using TEST Ad Unit IDs");
        }

        LoadBanner();
        LoadInterstitial();
        LoadRewarded();
#endif
    }

    // ================= Banner =================
    private void LoadBanner()
    {
#if USE_GOOGLEMOBILEADS
        if (bannerView != null) bannerView.Destroy();
        bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
        bannerView.LoadAd(new AdRequest.Builder().Build());
#endif
    }

    public void ShowBanner()
    {
#if USE_GOOGLEMOBILEADS
        bannerView?.Show();
#endif
    }

    public void HideBanner()
    {
#if USE_GOOGLEMOBILEADS
        bannerView?.Hide();
#endif
    }

    // ================= Interstitial =================
    public void LoadInterstitial()
    {
#if USE_GOOGLEMOBILEADS
        interstitial = new InterstitialAd(interstitialAdUnitId);
        interstitial.OnAdClosed += (sender, args) => LoadInterstitial();
        interstitial.LoadAd(new AdRequest.Builder().Build());
#endif
    }

    public void ShowInterstitial()
    {
#if USE_GOOGLEMOBILEADS
        if (interstitial != null && interstitial.IsLoaded())
        {
            interstitial.Show();
        }
        else
        {
            Debug.Log("Interstitial not ready, reloading...");
            LoadInterstitial();
        }
#endif
    }

    // ================= Rewarded =================
    public void LoadRewarded()
    {
#if USE_GOOGLEMOBILEADS
        rewardedAd = new RewardedAd(rewardedAdUnitId);
        rewardedAd.OnUserEarnedReward += (sender, reward) =>
        {
            onRewardedSuccess?.Invoke();
            onRewardedSuccess = null;
            Debug.Log("Reward granted: " + reward.Type);
        };
        rewardedAd.OnAdClosed += (sender, args) => LoadRewarded();

        rewardedAd.LoadAd(new AdRequest.Builder().Build());
#endif
    }

    public void ShowRewarded(Action onSuccess)
    {
#if USE_GOOGLEMOBILEADS
        onRewardedSuccess = onSuccess;
        if (rewardedAd != null && rewardedAd.IsLoaded())
        {
            rewardedAd.Show();
        }
        else
        {
            Debug.Log("Rewarded ad not ready, reloading...");
            LoadRewarded();
        }
#endif
    }
}