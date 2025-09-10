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

    private void Start()
    {
        loadingSlider.value = 0;
        percentLoadingText.text = "0%";
        LoadLevelsCoroutine();
    }

    private void LoadLevelsCoroutine()
    {
        loadingSlider.DOKill();
        loadingSlider.DOValue(1f, 1f)
            .OnComplete(() => 
            { 
                HandleLoadComplete();
            });
    }

    private void HandleLoadComplete()
    {
        Debug.Log("Load Scene Home");
        SaveManager.Instance?.LoadData();
        AudioManager.Instance?.InitAudio();
        CutScene.Instance.Show(() =>
        {
            SceneManager.LoadScene("GamePlay");
        });
    }
}
