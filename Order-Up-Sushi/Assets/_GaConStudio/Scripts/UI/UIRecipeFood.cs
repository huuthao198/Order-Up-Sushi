using System.Collections.Generic;
using UnityEngine;

public class UIRecipeFood : MonoBehaviour, IPoolableObject
{
    [Header("Prefabs")]
    [SerializeField] private UIRecipeItem foodIconPrefab;
    [SerializeField] private UIRecipeItem ingredientIconPrefab;
    [SerializeField] private UIRecipeItem equalPrefab;
    [SerializeField] private UIRecipeItem plusPrefab;

    [Header("Container")]
    [SerializeField] private Transform container; // Có HorizontalLayoutGroup

    private List<UIRecipeItem> items = new List<UIRecipeItem>();

    public FoodData Data => foodData;

    private FoodData foodData;
    public void ResetState()
    {
        foodData = null;
        if (items.Count == 0) return;

        foreach (var item in items)
        {
            UIPoolManager.ReturnObject(item);
        }
        items.Clear();
    }

    public void Setup(FoodData foodData)
    {
        this.foodData = foodData;
        // Food Icon
        var food = UIPoolManager.GetUIObject(foodIconPrefab, container);
        food.Init(foodData.id, foodData.foodName, ItemType.Food);
        items.Add(food);
        // Equal Sign
        var equal = UIPoolManager.GetUIObject(equalPrefab, container);
        equal.Init(0, "=", ItemType.EqualText);
        items.Add(equal);

        var Ingredients = foodData.Ingredients;
        // Ingredients + Plus signs
        for (int i = 0; i < Ingredients.Count; i++)
        {
            var ingredient = UIPoolManager.GetUIObject(ingredientIconPrefab, container);
            ingredient.Init(Ingredients[i].id, Ingredients[i].ingredientName, ItemType.Ingredient);
            items.Add(ingredient);

            if (i < Ingredients.Count - 1)
            {
                var plus = UIPoolManager.GetUIObject(plusPrefab, container);
                plus.Init(0, "+", ItemType.PlusText);
                items.Add(plus);
            }
        }
    }
}
