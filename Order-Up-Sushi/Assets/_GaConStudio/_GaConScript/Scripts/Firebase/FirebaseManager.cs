using System.Collections.Generic;
using UnityEngine;
#if USE_FIREBASE
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions; // để dùng ContinueWithOnMainThread
#endif

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

#if USE_FIREBASE
    private bool isInitialized = false;
#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitFirebase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitFirebase()
    {
#if USE_FIREBASE
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                isInitialized = true;
                Debug.Log("[Firebase] Initialized successfully!");
            }
            else
            {
                Debug.LogError("[Firebase] Could not resolve dependencies: " + dependencyStatus);
            }
        });
#endif
    }

    // ================== Analytics Helpers ==================
#if USE_FIREBASE
    public void SetAnalyticsEnabled(bool enabled)
    {
        if (!isInitialized) return;
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(enabled);
    }

    public void SetUserId(string userId)
    {
        if (!isInitialized) return;
        FirebaseAnalytics.SetUserId(userId);
        Debug.Log("[Firebase] SetUserId: " + userId);
    }

    public void SetUserProperty(string propertyName, string propertyValue)
    {
        if (!isInitialized) return;
        FirebaseAnalytics.SetUserProperty(propertyName, propertyValue);
        Debug.Log($"[Firebase] SetUserProperty: {propertyName}={propertyValue}");
    }
#endif

    // ================== LogEvent ==================
    public void LogEvent(string eventName)
    {
#if USE_FIREBASE
        if (!isInitialized) return;
        FirebaseAnalytics.LogEvent(eventName);
        Debug.Log("[Firebase] LogEvent: " + eventName);
#endif
    }

    public void LogEvent(string eventName, string paramName, string value)
    {
#if USE_FIREBASE
        if (!isInitialized) return;
        FirebaseAnalytics.LogEvent(eventName, paramName, value);
        Debug.Log($"[Firebase] LogEvent: {eventName} | {paramName}={value}");
#endif
    }

    public void LogEvent(string eventName, string paramName, long value)
    {
#if USE_FIREBASE
        if (!isInitialized) return;
        FirebaseAnalytics.LogEvent(eventName, paramName, value);
        Debug.Log($"[Firebase] LogEvent: {eventName} | {paramName}={value}");
#endif
    }

    public void LogEvent(string eventName, Dictionary<string, object> parameters)
    {
#if USE_FIREBASE
        if (!isInitialized) return;

        var firebaseParams = new Parameter[parameters.Count];
        int i = 0;
        foreach (var kv in parameters)
        {
            if (kv.Value is string strVal)
                firebaseParams[i] = new Parameter(kv.Key, strVal);
            else if (kv.Value is int intVal)
                firebaseParams[i] = new Parameter(kv.Key, intVal);
            else if (kv.Value is long longVal)
                firebaseParams[i] = new Parameter(kv.Key, longVal);
            else if (kv.Value is double doubleVal)
                firebaseParams[i] = new Parameter(kv.Key, doubleVal);
            else
                firebaseParams[i] = new Parameter(kv.Key, kv.Value.ToString());
            i++;
        }

        FirebaseAnalytics.LogEvent(eventName, firebaseParams);
        Debug.Log("[Firebase] LogEvent: " + eventName + " with params");
#endif
    }
}