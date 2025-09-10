using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableAssetCache : PersistantAndSingletonBehavior<AddressableAssetCache>
{
    private Dictionary<int, LevelData> levelCache = new Dictionary<int, LevelData>();

    /// <summary>
    /// Load tất cả level với batch size và báo progress mỗi batch
    /// </summary>
    public IEnumerator LoadAllLevelsWithProgress(Action<float> onProgress, Action onComplete = null, int batchSize = 20)
    {
        // Lấy tất cả asset locations theo Label "Level"
        //var locationsHandle = Addressables.LoadResourceLocationsAsync("Level");
        //yield return locationsHandle;
        //var locations = locationsHandle.Result;

        //int total = locations.Count;
        //for (int i = 0; i < total; i += batchSize)
        //{
        //    int currentBatch = Mathf.Min(batchSize, total - i);
        //    List<AsyncOperationHandle<TextAsset>> batchHandles = new List<AsyncOperationHandle<TextAsset>>();

        //    // Start load batch
        //    for (int j = 0; j < currentBatch; j++)
        //    {
        //        var handle = Addressables.LoadAssetAsync<TextAsset>(locations[i + j]);
        //        batchHandles.Add(handle);
        //    }

        //    // Wait all in batch
        //    foreach (var h in batchHandles)
        //        yield return h;

        //    // Parse + cache
        //    for (int j = 0; j < currentBatch; j++)
        //    {
        //        var handle = batchHandles[j];
        //        if (handle.Status == AsyncOperationStatus.Succeeded)
        //        {
        //            LevelData data = JsonUtility.FromJson<LevelData>(handle.Result.text);
        //            if (data != null && !levelCache.ContainsKey(data.levelId) && LevelLoader.IsLevelValid(data))
        //                levelCache.Add(data.levelId, data);
        //        }
        //        elsef
        //        {
        //            Debug.LogWarning($"Failed to load level at {locations[i + j].PrimaryKey}");
        //        }
        //    }

        //    // Update progress sau batch
        //    float progress = Mathf.Min(i + currentBatch, total) / (float)total;
        //    onProgress?.Invoke(progress);

        //    // Giữ frame render mượt
        //    yield return null;
        //}

        yield return null;
        onComplete?.Invoke();
    }

    public LevelData GetLevel(int levelId)
    {
        if (levelCache.TryGetValue(levelId, out LevelData data))
            return data;
        Debug.LogWarning($"Level {levelId} not found in cache!");
        return null;
    }

    public void ClearCache()
    {
        levelCache.Clear();
        Addressables.ClearResourceLocators();
    }
}
