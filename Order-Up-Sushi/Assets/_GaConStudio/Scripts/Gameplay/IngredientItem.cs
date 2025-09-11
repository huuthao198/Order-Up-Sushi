using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientItem : MonoBehaviour, IPoolableObject
{
    [SerializeField] private TextMeshProUGUI nameIngredient;
    [SerializeField] private Image img;

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
