using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipeItem : MonoBehaviour, IPoolableObject
{
    [SerializeField] private TextMeshProUGUI recipeText;
    [SerializeField] private Image icon;
    public void ResetState()
    {
        icon.sprite = null;
        recipeText.text = "";
    }
    
    public void Init(int id, string name, ItemType type)
    {
        if(type == ItemType.Food || type == ItemType.Ingredient)
        {
            icon.gameObject.SetActive(true);

            if(type == ItemType.Food)
            {
                var spr = AssetManager.Instance.GetFoodSpr(id);
                icon.sprite = spr;
            }

        }
        else icon.gameObject.SetActive(false);

        recipeText.text = name.ToString();
        //Get spr food, ingredents with id
    }

}
