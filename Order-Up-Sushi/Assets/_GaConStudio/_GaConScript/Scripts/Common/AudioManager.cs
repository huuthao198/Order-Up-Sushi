using System.Collections.Generic;
using UnityEngine;

public class AudioManager : PersistantAndSingletonBehavior<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;   // Nhạc nền
    [SerializeField] private AudioSource sfxSource;     // Hiệu ứng âm thanh

    [Header("Music Clips (Random)")]
    [SerializeField] private AudioClip[] backgroundMusics;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip[] winPlateClips; // 3 SFX random
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip winLevelClip;
    [SerializeField] private AudioClip loseLevelClip;

    // === MUSIC ===
    public void InitAudio()
    {
        ApplySettings();
        PlayRandomMusic();
    }

    // === SETTINGS ===
    public void ApplySettings()
    {
        if (SaveManager.Instance == null) return;

        musicSource.mute = !SaveManager.Instance.IsMusic;
        sfxSource.mute = !SaveManager.Instance.IsSound;
    }

    // === MUSIC ===
    public void PlayRandomMusic()
    {
        if (!SaveManager.Instance.IsMusic) return;
        if (backgroundMusics.Length == 0) return;

        int rand = Random.Range(0, backgroundMusics.Length);
        musicSource.clip = backgroundMusics[rand];
        musicSource.loop = true;
        musicSource.Play();
    }

    public void EnableMusic(bool isOn)
    {
        musicSource.mute = !isOn;

        if (isOn)
        {
            Debug.Log("Play Music");
            PlayRandomMusic();
        }
        else
        {
            Debug.Log("Stop Music");
            musicSource.Stop();
        }
    }


    public void EnableSound(bool isOn)
    {
        sfxSource.mute = !isOn;
    }
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // === SFX ===
    public void PlayWinPlate()
    {
        if (!SaveManager.Instance.IsSound) return;
        if (winPlateClips.Length == 0) return;

        int rand = Random.Range(0, winPlateClips.Length);
        sfxSource.PlayOneShot(winPlateClips[rand]);
    }

    public void PlayButtonClick()
    {
        if (!SaveManager.Instance.IsSound) return;
        if (buttonClickClip != null)
            sfxSource.PlayOneShot(buttonClickClip);
    }

    public void PlayWinLevel()
    {
        if (!SaveManager.Instance.IsSound) return;
        if (winLevelClip != null)
            sfxSource.PlayOneShot(winLevelClip);
    }

    public void PlayLoseLevel()
    {
        if (!SaveManager.Instance.IsSound) return;
        if (loseLevelClip != null)
            sfxSource.PlayOneShot(loseLevelClip);
    }
}
