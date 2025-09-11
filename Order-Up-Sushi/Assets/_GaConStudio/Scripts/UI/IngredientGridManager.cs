using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IngredientGridManager : SingletonBehavior<IngredientGridManager>
{
    [Header("Grid Settings")]
    [SerializeField] private int columns = 5;
    [SerializeField] private int rows = 6;

    [Header("References")]
    [SerializeField] private UIIngredientGrid ingredientPrefab;
    [SerializeField] private Transform gridParent; // có GridLayoutGroup
    [Header("UI Button Refesh")]
    [SerializeField] private UIRefeshButton refeshButton;
    [SerializeField] private UICoin coinUI;



    private IngredientAllData ingredientAllData;
    private IngredientData[,] grid;
    private GameObject[,] gridObjects;
    private System.Random random = new System.Random();

    private void Start()
    {
        ingredientAllData = SaveManager.Instance.GetIngredientData();

        grid = new IngredientData[columns, rows];
        gridObjects = new GameObject[columns, rows];

        //InitGrid();

        refeshButton.gameObject.SetActive(true);
        coinUI.gameObject.SetActive(true);
    }

    public void InitGrid()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                SpawnIngredientAt(x, y);
            }
        }
    }

    private void SpawnIngredientAt(int x, int y)
    {
        IngredientData ing = GetIngredientWeighted();
        grid[x, y] = ing;

        var go = UIPoolManager.GetUIObject(ingredientPrefab, gridParent);
        go.Init(ing);

        int col = x;
        int row = y;

        var btn = go.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => OnIngredientClicked(col, row));

        gridObjects[x, y] = go.gameObject;
    }

    private IngredientData GetIngredientWeighted()
    {
        // Lấy level hiện tại
        var level = SaveManager.Instance.GetLevelData();
        if (level == null || level.availableIngredients == null || level.availableIngredients.Count == 0)
        {
            // fallback: random toàn bộ
            return ingredientAllData.Ingredients[random.Next(ingredientAllData.Ingredients.Count)];
        }

        // Lấy order hiện tại
        var order = OrderManager.Instance.GetOrder();
        if (order == null || order.foods == null)
        {
            // fallback: random từ availableIngredients
            return level.availableIngredients[random.Next(level.availableIngredients.Count)];
        }

        // Gom tất cả nguyên liệu cần thiết cho order hiện tại
        HashSet<int> neededIds = new HashSet<int>(
            order.foods.SelectMany(f => f.Ingredients.Select(i => i.id))
        );

        var needed = level.availableIngredients.Where(i => neededIds.Contains(i.id)).ToList();
        var others = level.availableIngredients.Where(i => !neededIds.Contains(i.id)).ToList();

        // Tỉ lệ: cần thiết = 70%, còn lại = 30%
        if (random.NextDouble() < 0.3 && needed.Count > 0)
            return needed[random.Next(needed.Count)];
        else if (others.Count > 0)
            return others[random.Next(others.Count)];
        else
            return needed[random.Next(needed.Count)]; // fallback nếu chỉ toàn needed
    }

    private void OnIngredientClicked(int x, int y)
    {
        if (gridObjects[x, y] == null) return;
        // remove ingredient cũ
        var ingredient = gridObjects[x, y].GetComponent<UIIngredientGrid>();

        ChefController.Instance.AddIngredient(ingredient.Data, gridObjects[x, y].transform.position);

        UIPoolManager.ReturnObject(ingredient);
        gridObjects[x, y] = null;

        // spawn ingredient mới đúng tại vị trí (x, y)
        SpawnIngredientAt(x, y);
    }

    public void RefreshIngredients()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (gridObjects[x, y] != null)
                {
                    var ingredient = gridObjects[x, y].GetComponent<UIIngredientGrid>();
                    UIPoolManager.ReturnObject(ingredient);
                    gridObjects[x, y] = null; // reset reference
                }
                grid[x, y] = null;
                SpawnIngredientAt(x, y);
            }
        }
    }
}
