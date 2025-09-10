using System;
using UnityEngine;

public class GameTimer : PersistantAndSingletonBehavior<GameTimer>
{
    public float TimeCountdown { get; private set; }
    private bool isRunning = false;

    public event Action<float> OnTimeChanged; // callback UI
    public event Action OnTimeOver;

    public void SetTimer(float time)
    {
        TimeCountdown = time;
        // báo UI ngay từ đầu
        OnTimeChanged?.Invoke(TimeCountdown);
    }

    public void StarTime()
    {
        if(isRunning) return;
        isRunning = true;
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
        OnTimeChanged?.Invoke(TimeCountdown);
    }

    internal void StopTimer()
    {
        isRunning = false;
    }
}
