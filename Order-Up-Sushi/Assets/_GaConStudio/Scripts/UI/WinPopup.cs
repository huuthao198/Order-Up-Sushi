using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textCoinReward;

    float coinReward = 0;

    public void Show(float coinReward)
    {
        this.coinReward = coinReward;
        AudioManager.Instance?.PlayWinLevel();
        textCoinReward.text = string.Format($"{coinReward}");
    }

    public void CloseBtn()
    {
        CutScene.Instance.Show(() =>
        {
            SceneManager.LoadScene("Home");
        });
    }

    public void NextLevelBtn()
    {
        CutScene.Instance.Show(() =>
        {
            GamePlayerManager.Instance.ReLoadLevel();
        });
    }

    public void OnXRewardClick()
    {
        UICommon.Instance.ShowAds(() =>
        {
            GamePlayerManager.Instance.AddCoin(coinReward * 2);
        });
    }
}
