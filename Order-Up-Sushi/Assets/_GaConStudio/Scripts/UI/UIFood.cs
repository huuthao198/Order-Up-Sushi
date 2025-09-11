using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UIFood : MonoBehaviour, IPoolableObject
{
    [SerializeField] private TextMeshProUGUI foodName;
    [SerializeField] private Image foodImage;

    public FoodData Data => foodData;

    FoodData foodData;
    public void Init(FoodData data)
    {
        foodData = data;
        this.foodName.text = foodData.foodName;
    }

    public void ResetState()
    {
        foodData = null;
        foodName.text = string.Empty;
        foodImage.sprite = null;
    }
}
