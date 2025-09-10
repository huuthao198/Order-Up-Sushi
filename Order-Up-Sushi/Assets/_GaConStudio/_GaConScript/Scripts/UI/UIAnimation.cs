using UnityEngine;
using DG.Tweening;
public class UIAnimation : MonoBehaviour
{
    [Header("Loop Scale Settings")]
    public bool IsScaleLoop = false;
    public float loopScale = 1.1f;       // scale to bao nhiêu
    public float loopDuration = 0.5f;    // thời gian scale 1 vòng

    private Vector3 originalScale;
    private Tween loopTween;

    private void Awake()
    {
        originalScale = transform.localScale;

        if (IsScaleLoop)
        {
            StartLoopScale();
        }
    }

    private void OnDestroy()
    {
        loopTween?.Kill();
    }

    /// <summary>
    /// Bắt đầu loop scale to nhỏ vô hạn
    /// </summary>
    public void StartLoopScale()
    {
        loopTween?.Kill();

        loopTween = transform.DOScale(originalScale * loopScale, loopDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    /// <summary>
    /// Dừng loop scale
    /// </summary>
    public void StopLoopScale()
    {
        loopTween?.Kill();
        transform.localScale = originalScale;
    }
}
