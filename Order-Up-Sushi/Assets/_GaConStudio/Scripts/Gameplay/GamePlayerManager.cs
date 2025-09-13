using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerManager : SingletonBehavior<GamePlayerManager>
{
    // thêm dữ liệu level
    public LevelData CurrentLevelData => SaveManager.Instance.GetLevelData();
    public int Level => SaveManager.Instance.Leveled;
    public float Coin => SaveManager.Instance.Coin;
    private void Start()
    {
        StartCoroutine(DelayShow());
    }

    IEnumerator DelayShow()
    {
        yield return new WaitForSeconds(.4f);
        CutScene.Instance.Hide();
    }

    internal void StartLevel()
    {
        OrderManager.Instance.StartLevel(CurrentLevelData);
        IngredientGridManager.Instance.InitGrid();
    }

    internal void ReLoadLevel()
    {
        OrderManager.Instance.ResetOrder();
        UIGamePlayManager.Instance.ResetGame();
        OrderManager.Instance.StartLevel(CurrentLevelData);
        IngredientGridManager.Instance.RefreshIngredients();
        ChefController.Instance.InitChef();
        StartCoroutine(DelayShow());
    }

    internal void SaveData()
    {
        var newLevel = Level + 1;
        SaveManager.Instance.SetNewLevel(newLevel);
    }

    internal void AddCoin(float coin)
    {
        SaveManager.Instance.AddCoin(coin);
    }

    internal void SetNextLevel()
    {
        SaveManager.Instance.SetNewLevel(Level + 1);
    }
}
