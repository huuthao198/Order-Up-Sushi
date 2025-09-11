using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class AllFoodData
{
    public List<FoodData> Foods;
}

[Serializable]
public class AllIngredientData
{
    public List<IngredientData> Ingredients;
}

public class LevelGeneratorWindow : EditorWindow
{
    private int startLevel = 1;
    private int endLevel = 10;
    private int baseFoodCount = 5;
    private int coinReward = 100;
    private string savePath = "Assets/Resources/Levels/";

    private AllFoodData allFoodData;
    private AllIngredientData allIngredientData;

    [MenuItem("Tools/Level Generator")]
    public static void ShowWindow()
    {
        GetWindow<LevelGeneratorWindow>("Level Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Generator", EditorStyles.boldLabel);
        startLevel = EditorGUILayout.IntField("Start Level", startLevel);
        endLevel = EditorGUILayout.IntField("End Level", endLevel);
        baseFoodCount = EditorGUILayout.IntField("Base Food per Level", baseFoodCount);
        coinReward = EditorGUILayout.IntField("Coin Reward", coinReward);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Generate Levels"))
        {
            LoadFoodAndIngredients();
            GenerateLevels();
        }
    }

    private void LoadFoodAndIngredients()
    {
        var foodText = Resources.Load<TextAsset>("Library/AllFoods");
        allFoodData = foodText != null ?
            JsonUtility.FromJson<AllFoodData>(foodText.text) :
            new AllFoodData() { Foods = new List<FoodData>() };

        var ingredientText = Resources.Load<TextAsset>("Library/AllIngredients");
        allIngredientData = ingredientText != null ?
            JsonUtility.FromJson<AllIngredientData>(ingredientText.text) :
            new AllIngredientData() { Ingredients = new List<IngredientData>() };
    }

    private void GenerateLevels()
    {
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        System.Random rand = new System.Random();

        for (int levelId = startLevel; levelId <= endLevel; levelId++)
        {
            LevelData level = new LevelData
            {
                levelId = levelId,
                isHardLevel = levelId % 5 == 0,
                orders = new List<OrderData>(),
                freeRefreshCount = (levelId % 5 == 0) ? 5 : 3,
                refreshCost = coinReward / 5 + levelId * 2
            };

            // số lượng food tăng dần theo level
            int foodCount = baseFoodCount + (levelId / 2);
            int remainingFood = foodCount;

            List<IngredientData> usedIngredients = new List<IngredientData>();

            while (remainingFood > 0)
            {
                OrderData order = new OrderData
                {
                    foods = new List<FoodData>(),
                    coinReward = coinReward + levelId * 5
                };

                int foodsInOrder = Math.Min(rand.Next(1, 4), remainingFood);
                remainingFood -= foodsInOrder;

                HashSet<int> usedFoodIds = new HashSet<int>();

                for (int i = 0; i < foodsInOrder; i++)
                {
                    var food = PickFoodByDifficulty(levelId, rand, usedFoodIds);
                    if (food != null)
                    {
                        order.foods.Add(food);
                        usedFoodIds.Add(food.id);

                        // thu thập nguyên liệu dùng trong level
                        foreach (var ing in food.Ingredients)
                        {
                            if (!usedIngredients.Any(x => x.id == ing.id))
                                usedIngredients.Add(ing);
                        }
                    }
                }

                // tính thời gian dựa trên độ khó từng món
                order.timeLimit = order.foods.Sum(f => GetTimePerFood(f));

                level.orders.Add(order);
            }

            // chỉ giữ nguyên liệu được dùng trong level
            level.availableIngredients = usedIngredients;

            // ghi ra JSON
            string json = JsonUtility.ToJson(level, true);
            string fileName = Path.Combine(savePath, $"level_{levelId}.json");
            File.WriteAllText(fileName, json);

            Debug.Log($"Level {levelId} generated: Orders={level.orders.Count}, Ingredients={level.availableIngredients.Count}");
        }

        AssetDatabase.Refresh();
        Debug.Log("Level generation complete!");
    }

    private FoodData PickFoodByDifficulty(int levelId, System.Random rand, HashSet<int> usedFoodIds)
    {
        // tỉ lệ độ khó theo level
        float diff1, diff2, diff3;
        if (levelId <= 50)
        {
            diff1 = 0.7f; diff2 = 0.2f; diff3 = 0.1f;
        }
        else
        {
            diff1 = 0.5f; diff2 = 0.3f; diff3 = 0.2f;
        }

        double r = rand.NextDouble();
        List<FoodData> candidates;

        if (r < diff1)
            candidates = allFoodData.Foods.Where(f => f.difficulty == 1 && !usedFoodIds.Contains(f.id)).ToList();
        else if (r < diff1 + diff2)
            candidates = allFoodData.Foods.Where(f => f.difficulty == 2 && !usedFoodIds.Contains(f.id)).ToList();
        else
            candidates = allFoodData.Foods.Where(f => f.difficulty == 3 && !usedFoodIds.Contains(f.id)).ToList();

        // fallback nếu rỗng
        if (candidates == null || candidates.Count == 0)
        {
            candidates = allFoodData.Foods
                .Where(f => !usedFoodIds.Contains(f.id) && f.Ingredients != null && f.Ingredients.Count > 0)
                .ToList();
        }

        if (candidates.Count == 0)
            return null;

        return candidates[rand.Next(candidates.Count)];
    }

    private float GetTimePerFood(FoodData food)
    {
        switch (food.difficulty)
        {
            case 1: return 30f;
            case 2: return 35f;
            case 3: return 40f;
            case 4: return 45f;
            default: return 30f;
        }
    }
}
