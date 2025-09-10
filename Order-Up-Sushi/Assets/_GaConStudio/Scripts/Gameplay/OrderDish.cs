using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

public class OrderDish : MonoBehaviour
{
    [SerializeField] private Transform point;

    private List<FoodData> foods;          // danh sách food cần cho order này
    private float timeLimit;
    private List<FoodData> addedFood = new List<FoodData>();

    public Action<OrderDish> OnCompleted;

    public void Setup(List<FoodData> foods, float timeLimit)
    {
        this.foods = foods;
        this.timeLimit = timeLimit;
        addedFood.Clear();

        ChefController.Instance.SetFood(foods);

        Debug.Log("Dish requires: " + string.Join(",", foods.Select(f => f.foodName)));
    }

    public void AddFood(FoodData foodAdd)
    {
        Debug.Log("Add food " + foodAdd.foodName);

        // kiểm tra xem món này có nằm trong foods không
        var required = foods.Find(f => f.id == foodAdd.id);
        if (required == null)
        {
            Debug.LogError($"❌ {foodAdd.foodName} không thuộc order này!");
            return;
        }

        // thêm vào addedFood nếu chưa có
        if (!addedFood.Any(f => f.id == foodAdd.id))
        {
            addedFood.Add(foodAdd);
        }

        Debug.Log($"Progress: {addedFood.Count}/{foods.Count}");

        // kiểm tra complete
        if (IsCompleted())
        {
            Debug.Log("✅ Dish completed!");
            OnCompleted?.Invoke(this);
        }
    }

    private bool IsCompleted()
    {
        if (addedFood.Count != foods.Count)
            return false;

        // check tất cả món required đều có trong addedFood
        return foods.All(req => addedFood.Any(f => f.id == req.id));
    }

    public Transform GetPointFood()
    {
        return point;
    }
}
