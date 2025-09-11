using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRefeshButton : MonoBehaviour
{
    [SerializeField] private Button refreshButton;
    [SerializeField] private TextMeshProUGUI refreshText;

    private void OnEnable()
    {
        OrderManager.OnRefreshCostChanged += UpdateUI;
    }

    private void OnDisable()
    {
        OrderManager.OnRefreshCostChanged -= UpdateUI;
    }

    private void Start()
    {
        refreshButton.onClick.RemoveAllListeners();
        refreshButton.onClick.AddListener(OnClickRefresh);
    }

    private void UpdateUI(int cost, bool isFree)
    {
        if (isFree)
        {
            refreshText.text = "Free";
        }
        else
        {
            refreshText.text = $"-{cost}";
        }
    }

    private void OnClickRefresh()
    {
        if (OrderManager.Instance.TryUseRefresh())
        {
            IngredientGridManager.Instance.RefreshIngredients();
        }
    }
}
