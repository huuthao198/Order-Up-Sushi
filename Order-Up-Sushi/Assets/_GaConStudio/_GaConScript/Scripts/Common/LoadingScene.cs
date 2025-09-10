using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] Slider loadingSlider;
    [SerializeField] TextMeshProUGUI percentLoadingText;
    [SerializeField] float minSliderTime = 1f;

    private void Start()
    {
        loadingSlider.value = 0;
        percentLoadingText.text = "0%";

        StartCoroutine(LoadLevelsCoroutine());
    }

    private IEnumerator LoadLevelsCoroutine()
    {
        float startTime = Time.realtimeSinceStartup;

        yield return AddressableAssetCache.Instance.LoadAllLevelsWithProgress(
            progress =>
            {
                // Thay vì set trực tiếp => tween tới giá trị mới
                loadingSlider.DOKill(); // hủy tween cũ (nếu có)
                loadingSlider
                    .DOValue(progress, 0.3f) // tween trong 0.3s (tùy chỉnh)
                    .SetEase(Ease.OutCubic)
                    .OnUpdate(() =>
                    {
                        percentLoadingText.text = Mathf.RoundToInt(loadingSlider.value * 100) + "%";
                    });
            },
            () => { },
            batchSize: 80
        );
 
        // Đảm bảo tổng thời gian tối thiểu
        float elapsed = Time.realtimeSinceStartup - startTime;
        if (elapsed < minSliderTime)
        {
            yield return loadingSlider
                .DOValue(1f, minSliderTime - elapsed)
                .SetEase(Ease.OutCubic)
                .OnUpdate(() =>
                {
                    percentLoadingText.text = Mathf.RoundToInt(loadingSlider.value * 100) + "%";
                })
                .WaitForCompletion();
        }
        else
        {
            // Tween nốt đến 1 để cảm giác mượt
            yield return loadingSlider
                .DOValue(1f, 0.5f)
                .SetEase(Ease.OutCubic)
                .OnUpdate(() =>
                {
                    percentLoadingText.text = Mathf.RoundToInt(loadingSlider.value * 100) + "%";
                })
                .WaitForCompletion();
        }

        HandleLoadComplete();
    }

    private void HandleLoadComplete()
    {
        Debug.Log("Load Scene Home");
        SaveManager.Instance.LoadData();
        AudioManager.Instance.InitAudio();
        CutScene.Instance.Show(() =>
        {
            SceneManager.LoadScene("Home");
        });
    }
}
