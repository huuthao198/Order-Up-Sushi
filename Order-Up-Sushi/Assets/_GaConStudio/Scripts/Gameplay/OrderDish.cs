using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

public class OrderDish : MonoBehaviour
{
    [SerializeField] private Transform point;
    [Header("UI Food")]
    [SerializeField] private Transform foodUIContent;
    [SerializeField] private UIFood foodUIPrefab;

    private List<FoodData> foods;          // danh sách food cần cho order này
    private float timeLimit;
    private List<FoodData> addedFood = new List<FoodData>();

    List<UIFood> foodUIList = new();

    public Action<OrderDish> OnCompleted;

    public void Setup(List<FoodData> foods, float timeLimit)
    {
        this.foods = foods;
        this.timeLimit = timeLimit;
        addedFood.Clear();
        InitOrderUI(foods);
        ChefController.Instance.SetFood(foods);

        Debug.Log("Dish requires: " + string.Join(",", foods.Select(f => f.foodName)));
    }

    public void AddFood(FoodItem foodAdd)
    {
        Debug.Log("Add food " + foodAdd.Data.foodName);

        // kiểm tra xem món này có nằm trong foods không
        var required = foods.Find(f => f.id == foodAdd.Data.id);
        if (required == null)
        {
            Debug.LogError($"❌ {foodAdd.Data.foodName} không thuộc order này!");
            return;
        }
        // thêm vào addedFood
        addedFood.Add(foodAdd.Data);

        Debug.Log($"Progress: {addedFood.Count}/{foods.Count}");

        // kiểm tra complete
        if (IsCompleted())
        {
            Debug.Log("✅ Dish completed!");
            OnCompleted?.Invoke(this);
        }

        DoneFood(foodAdd.Data);
        UIGamePlayManager.Instance.DoneRecipeFood(foodAdd.Data);
        ChefController.Instance.ReturnFood(foodAdd);
    }

    public void InitOrderUI(List<FoodData> foods)
    {
        if (foods.Count == 0) return;

        foreach (var item in foods)
        {
            var f = UIPoolManager.GetUIObject(foodUIPrefab, foodUIContent);
            f.transform.localPosition = Vector3.zero;
            f.transform.localRotation = Quaternion.identity;
            f.transform.localScale = Vector3.one;

            f.Init(item);
            foodUIList.Add(f);
        }
    }


    public void ReturnFoodUI()
    {
        if (foodUIList.Count == 0) return;

        foreach (var item in foodUIList)
        {
            UIPoolManager.ReturnObject(item);
        }

        foodUIList.Clear();
    }

    private bool IsCompleted()
    {
        // group món trong order theo id
        var requiredGroups = foods.GroupBy(f => f.id)
                                  .ToDictionary(g => g.Key, g => g.Count());

        // group món đã add theo id
        var addedGroups = addedFood.GroupBy(f => f.id)
                                   .ToDictionary(g => g.Key, g => g.Count());

        // check tất cả món required đều đủ số lượng
        foreach (var kvp in requiredGroups)
        {
            int requiredCount = kvp.Value;
            int addedCount = addedGroups.ContainsKey(kvp.Key) ? addedGroups[kvp.Key] : 0;
            if (addedCount < requiredCount)
                return false;
        }

        return true;
    }

    public Transform GetPointFood()
    {
        return point;
    }

    public void DoneFood(FoodData food)
    {
        var f = foodUIList.Find(f => f.Data.id == food.id);
        if (f != null)
        {
            UIPoolManager.ReturnObject(f);
        }
    }
}
