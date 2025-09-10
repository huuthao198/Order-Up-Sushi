using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class UIButton : MonoBehaviour
{
    [Header("Settings")]
    public bool IsScale = false;
    public float loopScale = 1.1f;      // scale to bao nhiêu khi loop
    public float loopDuration = 1f;   // thời gian scale 1 vòng

    public float clickScale = 1.2f;     // scale khi click
    public float clickDuration = 0.2f;  // thời gian scale click

    private Vector3 originalScale;
    private Button button;
    private Tween loopTween;

    private void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(PlayBtnClick);

        if (IsScale)
        {
            StartLoopScale();
        }
    }

    private void OnDestroy()
    {
        loopTween?.Kill();
    }

    private void StartLoopScale()
    {
        loopTween?.Kill();

        loopTween = transform.DOScale(originalScale * loopScale, loopDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void PlayBtnClick()
    {
        AudioManager.Instance?.PlayButtonClick();
    }
}
