using System.Collections.Generic;
using UnityEngine;
#if USE_APPSFLYER
using AppsFlyerSDK;
#endif

public class AppsflyerManager : MonoBehaviour
#if USE_APPSFLYER
    , IAppsFlyerConversionData
#endif
{
    public static AppsflyerManager Instance { get; private set; }
#if USE_APPSFLYER

    [Header("AppsFlyer Settings")]
    [SerializeField] private string devKey = "YOUR_DEV_KEY";
    [SerializeField] private string appID = "YOUR_APP_ID"; // chỉ cần cho iOS
    [SerializeField] private bool isDebug = true;
#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitAppsFlyer();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitAppsFlyer()
    {
#if USE_APPSFLYER
#if UNITY_IOS
        AppsFlyer.initSDK(devKey, appID, this);
#elif UNITY_ANDROID
        AppsFlyer.initSDK(devKey, null, this); // Android không cần appID
#endif
        AppsFlyer.setIsDebug(isDebug);
        AppsFlyer.startSDK();

        Debug.Log("[AppsFlyer] SDK initialized with DevKey: " + devKey);
#else
        Debug.LogWarning("[AppsFlyer] USE_APPSFLYER not defined, SDK disabled.");
#endif
    }

    /// <summary>
    /// Send event without params
    /// </summary>
    public void LogEvent(string eventName)
    {
#if USE_APPSFLYER
        AppsFlyer.sendEvent(eventName, null);
        Debug.Log("[AppsFlyer] LogEvent: " + eventName);
#else
        Debug.LogWarning("[AppsFlyer] USE_APPSFLYER not defined, event skipped: " + eventName);
#endif
    }

    /// <summary>
    /// Send event with params
    /// </summary>
    public void LogEvent(string eventName, Dictionary<string, string> parameters)
    {
#if USE_APPSFLYER
        AppsFlyer.sendEvent(eventName, parameters);
        Debug.Log("[AppsFlyer] LogEvent: " + eventName + " with params");
#else
        Debug.LogWarning("[AppsFlyer] USE_APPSFLYER not defined, event skipped: " + eventName);
#endif
    }

#if USE_APPSFLYER
    // ================= Attribution Callbacks =================
    public void onConversionDataSuccess(string conversionData)
    {
        Debug.Log("[AppsFlyer] Conversion Data Success: " + conversionData);
    }

    public void onConversionDataFail(string error)
    {
        Debug.LogError("[AppsFlyer] Conversion Data Fail: " + error);
    }

    public void onAppOpenAttribution(string attributionData)
    {
        Debug.Log("[AppsFlyer] App Open Attribution: " + attributionData);
    }

    public void onAppOpenAttributionFailure(string error)
    {
        Debug.LogError("[AppsFlyer] App Open Attribution Fail: " + error);
    }
#endif
}