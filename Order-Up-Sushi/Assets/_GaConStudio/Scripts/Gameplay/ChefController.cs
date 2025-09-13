using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ChefController : SingletonBehavior<ChefController>
{
    [SerializeField] private TextMeshProUGUI processText;
    [SerializeField] private Transform plateTrs;
    [SerializeField] private Transform plateParent;
    [SerializeField] private IngredientItem ingredientPrefab;
    [SerializeField] private FoodItem foodPrefab;

    private List<IngredientItem> ingredientItem = new();
    private List<FoodItem> foodItems = new();
    private List<FoodData> foods;
    private List<FoodData> candidateFoods = new(); // danh sách món ăn khả dĩ

    public void InitChef()
    {
        plateTrs.gameObject.SetActive(true);
    }

    internal void SetFood(List<FoodData> foodDatas)
    {
        foods = foodDatas;
    }

    internal void AddIngredient(IngredientData data, Vector3 spawnPoint)
    {
        var go = UIPoolManager.GetUIObject(ingredientPrefab, plateParent);
        go.transform.position = spawnPoint;
        go.transform.rotation = Quaternion.identity;
        go.Init(data);

        MoveTo(go.transform, plateTrs.position, .5f, () =>
        {
            ingredientItem.Add(go);
            CheckProgress(data.id);
        });
    }

    private void CheckProgress(int newIngredientId)
    {
        // Thêm nguyên liệu mới vào danh sách
        var currentIds = ingredientItem.Select(x => x.Data.id).ToList();

        // Nếu ingredient đầu tiên, lọc từ toàn bộ foods
        if (ingredientItem.Count == 1)
        {
            candidateFoods = foods
                .Where(f => f.Ingredients.Any(ing => ing.id == newIngredientId))
                .ToList();
        }
        else
        {
            // Các ingredient tiếp theo → lọc theo số lượng nguyên liệu
            candidateFoods = candidateFoods
                .Where(f =>
                {
                    var recipeIds = f.Ingredients.Select(x => x.id).ToList();
                    foreach (var id in currentIds)
                    {
                        if (!recipeIds.Contains(id))
                            return false;
                        recipeIds.Remove(id); // loại bỏ để tính trùng
                    }
                    return true;
                })
                .ToList();
        }

        // Nếu không còn ứng viên nào → hỏng đĩa
        if (candidateFoods.Count == 0)
        {
            Debug.LogError("❌ Sai công thức! Đĩa hỏng.");
            CleanPlate();
            return;
        }

        // Debug tiến trình dựa trên food có số nguyên liệu lớn nhất còn match
        var candidate = candidateFoods.OrderByDescending(f => f.Ingredients.Count).First();
        int total = candidate.Ingredients.Count;
        int current = currentIds.Count;
        processText.text = $"{current}/{total}";
        Debug.Log($"Progress: {current}/{total} ({candidate.foodName})");

        // Cook các món đã đủ nguyên liệu
        for (int i = candidateFoods.Count - 1; i >= 0; i--)
        {
            var food = candidateFoods[i];
            var recipeIds = food.Ingredients.Select(x => x.id).OrderBy(x => x).ToList();
            var inputIds = currentIds.OrderBy(x => x).ToList();

            if (recipeIds.SequenceEqual(inputIds))
            {
                CookFood(food);
                candidateFoods.RemoveAt(i); // loại bỏ món đã cook
                break; // cook từng món 1 lần
            }
        }
    }

    private void CookFood(FoodData food)
    {
        Debug.Log($"🍲 Cooked Food: {food.foodName}");

        // Clear ingredient items
        foreach (var ing in ingredientItem)
            UIPoolManager.ReturnObject(ing);
        ingredientItem.Clear();

        OrderDish dish = OrderManager.Instance.GetDish();

        // Spawn food object tại Chef
        var f = UIPoolManager.GetUIObject(foodPrefab, dish.transform);
        f.transform.position = plateTrs.position;
        f.Init(food);
        foodItems.Add(f);
        // Lấy dish
        DOVirtual.DelayedCall(.1f, () =>
        {
            // Tween bay tới dish trên root transform
            Vector3 targetPos = dish.transform.position;
            float duration = 0.6f;
            f.transform.DOKill(); // Kill bất kỳ tween cũ
            f.transform.DOMove(targetPos, duration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    processText.text = "";
                    dish.AddFood(f); // thêm food vào dish
                    Debug.Log($"✅ {food.foodName} đã được đặt lên đĩa.");
                });
            candidateFoods.Clear();
        });
    }

    private void CleanPlate()
    {
        processText.text = "";

        foreach (var ing in ingredientItem)
        {
            UIPoolManager.ReturnObject(ing);
        }
        ingredientItem.Clear();
        candidateFoods.Clear();
    }

    public void ReturnFood(FoodItem item)
    {
        foodItems.Remove(item);
        UIPoolManager.ReturnObject(item);
    }

    public void ReturnFoodToPool()
    {
        CleanPlate();

        if (foodItems.Count == 0) return;
        foreach (var food in foodItems)
        {
            UIPoolManager.ReturnObject(food);
        }   
        foodItems.Clear();
    }
    private void MoveTo(Transform target, Vector3 pos, float duration, Action onDone = null)
    {
        target.DOMove(pos, duration)
              .SetEase(Ease.Linear)
              .OnComplete(() => onDone?.Invoke());
    }
}
