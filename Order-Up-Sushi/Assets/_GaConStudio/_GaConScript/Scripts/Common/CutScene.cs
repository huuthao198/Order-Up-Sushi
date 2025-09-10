using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CutScene : PersistantAndSingletonBehavior<CutScene>
{
    [SerializeField] RectTransform canvasRect;
    [SerializeField] Transform img1;
    [SerializeField] Transform img2;
    Vector2 localImg1;
    Vector2 localImg2;

    public override void Awake()
    {
        base.Awake();

        localImg1 = img1.localPosition;
        localImg2 = img2.localPosition;
    }

    public void Show(Action complete)
    {
        img1.gameObject.SetActive(true);
        img2.gameObject.SetActive(true);

        RectTransform rt1 = img1 as RectTransform;
        RectTransform rt2 = img2 as RectTransform;

        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        Vector2 targetImg1 = new Vector2(-canvasWidth / 2, 0);
        Vector2 targetImg2 = new Vector2(canvasWidth / 2, 0);

        rt1.DOAnchorPos(targetImg1, 1f).SetEase(Ease.OutCubic);
        rt2.DOAnchorPos(targetImg2, 1f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            complete?.Invoke();
        });
    }


    public void Hide()
    {
        img1.DOKill();
        img1.DOLocalMove(localImg1, 1f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                img1.gameObject.SetActive(false);
            });
        img2.DOKill();
        img2.DOLocalMove(localImg2, 1f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                img2.gameObject.SetActive(false);
            });
    }
}
