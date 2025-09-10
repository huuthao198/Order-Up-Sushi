using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Quản lý pooling cho các UI như FloatingText, HealBar.
/// Hỗ trợ nhiều loại prefab khác nhau.
/// </summary>
public class UIPoolManager : MonoBehaviour
{
    private static UIPoolManager _instance;
    public static UIPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                CreateInstance();
            }
            return _instance;
        }
    }
    private Canvas poolCanvas;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitOnLoad()
    {
        if (_instance == null)
        {
            CreateInstance();
        }
    }

    private static void CreateInstance()
    {
        // Tự động tạo nếu chưa tồn tại
        GameObject go = new GameObject("[UIPoolManager]");
        _instance = go.AddComponent<UIPoolManager>();

        // Tạo canvas gốc để chứa UI pooled object
        GameObject canvasGO = new GameObject("[UIPoolCanvas]");
        canvasGO.transform.SetParent(go.transform, false);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        var canvasScaler = canvasGO.GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f; // Cân bằng giữa width và height
        canvasGO.AddComponent<GraphicRaycaster>();

        _instance.poolCanvas = canvas;

        DontDestroyOnLoad(go); // Giữ khi load scene mới
    }

    private class Pool
    {
        public readonly List<GameObject> active = new();
        public readonly Queue<GameObject> inactive = new();
    }

    private static Dictionary<string, Pool> prefabPools = new();

    private static string GetPoolKey(GameObject prefab) => $"{prefab.name}_POOL";

    public static T GetUIObject<T>(T prefab, Transform parent) where T : Component
    {
        string key = GetPoolKey(prefab.gameObject);

        if (!prefabPools.TryGetValue(key, out var pool))
        {
            pool = new Pool();
            prefabPools[key] = pool;
        }

        GameObject obj;

        if (pool.inactive.Count > 0)
        {
            obj = pool.inactive.Dequeue();
        }
        else
        {
            obj = Instantiate(prefab.gameObject);
            obj.name = key;
        }

        pool.active.Add(obj);

        obj.transform.SetParent(parent, false);
        obj.SetActive(true);

        ResetObj(obj);

        return obj.GetComponent<T>();
    }

    public static void ReturnObject<T>(T component) where T : Component
    {
        GameObject obj = component.gameObject;
        string key = obj.name;

        if (!prefabPools.TryGetValue(key, out var pool))
        {
            Debug.LogWarning($"No pool found for {key}");
            return;
        }

        if (pool.active.Remove(obj)) // chỉ remove nếu đang active
        {
            ResetObj(obj);

            obj.SetActive(false);
            obj.transform.SetParent(Instance.poolCanvas.transform, false);
            pool.inactive.Enqueue(obj);
        }
    }

    static void ResetObj(GameObject obj)
    {
        if (obj.TryGetComponent<IPoolableObject>(out var poolable))
        {
            poolable.ResetState();
        }
    }
}
