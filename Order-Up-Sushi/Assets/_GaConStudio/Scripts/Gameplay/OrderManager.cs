using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OrderManager : SingletonBehavior<OrderManager>
{
    [Header("Dish Prefabs & Positions")]
    [SerializeField] private OrderDish dishPrefab;
    [SerializeField] private Transform spawnLeft;
    [SerializeField] private Transform centerPos;
    [SerializeField] private Transform exitRight;

    [Header("Level Settings")]
    [SerializeField] private float successRateThreshold = 0.8f; // 80%

    private OrderDish currentDish;
    private OrderData currentOrderData;

    private int currentOrderIndex = 0;
    private LevelData levelData;

    private int totalOrders;
    private int successOrders;
    private int failedOrders;
    private int totalCoin;
    private int freeRefreshPerLevel;
    private int refreshCoinCost;
    private int freeRefreshRemaining;

    public static event Action<bool, int> OnLevelComplete;
    // bool = win/lose, int = coin reward
    public static event Action<int, bool> OnRefreshCostChanged;
    // int = coin cost, bool = isFree
    public static event Action<int> OnCoinChanged;

    public void StartLevel(LevelData levelData)
    {
        this.levelData = levelData;

        currentOrderIndex = 0;
        successOrders = 0;
        failedOrders = 0;
        totalCoin = 0;
        totalOrders = levelData.orders.Count;
        freeRefreshPerLevel = levelData.freeRefreshCount;
        refreshCoinCost = levelData.refreshCost;

        freeRefreshRemaining = freeRefreshPerLevel;

        OnCoinChanged?.Invoke(totalCoin);

        UpdateOrderCount();

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

        var dish = UIPoolManager.GetUIObject(dishPrefab, this.transform);

        dish.transform.localPosition = spawnLeft.localPosition;
        dish.transform.localRotation = Quaternion.identity;

        currentDish = dish;
        currentOrderData = orderData;

        dish.Setup(orderData.foods, orderData.timeLimit);

        dish.OnCompleted = (completedDish) =>
        {
            GameTimer.Instance.StopTimer();

            successOrders++;
            totalCoin += (int)orderData.coinReward;

            OnCoinChanged?.Invoke(totalCoin);

            Debug.Log($"✅ Order Success! +{orderData.coinReward} coins (Total: {totalCoin})");
            UpdateOrderCount();
            dish.ReturnFoodUI();
            UIGamePlayManager.Instance.ReturnFoodRecipeUI();
            ChefController.Instance.ReturnFoodToPool();

            DOVirtual.DelayedCall(.1f, () =>
            {
                MoveTo(completedDish.transform, exitRight.position, 1f, () =>
                {
                    UIPoolManager.ReturnObject(completedDish);

                    currentDish = null;
                    currentOrderData = null;
                    SpawnNext();
                });
            });
        };

        DOVirtual.DelayedCall(.1f, () =>
        {
            MoveTo(dish.transform, centerPos.position, 1f, () =>
            {
                UIGamePlayManager.Instance.InitOrderUI(orderData.foods, dish.GetPointFood());
                StartDishTimer(dish, orderData);
            });
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
        UpdateOrderCount();
        Debug.Log("❌ Order Failed! Time out!");

        // check lose early
        if (!CanStillWin())
        {
            Debug.Log("💥 Not enough orders left to reach success threshold → Lose early!");
            EndLevel(false);
            return;
        }

        currentDish.ReturnFoodUI();
        UIGamePlayManager.Instance.ReturnFoodRecipeUI();

        DOVirtual.DelayedCall(.1f, () =>
        {
            MoveTo(currentDish.transform, exitRight.position, 1f, () =>
            {
                ResetOrder();
                currentDish = null;
                currentOrderData = null;
                SpawnNext();
            });
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
        if (isWin)
        {
            //Setup next level count
            GamePlayerManager.Instance.SetNextLevel();
        }

        GamePlayerManager.Instance.AddCoin(totalCoin);
        OnLevelComplete?.Invoke(isWin, totalCoin);
    }

    private void SpawnNext()
    {
        StartCoroutine(DelaySpawn(.2f));
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

    internal OrderData GetOrder()
    {
        return currentOrderData;
    }

    public bool TryUseRefresh()
    {
        if (freeRefreshRemaining > 0)
        {
            freeRefreshRemaining--;
            UpdateRefreshUI();
            return true;
        }
        else
        {
            if (totalCoin >= refreshCoinCost)
            {
                totalCoin -= refreshCoinCost;
                UpdateRefreshUI();
                OnCoinChanged?.Invoke(totalCoin);
                return true;
            }
            else
            {
                Debug.Log("❌ Not enough coin to refresh!");
                return false;
            }
        }
    }

    private void UpdateRefreshUI()
    {
        bool isFree = freeRefreshRemaining > 0;
        int cost = isFree ? 0 : refreshCoinCost;

        OnRefreshCostChanged?.Invoke(cost, isFree);
    }

    private void UpdateOrderCount()
    {
        UIGamePlayManager.Instance.UpdateOrderCount(successOrders, failedOrders, totalOrders);
    }

    public void ResetOrder()
    {
        if(currentDish != null)
        {
            currentDish.ReturnFoodUI();
            UIPoolManager.ReturnObject(currentDish);
        }
        currentDish = null;

        ChefController.Instance.ReturnFoodToPool();
    }
}
