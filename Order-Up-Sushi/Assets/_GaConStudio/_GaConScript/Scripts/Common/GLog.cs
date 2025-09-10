using UnityEngine;

public class GLog : SingletonBehavior<GLog>
{
    [Header("Bật tắt log toàn cục")]
    public bool IsDebug = true;

    public static void Log(string msg)
    {
        if (!HasInstance()) return;

        if (Instance.IsDebug)
            Debug.Log(msg);
    }

    public static void Log(string msg, bool show)
    {
        if (!HasInstance()) return;

        if (show)
            Debug.Log(msg);
    }

    public static void LogWarning(string msg)
    {
        if (!HasInstance()) return;

        if (Instance.IsDebug)
            Debug.LogWarning(msg);
    }

    public static void LogError(string msg)
    {
        if (!HasInstance()) return;

        if (Instance.IsDebug)
            Debug.LogError(msg);
    }

}