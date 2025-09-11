using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class ObjectPoolManager : MonoBehaviour
{
    private static Dictionary<string, Queue<GameObject>> poolDict = new();
    private static ObjectPoolManager _instance;
    private Transform poolRoot;

    public static ObjectPoolManager Instance
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
        GameObject go = new GameObject("[ObjectPoolManager]");
        GameObject root = new GameObject("[ObjectPoolRoot]");
        root.transform.SetParent(go.transform);
        root.transform.localPosition = Vector3.zero;
        root.transform.localRotation = Quaternion.identity;
        root.transform.localScale = Vector3.one;

        _instance = go.AddComponent<ObjectPoolManager>();
        _instance.poolRoot = root.transform;
        DontDestroyOnLoad(go);
    }


    /// <summary>
    /// Lấy một GameObject từ pool hoặc tạo mới nếu chưa có.
    /// </summary>
    /// <param name="prefab">Prefab GameObject</param>
    /// <param name="position">Vị trí hiển thị (world space)</param>
    /// <param name="rotation">Góc xoay</param>
    /// <returns>GameObject</returns>
    public static GameObject GetObject(GameObject prefab)
    {
        string key = GetPoolKey(prefab);

        if (!poolDict.ContainsKey(key))
        {
            poolDict[key] = new Queue<GameObject>();
        }

        GameObject obj = null;

        while (poolDict[key].Count > 0)
        {
            var candidate = poolDict[key].Dequeue();

            if (candidate == null || candidate.Equals(null))
                continue;

            obj = candidate;
            break;
        }

        if (obj != null)
        {
            obj.SetActive(true);
            ResetObj(obj);
        }
        else
        {
            obj = Instantiate(prefab);
            obj.name = key;
        }

        return obj;
    }

    public static GameObject GetObject(GameObject prefab, Transform parent)
    {
        string key = GetPoolKey(prefab);

        if (!poolDict.ContainsKey(key))
        {
            poolDict[key] = new Queue<GameObject>();
        }

        GameObject obj = null;

        while (poolDict[key].Count > 0)
        {
            var candidate = poolDict[key].Dequeue();

            if (candidate == null || candidate.Equals(null))
                continue;

            obj = candidate;
            break;
        }

        if (obj != null)
        {
            obj.transform.SetParent(parent, false);
            obj.SetActive(true);
            ResetObj(obj);
        }
        else
        {
            obj = Instantiate(prefab, parent);
            obj.name = key;
        }

        return obj;
    }


    /// <summary>
    /// Trả lại object về pool và ẩn nó đi.
    /// </summary>
    /// <param name="obj">FloatingText GameObject</param>
    public static void ReturnObject(GameObject obj)
    {
        if (obj == null) return;

        string key = obj.name;

        if (!poolDict.ContainsKey(key))
            poolDict[key] = new Queue<GameObject>();

        // Gọi reset nếu có
        ResetObj(obj);

        obj.transform.SetParent(Instance.poolRoot);
        obj.transform.localPosition = Vector3.zero; // Reset vị trí local
        obj.transform.localRotation = Quaternion.identity; // Reset góc xoay local
        obj.transform.localScale = Vector3.one; // Reset tỉ lệ local

        obj.SetActive(false);

        // Tránh enqueue trùng object nếu đang còn ở pool
        if (!poolDict[key].Contains(obj))
            poolDict[key].Enqueue(obj);
    }

    static void ResetObj(GameObject obj)
    {
        var resetObj = obj.GetComponent<IPoolableObject>();
        if (resetObj != null)
            resetObj.ResetState();
    }

    private static string GetPoolKey(GameObject prefab)
    {
        return $"{prefab.name}_POOL";
    }

    public void ReturnPoolToNextFrame(GameObject obj)
    {
        StartCoroutine(ReturnFoodNextFrame(obj));
    }

    private IEnumerator ReturnFoodNextFrame(GameObject go)
    {
        yield return null; // 1 frame
        ReturnObject(go);
    }
}
public class CleanObj
{
    // chuyển obj về scene chính dùng với Object pooling
    public static void CleanObject(GameObject obj)
    {
        // Nếu obj là prefab asset thì bỏ qua
        if (obj.scene.rootCount == 0)
        {
            Debug.LogWarning($"[CleanObject] {obj.name} là prefab asset, không thể SetParent.");
            return;
        }

        obj.transform.SetParent(null);
        SceneManager.MoveGameObjectToScene(obj, SceneManager.GetActiveScene());
    }
}