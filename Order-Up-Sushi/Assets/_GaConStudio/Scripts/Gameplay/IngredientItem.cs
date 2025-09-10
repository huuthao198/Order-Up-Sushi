using TMPro;
using UnityEngine;

public class IngredientItem : MonoBehaviour, IPoolableObject
{
    [SerializeField] private TextMeshPro nameIngredient;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public IngredientData Data => data;

    IngredientData data;
    public void Init(IngredientData data)
    {
        this.data = data;

        nameIngredient.text = data.ingredientName;
    }

    public void ResetState()
    {
        data = null;
        nameIngredient.text = string.Empty;
    }

}
