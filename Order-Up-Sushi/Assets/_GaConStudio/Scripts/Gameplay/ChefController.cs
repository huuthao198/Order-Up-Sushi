using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ChefController : SingletonBehavior<ChefController>
{
    [SerializeField] private Transform point;
    [SerializeField] private IngredientItem ingredientPrefab;
    [SerializeField] private FoodItem foodPrefab;

    private List<IngredientItem> ingredientItem = new();
    private List<FoodItem> foodItems = new();
    private List<FoodData> foods;
    private List<FoodData> candidateFoods = new(); // danh sách món ăn khả dĩ

    internal void SetFood(List<FoodData> foodDatas)
    {
        foods = foodDatas;
    }

    internal void AddIngredient(IngredientData data, Vector3 spawnPoint)
    {
        var go = ObjectPoolManager.GetObject(ingredientPrefab.gameObject);
        CleanObj.CleanObject(go);

        go.transform.position = spawnPoint;
        go.transform.rotation = Quaternion.identity;

        var i = go.GetComponent<IngredientItem>();
        i.Init(data);

        float duration = 0.5f;
        go.transform.DOMove(point.position, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                ingredientItem.Add(i);

                go.transform.SetParent(point);

                //ObjectPoolManager.ReturnObject(go);
                // Kiểm tra xem ingredient này có hợp lệ không
                CheckProgress(data.id);
            });
    }

    private void CheckProgress(int newIngredientId)
    {
        var currentIds = ingredientItem.Select(x => x.Data.id).ToList();

        if (ingredientItem.Count == 1)
        {
            // Ingredient đầu tiên → lọc từ toàn bộ foods
            candidateFoods = foods
                .Where(f => f.Ingredients.Any(ing => ing.id == newIngredientId))
                .ToList();
        }
        else
        {
            // Các ingredient tiếp theo → lọc từ candidateFoods
            candidateFoods = candidateFoods
                .Where(f => currentIds.All(id => f.Ingredients.Any(ing => ing.id == id)))
                .ToList();
        }

        if (candidateFoods.Count == 0)
        {
            Debug.LogError("❌ Sai công thức! Đĩa hỏng.");
            CleanPlate();
            return;
        }

        // ✅ Debug tiến trình dựa trên food đầu tiên trong danh sách hợp lệ
        var candidate = candidateFoods[0];
        int total = candidate.Ingredients.Count;
        int current = ingredientItem.Count;

        IngredientSpawner.Instance.SetProcess($"{current}/{total}");
        Debug.Log($"Progress: {current}/{total} ({candidate.foodName})");

        // Nếu chỉ còn 1 ứng viên và đủ nguyên liệu thì cook
        if (candidateFoods.Count == 1)
        {
            var food = candidateFoods[0];
            var recipeIds = food.Ingredients.Select(x => x.id).OrderBy(x => x).ToList();
            var inputIds = currentIds.OrderBy(x => x).ToList();

            if (recipeIds.SequenceEqual(inputIds))
            {
                CookFood(food);
            }
        }
    }

    private void CookFood(FoodData food)
    {
        Debug.Log($"🍲 Cooked Food: {food.foodName}");

        // Clear ingredient items
        foreach (var ing in ingredientItem)
        {
            ObjectPoolManager.ReturnObject(ing.gameObject);
        }
        ingredientItem.Clear();

        // Spawn food object tại Chef
        var foodGo = ObjectPoolManager.GetObject(foodPrefab.gameObject);
        foodGo.transform.position = point.position;
        var f = foodGo.GetComponent<FoodItem>();
        f.Init(food);

        foodItems.Add(f);

        // ✅ Lấy point dish từ OrderManager
        OrderDish dish = OrderManager.Instance.GetDish();
        IngredientSpawner.Instance.SetProcess($"");

        // Tween bay tới dish
        float duration = 0.6f;
        foodGo.transform.DOMove(dish.transform.position, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // Sau khi bay tới dish có thể set parent để cố định
                foodGo.transform.SetParent(dish.transform);
                dish.AddFood(food);
                Debug.Log($"✅ {food.foodName} đã được đặt lên đĩa.");
            });

        // Reset candidateFoods
        candidateFoods.Clear();
    }

    private void CleanPlate()
    {
        foreach (var ing in ingredientItem)
        {
            ObjectPoolManager.ReturnObject(ing.gameObject);
        }
        ingredientItem.Clear();
        candidateFoods.Clear();
    }

    public void ReturnFoodToPool()
    {
        if (foodItems.Count == 0) return;
        foreach (var food in foodItems)
        {
            ObjectPoolManager.ReturnObject(food.gameObject);
        }   
        foodItems.Clear();
    }
}
