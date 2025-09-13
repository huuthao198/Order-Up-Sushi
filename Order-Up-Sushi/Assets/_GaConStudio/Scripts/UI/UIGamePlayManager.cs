using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class UIGamePlayManager : SingletonBehavior<UIGamePlayManager>
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI orderSusscesCountText;
    [SerializeField] private TextMeshProUGUI orderFaildCountText;

    [SerializeField] private Button startBtn;
    [Header("UI Recipe")]
    [SerializeField] private UIRecipeFood recipeUIPrefab;
    [SerializeField] private Transform recipeUIContent;
    [Header("UI Ingredient")]
    [SerializeField] private Transform IngredientUI;
    [Header("UI Popup")]
    [SerializeField] private WinPopup winPopup;
    [SerializeField] private LosePopup losePopup;
    List<UIRecipeFood> recipeList = new ();

    private void Start()
    {
        winPopup.gameObject.SetActive (false);
        losePopup.gameObject.SetActive (false);

        levelText.text = string.Format($"Level\n{GamePlayerManager.Instance.Level}");
        StartCoroutine(DelayLoad());

        OrderManager.OnLevelComplete += OrderManager_OnLevelComplete;
    }

    private void OrderManager_OnLevelComplete(bool isWin, int totalCoin)
    {
        if (isWin) 
        { 
            winPopup.gameObject.SetActive(true);
            winPopup.Show(totalCoin);
        }
        else
        {
            losePopup.gameObject.SetActive(true);
            losePopup.Show();
        }
    }

    IEnumerator DelayLoad()
    {
        yield return new WaitForSeconds(.3f);
        CutScene.Instance.Hide();
        StartGame();
    }

    public void StartGame()
    {
        startBtn.gameObject.SetActive(false);
        IngredientUI.DOKill();
        IngredientUI.DOLocalMove(Vector3.zero, .5f)
            .OnComplete(() =>
            {
                GamePlayerManager.Instance.StartLevel();
                ChefController.Instance.InitChef();
            });
    }

    public void ResetGame()
    {
        startBtn.gameObject.SetActive(false);
        winPopup.gameObject.SetActive(false);
        losePopup.gameObject.SetActive(false);
        levelText.text = string.Format($"Level\n{GamePlayerManager.Instance.Level}");
        ReturnFoodRecipeUI();

    }

    private void OnEnable()
    {
        GameTimer.Instance.OnTimeChanged += UpdateTimerUI;
        GameTimer.Instance.OnTimeOver += HandleTimeOver;
    }

    private void OnDisable()
    {
        OrderManager.OnLevelComplete -= OrderManager_OnLevelComplete;

        GameTimer.Instance.OnTimeChanged -= UpdateTimerUI;
        GameTimer.Instance.OnTimeOver -= HandleTimeOver;
    }

    private void UpdateTimerUI(float timeLeft)
    {
        string timeStr = TimeSpan.FromSeconds(Mathf.CeilToInt(timeLeft)).ToString(@"mm\:ss");
        timerText.text = timeStr;
    }

    private void HandleTimeOver()
    {
        Debug.Log("⏰ UI báo hết giờ!");
        // ✅ Gọi OrderManager để xử lý fail dish
        OrderManager.Instance.HandleOrderFail();
    }

    public void InitOrderUI(List<FoodData> foods, Transform dish)
    {
        if(foods.Count == 0) return;

        foreach (var item in foods)
        {
            var r = UIPoolManager.GetUIObject(recipeUIPrefab, recipeUIContent);
            r.Setup(item);
            recipeList.Add(r);
        }
    }

    public void ReturnFoodRecipeUI()
    {
        if (recipeList.Count == 0) return;

        foreach (var item in recipeList)
        {
            UIPoolManager.ReturnObject(item);
        }

        recipeList.Clear();
    }

    public void DoneRecipeFood(FoodData food)
    {
        var r = recipeList.Find(r => r.Data.id == food.id);
        if (r != null) 
        {
            UIPoolManager.ReturnObject(r);
        }
    }

    public void UpdateOrderCount(int sussces, int faild, int total)
    {
        orderSusscesCountText.text = $"{sussces}/{total}";
        orderFaildCountText.text = $"{faild}/{total}";

    }
}
