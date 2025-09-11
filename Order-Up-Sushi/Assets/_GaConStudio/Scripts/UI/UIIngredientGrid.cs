using TMPro;
using UnityEngine;

public class UIIngredientGrid : MonoBehaviour, IPoolableObject
{
    [SerializeField] private TextMeshProUGUI nameIngredient;

    public IngredientData Data => data;

    private IngredientData data;

    public void Init(IngredientData data)
    {
        this.data = data;
        nameIngredient.text = data.ingredientName;
    }

    public void ResetState()
    {
        data = null;
    }
}
