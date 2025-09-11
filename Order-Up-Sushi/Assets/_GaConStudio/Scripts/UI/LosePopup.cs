using UnityEngine;
using UnityEngine.SceneManagement;

public class LosePopup : MonoBehaviour
{
    public void Show()
    {
        AudioManager.Instance?.PlayLoseLevel();
    }

    public void CloseBtn()
    {
        CutScene.Instance.Show(() =>
        {
            SceneManager.LoadScene("Home");
        });
    }

    public void OnTryAgainBtn()
    {
        CutScene.Instance.Show(() =>
        {
            GamePlayerManager.Instance.ReLoadLevel();
        });
    }

    public void OnAddTimeBtn()
    {
        UICommon.Instance.ShowAds(() => 
        {
            GameTimer.Instance.SetTimer(30);
            GameTimer.Instance.StartTimer();
            gameObject.SetActive(false);
        });
    }
}
