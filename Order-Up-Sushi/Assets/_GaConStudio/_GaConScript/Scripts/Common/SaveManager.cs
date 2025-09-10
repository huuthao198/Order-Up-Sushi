using System;
using UnityEngine;

public class SaveManager : PersistantAndSingletonBehavior<SaveManager>
{
    public int Leveled => level;
    public float Coin => coin;
    public bool IsSound => isSound;
    public bool IsMusic => isMusic;

    private float coin;
    private int level;
    private LevelData levelData;
    private bool isMusic;
    private bool isSound;

    public void LoadData()
    {

        if (PlayerPrefs.HasKey("Coin"))
        {
            coin = GetCoin();
        }
        else
        {
            coin = 0;
        }

        if (PlayerPrefs.HasKey("Level"))
        {
            level = GetLevel();
        }
        else
        {
            level = 1;
        }

        if(PlayerPrefs.HasKey("IsSound"))
        {
            isSound = GetSound();
        }    
        else
        {
            isSound = true;
        }

        if (PlayerPrefs.HasKey("IsMusic"))
        {
            isMusic = GetMusic();
        }
        else
        {
            isMusic = true;
        }

        LoadLevel(level);
    }

    private bool GetSound()
    {
        // Ép int -> bool (1 = true, 0 = false)
        return PlayerPrefs.GetInt("IsSound", 0) == 1;
    }

    public void SetSound(bool sound)
    {
        isSound = sound;
        // Ép bool -> int
        PlayerPrefs.SetInt("IsSound", sound ? 1 : 0);
        PlayerPrefs.Save();
    }

    private bool GetMusic()
    {
        return PlayerPrefs.GetInt("IsMusic", 0) == 1;
    }

    public void SetMusic(bool music)
    {
        isMusic = music;
        PlayerPrefs.SetInt("IsMusic", music ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadLevel(int levelId)
    {
        //string fileName = $"level_{levelId}";
        //levelData = LevelLoader.LoadLevel(fileName);

        //levelData = AddressableAssetCache.Instance.GetLevel(levelId);

        //if (levelData != null)
        //{
        //    Debug.Log("Load level thành công: " + levelData.levelId);
        //}


        string fileName = $"level_{levelId}";
        levelData = LevelLoader.LoadLevel(fileName);
        if (levelData != null)
        {
            Debug.Log($"Loaded Level {levelData.levelId} with {levelData.orders.Count} orders.");
        }
}

    private float GetCoin()
    {
        return PlayerPrefs.GetFloat("Coin");
    }

    private int GetLevel()
    {
        return PlayerPrefs.GetInt("Level");
    }

    public void AddCoin(float coin)
    {
        this.coin += coin;
        UIObserver.CoinChanged(this.coin);
        SaveCoin();
    }

    public void SubCoin(float coin)
    {
        this.coin -= coin;
        UIObserver.CoinChanged(this.coin);
        SaveCoin();
    }

    public void SaveCoin()
    {
        PlayerPrefs.SetFloat("Coin", coin);
        PlayerPrefs.Save();
    }

    public void SaveLevel(int level)
    {
        this.level = level;
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.Save();
    }

    internal LevelData GetLevelData()
    {
        return levelData;
    }

    internal void SetNewLevel(int level)
    {
        SaveLevel(level);
        LoadLevel(level);
    }
}
