using UnityEngine;

public class LevelLoader
{
    public static LevelData LoadLevel(string levelName)
    {
        // Load file từ Resources/Levels
        TextAsset jsonFile = Resources.Load<TextAsset>($"Levels/{levelName}");
        if (jsonFile == null)
        {
            Debug.LogError($"Không tìm thấy file JSON: {levelName}");
            return null;
        }

        // Parse JSON
        LevelData levelData = JsonUtility.FromJson<LevelData>(jsonFile.text);
        return levelData;
    }
}
