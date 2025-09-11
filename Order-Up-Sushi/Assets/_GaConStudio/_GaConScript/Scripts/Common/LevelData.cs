using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public int levelId;
    public bool isHardLevel;
    public List<OrderData> orders;
    public List<IngredientData> availableIngredients;
    public int freeRefreshCount;   // số lần refresh free
    public int refreshCost;        // cost coin mỗi lần refresh sau khi hết free
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
    public int difficulty;
    public string foodName;
    public List<IngredientData> Ingredients;
}

[System.Serializable]
public class IngredientAllData
{
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