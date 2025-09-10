using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingPopup : MonoBehaviour
{
    [SerializeField] private ToggleSetting soundToggle;
    [SerializeField] private ToggleSetting musicToggle;
    [SerializeField] private GameObject btnAll;

    private bool isMusic => SaveManager.Instance.IsMusic;
    private bool isSound => SaveManager.Instance.IsSound;

    public void ShowWithGamePlay()
    {
        btnAll.SetActive(true);
        musicToggle.OnToggle(isMusic);
        soundToggle.OnToggle(isSound);
    }

    public void ShowWithHome()
    {
        btnAll.SetActive(false);
        musicToggle.OnToggle(isMusic);
        soundToggle.OnToggle(isSound);
    }

    public void OnBtnMusicClick()
    {
        SaveManager.Instance.SetMusic(!isMusic);
        musicToggle.OnToggle(isMusic);
        AudioManager.Instance.EnableMusic(isMusic);
    }

    public void OnBtnSoundClick() 
    {
        SaveManager.Instance.SetSound(!isSound);
        soundToggle.OnToggle(isSound);
        AudioManager.Instance.EnableSound(isSound);
    }

    public void OnBtnHomeClick()
    {
        UICommon.Instance.HideSettingPopup();
        CutScene.Instance.Show(() =>
        {
            SceneManager.LoadScene("Home");
        });

    }

    public void OnRestartClick()
    {

        if (LifeManager.Instance.UseLife())
        {
            Debug.Log("Restart Level");
            UICommon.Instance.HideSettingPopup();
            CutScene.Instance.Show(() =>
            {
                OnClose();
                //GamePlayerManager.Instance.ReLoadLevel();
            });
        }
        else
        {
            Debug.Log("không đủ life!");
            UICommon.Instance.MessageBoxShow("Not enough hearts!");
            //UICommon.Instance.ShowHeartPopup(true);
        }
    }

    public void OnPauseClick()
    {

        OnClose();
    }

    public void OnClose()
    {
        GameTimer.Instance.StarTime();
        gameObject.SetActive(false);
    }
}
