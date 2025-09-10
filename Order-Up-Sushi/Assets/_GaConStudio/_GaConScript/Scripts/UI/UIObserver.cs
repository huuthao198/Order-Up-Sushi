using System;
using UnityEngine;

public static class UIObserver
{
    // Battle events
    public static event Action<float> OnCoinChanged;

    // --- Method để trigger ---
    public static void CoinChanged(float coin) => OnCoinChanged?.Invoke(coin);
}
