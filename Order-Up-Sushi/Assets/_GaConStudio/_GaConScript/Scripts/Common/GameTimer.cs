using System;
using UnityEngine;

public class GameTimer : PersistantAndSingletonBehavior<GameTimer>
{
    public float TimeCountdown { get; private set; }
    private bool isRunning = false;

    public event Action<float> OnTimeChanged; // mỗi frame báo UI
    public event Action OnTimeOver;           // khi hết giờ

    /// <summary>
    /// Đặt thời gian timer (không auto start).
    /// </summary>
    public void SetTimer(float time)
    {
        TimeCountdown = Mathf.Max(0, time);
        OnTimeChanged?.Invoke(TimeCountdown);
        isRunning = false;
    }

    /// <summary>
    /// Bắt đầu đếm timer (nếu chưa chạy).
    /// </summary>
    public void StartTimer()
    {
        if (isRunning) return;
        isRunning = true;
    }

    /// <summary>
    /// Tạm dừng timer (giữ nguyên TimeCountdown).
    /// </summary>
    public void PauseTimer()
    {
        isRunning = false;
    }

    /// <summary>
    /// Dừng hẳn timer.
    /// </summary>
    public void StopTimer()
    {
        isRunning = false;
        TimeCountdown = 0f;
        OnTimeChanged?.Invoke(TimeCountdown);
    }

    private void Update()
    {
        if (!isRunning) return;

        if (TimeCountdown <= 0f)
        {
            isRunning = false;
            TimeCountdown = 0f;
            OnTimeChanged?.Invoke(TimeCountdown);
            OnTimeOver?.Invoke();
            return;
        }

        TimeCountdown -= Time.deltaTime;
        if (TimeCountdown < 0) TimeCountdown = 0f;

        OnTimeChanged?.Invoke(TimeCountdown);
    }
}
