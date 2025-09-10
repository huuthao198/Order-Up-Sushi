using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIMessagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;

    Vector3 localPosition;
    private void Awake()
    {
        localPosition = transform.localPosition;
    }

    public void Show(string message)
    {
        messageText.text = message;
        transform.DOKill();
        transform.DOLocalMove(Vector3.zero, .85f).OnComplete(() =>
        {
            DOVirtual.DelayedCall(.75f, () =>
            {
                transform.DOLocalMove(localPosition, .75f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            });
        });
    }
}
