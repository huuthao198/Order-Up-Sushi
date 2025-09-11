using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodItem : MonoBehaviour, IPoolableObject
{
    [SerializeField] private TextMeshProUGUI nameIngredient;
    [SerializeField] private Image spriteRenderer;

    public FoodData Data => data;

    FoodData data;
    public void Init(FoodData data)
    {
        this.data = data;

        nameIngredient.text = data.foodName;
    }

    public void ResetState()
    {
        data = null;
        nameIngredient.text = string.Empty;
    }
}
