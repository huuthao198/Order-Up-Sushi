using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodItem : MonoBehaviour, IPoolableObject
{
    [SerializeField] private TextMeshProUGUI nameIngredient;
    [SerializeField] private Image foodImage;

    public FoodData Data => data;

    FoodData data;
    public void Init(FoodData data)
    {
        this.data = data;

        foodImage.sprite = AssetManager.Instance.GetFoodSpr(data.id);

        nameIngredient.text = data.foodName;
    }

    public void ResetState()
    {
        data = null;
        nameIngredient.text = string.Empty;
    }
}
