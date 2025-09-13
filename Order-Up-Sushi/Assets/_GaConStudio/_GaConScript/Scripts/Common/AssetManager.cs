using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AssetReference
{
    public string key;
    public UnityEngine.Object asset;
}

public class AssetManager : PersistantAndSingletonBehavior<AssetManager>
{

    [Header("Food Spr")]
    [SerializeField] private List<Sprite> foodSprs;


    [Header("Custom Asset References")]
    [SerializeField] private List<AssetReference> assetReferences = new List<AssetReference>();

    private Dictionary<string, UnityEngine.Object> assetDict;

    public override void Awake()
    {
        base.Awake();
        assetDict = new Dictionary<string, UnityEngine.Object>();
        foreach (var entry in assetReferences)
        {
            if (!string.IsNullOrEmpty(entry.key) && entry.asset != null)
            {
                assetDict[entry.key] = entry.asset;
            }
        }
    }

    public Sprite GetFoodSpr(int key)
    {
        return foodSprs[key];
    }

    public T GetAsset<T>(string key) where T : UnityEngine.Object
    {
        if (assetDict.TryGetValue(key, out var obj))
        {
            if (obj is T tObj)
            {
                return tObj;
            }
            else if (obj is GameObject go && typeof(T).IsSubclassOf(typeof(Component)))
            {
                return go.GetComponent<T>();
            }
        }
        Debug.LogError($"[AssetManager] Asset with key '{key}' not found or type mismatch.");
        return null;
    }
}
