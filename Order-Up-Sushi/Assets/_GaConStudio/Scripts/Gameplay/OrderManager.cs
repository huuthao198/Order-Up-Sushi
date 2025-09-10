using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : SingletonBehavior<OrderManager>
{
    [Header("Dish Prefabs & Positions")]
    [SerializeField] private GameObject dishPrefab;
    [SerializeField] private Transform spawnLeft;
    [SerializeField] private Transform centerPos;
    [SerializeField] private Transform exitRight;

    [Header("Level Settings")]
    [SerializeField] private float successRateThreshold = 0.8f; // 80%

    private OrderDish currentDish;
    private int currentOrderIndex = 0;
    private LevelData levelData;

    private int totalOrders;
    private int successOrders;
    private int failedOrders;
    private int totalCoin;

    public static event Action<bool, int> OnLevelComplete;
    // bool = win/lose, int = coin reward

    public void StartLevel(LevelData levelData)
    {
        this.levelData = levelData;

        currentOrderIndex = 0;
        successOrders = 0;
        failedOrders = 0;
        totalCoin = 0;
        totalOrders = levelData.orders.Count;

        SpawnDish();
    }

    private void SpawnDish()
    {
        if (currentOrderIndex >= totalOrders)
        {
            CheckLevelComplete();
            return;
        }

        var orderData = levelData.orders[currentOrderIndex];
        currentOrderIndex++;

        var dishGO = ObjectPoolManager.GetObject(dishPrefab);

        CleanObj.CleanObject(dishGO);

        dishGO.transform.SetParent(this.transform);
        dishGO.transform.localPosition = spawnLeft.localPosition;
        dishGO.transform.localRotation = Quaternion.identity;

        OrderDish dish = dishGO.GetComponent<OrderDish>();
        currentDish = dish;

        dish.Setup(orderData.foods, orderData.timeLimit);

        dish.OnCompleted = (completedDish) =>
        {
            GameTimer.Instance.StopTimer();

            successOrders++;
            totalCoin += (int)orderData.coinReward;
            Debug.Log($"✅ Order Success! +{orderData.coinReward} coins (Total: {totalCoin})");

            UIGamePlayManager.Instance.ReturnFoodUI();
            UIGamePlayManager.Instance.ReturnFoodRecipeUI();

            MoveTo(completedDish.transform, exitRight.position, 1f, () =>
            {
                ChefController.Instance.ReturnFoodToPool();
                ObjectPoolManager.ReturnObject(completedDish.gameObject);
                currentDish = null;
                SpawnNext();
            });
        };

        MoveTo(dish.transform, centerPos.position, 1f, () =>
        {
            UIGamePlayManager.Instance.InitOrderUI(orderData.foods, dish.GetPointFood());
            StartDishTimer(dish, orderData);
        });
    }

    private void StartDishTimer(OrderDish dish, OrderData data)
    {
        GameTimer.Instance.SetTimer(data.timeLimit);

        GameTimer.Instance.StartTimer();
    }

    public void HandleOrderFail()
    {
        if (currentDish == null) return;

        failedOrders++;
        Debug.Log("❌ Order Failed! Time out!");

        // check lose early
        if (!CanStillWin())
        {
            Debug.Log("💥 Not enough orders left to reach success threshold → Lose early!");
            EndLevel(false);
            return;
        }
        UIGamePlayManager.Instance.ReturnFoodUI();
        UIGamePlayManager.Instance.ReturnFoodRecipeUI();
        MoveTo(currentDish.transform, exitRight.position, 1f, () =>
        {
            ObjectPoolManager.ReturnObject(currentDish.gameObject);
            currentDish = null;
            SpawnNext();
        });
    }

    private bool CanStillWin()
    {
        int ordersLeft = totalOrders - (successOrders + failedOrders);
        int maxPossibleSuccess = successOrders + ordersLeft;

        return (float)maxPossibleSuccess / totalOrders >= successRateThreshold;
    }

    private void CheckLevelComplete()
    {
        float rate = (float)successOrders / totalOrders;
        bool isWin = rate >= successRateThreshold;
        EndLevel(isWin);
    }

    private void EndLevel(bool isWin)
    {
        Debug.Log(isWin
            ? $"🎉 Level {levelData.levelId} Complete! Coin: {totalCoin}"
            : $"💀 Level {levelData.levelId} Failed! Coin: {totalCoin}");

        OnLevelComplete?.Invoke(isWin, totalCoin);
    }

    private void SpawnNext()
    {
        StartCoroutine(DelaySpawn(1f));
    }

    private IEnumerator DelaySpawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnDish();
    }

    private void MoveTo(Transform target, Vector3 pos, float duration, Action onDone = null)
    {
        target.DOMove(pos, duration)
              .SetEase(Ease.Linear)
              .OnComplete(() => onDone?.Invoke());
    }

    internal OrderDish GetDish()
    {
        return currentDish;
    }
}
