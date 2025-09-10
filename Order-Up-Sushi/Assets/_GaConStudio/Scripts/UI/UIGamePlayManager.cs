using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections.Generic;
public class UIGamePlayManager : SingletonBehavior<UIGamePlayManager>
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button startBtn;
    [Header("UI Food")]
    [SerializeField] private Transform foodUIContent;
    [SerializeField] private UIFood foodUIPrefab;
    [Header("UI Recipe")]
    [SerializeField] private UIRecipeFood recipeUIPrefab;
    [SerializeField] private Transform recipeUIContent;

    List<UIFood> foodList = new ();
    List<UIRecipeFood> recipeList = new ();
    private void Start()
    {
        StartCoroutine(DelayLoad());
    }

    IEnumerator DelayLoad()
    {
        yield return new WaitForSeconds(.3f);
        CutScene.Instance.Hide();
    }

    public void StartGame()
    {
        startBtn.gameObject.SetActive(false);
        var levelData = SaveManager.Instance.GetLevelData();
        OrderManager.Instance.StartLevel(levelData);
        IngredientSpawner.Instance.SetupIngredients(levelData);
    }

    public void ResetGame()
    {
        startBtn.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        GameTimer.Instance.OnTimeChanged += UpdateTimerUI;
        GameTimer.Instance.OnTimeOver += HandleTimeOver;
    }

    private void OnDisable()
    {
        if (GameTimer.Instance == null) return;

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
            var f = UIPoolManager.GetUIObject(foodUIPrefab, foodUIContent);
            f.transform.localPosition = Vector3.zero;
            f.transform.localRotation = Quaternion.identity;
            f.transform.localScale = Vector3.one;

            f.Init(item);
            foodList.Add(f);


            var r = UIPoolManager.GetUIObject(recipeUIPrefab, recipeUIContent);
            r.Setup(item);
            recipeList.Add(r);
        }

        foodUIContent.position = Camera.main.WorldToScreenPoint(dish.transform.position);
    }

    public void ReturnFoodUI()
    {
        if(foodList.Count == 0) return;

        foreach (var item in foodList)
        {
            UIPoolManager.ReturnObject(item);
        }

        foodList.Clear();
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
}
