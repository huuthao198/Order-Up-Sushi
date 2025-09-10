using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public int levelId;
    public List<OrderData> orders;
    public List<IngredientData> availableIngredients;
}

[System.Serializable]
public class OrderData
{
    public List<FoodData> foods; // id list
    public float timeLimit;
    public float coinReward;
}


[System.Serializable]
public class FoodData
{
    public int id;
    public string foodName;
    public List<IngredientData> Ingredients;
}

[System.Serializable]
public class IngredientData
{
    public int id;
    public string ingredientName;
}

public enum ItemType
{
    Food,
    Ingredient,
    EqualText,
    PlusText,
}