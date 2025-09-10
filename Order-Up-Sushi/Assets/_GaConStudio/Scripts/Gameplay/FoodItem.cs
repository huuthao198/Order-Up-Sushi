using TMPro;
using UnityEngine;

public class FoodItem : MonoBehaviour, IPoolableObject
{
    [SerializeField] private TextMeshPro nameIngredient;
    [SerializeField] private SpriteRenderer spriteRenderer;

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
