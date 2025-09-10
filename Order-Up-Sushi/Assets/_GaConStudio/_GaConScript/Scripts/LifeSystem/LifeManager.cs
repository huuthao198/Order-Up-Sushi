using UnityEngine;
using System;
public class LifeManager : PersistantAndSingletonBehavior<LifeManager>
{
    [Header("Config")]
    [SerializeField] private int maxLives = 5;
    [SerializeField] private float restoreDuration = 600f; // 10 phút (giây)

    public int CurrentLives { get; private set; }
    public float Timer { get; private set; }

    private DateTime lastQuitTime;

    // Events
    public event Action<int> OnLifeChanged;
    public event Action<string> OnTimerTick;
    public event Action OnLivesFull;

    private const string FULL_STATE = "Full";

    public override void Awake()
    {
        base.Awake();
        LoadData();
    }

    private void Update()
    {
        if (CurrentLives < maxLives)
        {
            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        if (CurrentLives >= maxLives)
        {
            Timer = 0;
            OnTimerTick?.Invoke(FULL_STATE);
            return;
        }

        Timer -= Time.deltaTime;

        if (Timer <= 0f)
        {
            RestoreLife();

            if (CurrentLives < maxLives)
            {
                Timer = restoreDuration;
            }
            else
            {
                Timer = 0f;
                OnLivesFull?.Invoke();
                OnTimerTick?.Invoke(FULL_STATE);
                return;
            }
        }

        TimeSpan time = TimeSpan.FromSeconds(Timer);
        string timerStr = $"{time.Minutes:D2}:{time.Seconds:D2}";
        OnTimerTick?.Invoke(timerStr);
    }

    public bool UseLife()
    {
        if (CurrentLives > 0)
        {
            CurrentLives--;
            OnLifeChanged?.Invoke(CurrentLives);

            if (CurrentLives < maxLives && Timer <= 0f)
            {
                Timer = restoreDuration;
            }

            SaveData();
            return true;
        }
        return false;
    }

    private void RestoreLife()
    {
        if (CurrentLives < maxLives)
        {
            CurrentLives++;
            OnLifeChanged?.Invoke(CurrentLives);
            SaveData();
        }
    }

    public void RestoreLife(int live)
    {
        if (CurrentLives < maxLives)
        {
            CurrentLives += live;
            OnLifeChanged?.Invoke(CurrentLives);
            UpdateTimer();
            SaveData();
        }
    }
    public void RestoreFullLife()
    {
        CurrentLives = maxLives;
        OnLifeChanged?.Invoke(CurrentLives);
        UpdateTimer();
        SaveData();
    }

    private void LoadData()
    {
        CurrentLives = PlayerPrefs.GetInt("Lives", maxLives);

        if (PlayerPrefs.HasKey("QuitTime"))
        {
            long quitBinary = Convert.ToInt64(PlayerPrefs.GetString("QuitTime"));
            lastQuitTime = DateTime.FromBinary(quitBinary);

            TimeSpan elapsed = DateTime.Now - lastQuitTime;
            float savedTimer = PlayerPrefs.GetFloat("Timer", restoreDuration); // remaining

            if (CurrentLives < maxLives)
            {
                // Timer đang là remaining → convert sang "đã đếm được bao nhiêu"
                double alreadyCounted = restoreDuration - savedTimer;
                double totalSeconds = alreadyCounted + elapsed.TotalSeconds;

                int livesRestored = (int)(totalSeconds / restoreDuration);
                CurrentLives = Mathf.Min(maxLives, CurrentLives + livesRestored);

                double leftover = totalSeconds % restoreDuration;

                if (CurrentLives < maxLives)
                {
                    Timer = (float)(restoreDuration - leftover);
                }
                else
                {
                    Timer = 0;
                }

                Debug.Log($"[LifeManager] LoadData: Lives={CurrentLives}, " +
                          $"SavedTimer(remain)={savedTimer:F0}s, " +
                          $"AlreadyCounted={alreadyCounted:F0}s, " +
                          $"Elapsed={elapsed.TotalSeconds:F0}s, " +
                          $"Total={totalSeconds:F0}s, " +
                          $"LivesRestored={livesRestored}, " +
                          $"Leftover={leftover:F0}s, " +
                          $"NewTimer(remain)={Timer:F0}s");
            }
            else
            {
                Timer = 0;
                Debug.Log($"[LifeManager] LoadData: Lives full ({CurrentLives}), Timer=0");
            }
        }
        else
        {
            // Lần đầu chơi → full mạng
            Timer = 0;
            Debug.Log("[LifeManager] LoadData: First time play, full lives.");
        }

        OnLifeChanged?.Invoke(CurrentLives);

        if (CurrentLives >= maxLives)
        {
            OnTimerTick?.Invoke(FULL_STATE);
        }
        else
        {
            TimeSpan time = TimeSpan.FromSeconds(Timer);
            string timerStr = $"{time.Minutes:D2}:{time.Seconds:D2}";
            OnTimerTick?.Invoke(timerStr);
        }
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("Lives", CurrentLives);
        PlayerPrefs.SetFloat("Timer", Timer);
        PlayerPrefs.SetString("QuitTime", DateTime.Now.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) SaveData();
    }

    internal bool IsFull()
    {
        return CurrentLives >= maxLives;
    }

    internal bool IsCanPlay()
    {
        return CurrentLives > 0;
    }
}
