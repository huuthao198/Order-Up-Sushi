using TMPro;
using UnityEngine;

public class UICoin : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cointText;

    private void OnEnable()
    {
        OrderManager.OnCoinChanged += UpdateUI;
    }

    private void OnDisable()
    {
        OrderManager.OnCoinChanged -= UpdateUI;
    }

    private void UpdateUI(int coint)
    {
        cointText.text = $"{coint}";
    }
}
