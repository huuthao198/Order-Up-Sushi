using System;
using UnityEngine;

public class UICommon : PersistantAndSingletonBehavior<UICommon>
{
    [SerializeField] private SettingPopup settingPopup;
    [SerializeField] private UIMessagePopup messagePopup;
    //[SerializeField] private HeartPopup heartPopup;

    [SerializeField] private GameObject adsPopup;

    Action adsDone;

    public void ShowSettingWithHome()
    {
        settingPopup.gameObject.SetActive(true);
        settingPopup.ShowWithHome();
    }

    public void ShowSettingWithGamePlay()
    {
        settingPopup.gameObject.SetActive(true);
        settingPopup.ShowWithGamePlay();
    }

    public void HideSettingPopup()
    {
        settingPopup.gameObject.SetActive(false);
    }

    public void ShowAds(Action adsDone)
    {
        this.adsDone = adsDone;
        adsPopup.SetActive(true);
    }

    public void CloseAds()
    {
        adsDone?.Invoke();
        adsDone = null;
        adsPopup.SetActive(false);
    }

    public void MessageBoxShow(string message)
    {
        messagePopup.gameObject.SetActive(true);
        messagePopup.Show(message);
    }

    //public void ShowHeartPopup(bool show)
    //{
    //    heartPopup.gameObject.SetActive(show);
    //    heartPopup.Show();
    //}
}
