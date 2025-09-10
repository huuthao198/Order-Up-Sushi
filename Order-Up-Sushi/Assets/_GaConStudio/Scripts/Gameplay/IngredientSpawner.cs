using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IngredientSpawner : SingletonBehavior<IngredientSpawner>
{
    [SerializeField] private Transform[] slots;
    [SerializeField] private IngredientObject ingredientPrefab;
    [SerializeField] private Transform plateTrs;
    [SerializeField] private TextMeshPro processText;

    private List<IngredientObject> ingredients = new ();

    public void SetupIngredients(LevelData levelData)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < levelData.availableIngredients.Count)
            {
                var id = levelData.availableIngredients[i];
                var go = ObjectPoolManager.GetObject(ingredientPrefab.gameObject);
                CleanObj.CleanObject(go);
                go.transform.SetParent(slots[i]);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                var ingredient = go.GetComponent<IngredientObject>();
                ingredient.Setup(id, null);
                ingredients.Add(ingredient);
            }   
            else
            {
                // không dùng slot này cho level
                slots[i].gameObject.SetActive(false);
            }
        }

        plateTrs.gameObject.SetActive(true);
    }

    public void SetProcess(string value)
    {
        processText.text = value;
    }

    private void OnDestroy()
    {
        if(ingredients.Count > 0)
        {
            foreach(var i in ingredients)
            {
                ObjectPoolManager.ReturnObject(i.gameObject);
            }
            ingredients.Clear();
        }
    }


}
