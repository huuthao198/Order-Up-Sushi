using UnityEngine;
using TMPro;
public class IngredientObject : MonoBehaviour, IPoolableObject
{
    [SerializeField] private TextMeshPro nameIngredient;

    IngredientData data;
    public void Setup(IngredientData data, Sprite sprite)
    {
        this.data = data;
        nameIngredient.text = data.ingredientName;
    }

    private void OnMouseDown()
    {
        Debug.Log($"Clicked {data.ingredientName}");
        // có thể làm hiệu ứng ingredient bay vào dish
        ChefController.Instance.AddIngredient(data, transform.position);
    }


    public void ResetState()
    {
        data = null;
        nameIngredient.text = "";
    }
}
