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

    public static IngredientAllData LoadIngredient()
    {
        // Load file từ Resources/Levels
        TextAsset jsonFile = Resources.Load<TextAsset>($"Library/AllIngredients");
        if (jsonFile == null)
        {
            Debug.LogError($"Không tìm thấy file JSON: AllIngredients");
            return null;
        }
        // Parse JSON
        IngredientAllData data = JsonUtility.FromJson<IngredientAllData>(jsonFile.text);
        return data;
    }
}
