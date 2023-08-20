using System;
using System.Threading;
using ScreenManagerClient.Models;

namespace ScreenManagerClient.Logic;

public class Timer
{
    private System.Threading.Timer? _timer = null;

    public Timer()
    {

    }

    public void StartTimer(UpdatePeriod updatePeriod, Action onTimerTick)
    {
        if (_timer != null)
        {
            _timer.Dispose();
        }
        _timer = new System.Threading.Timer(
            state => onTimerTick(),
            null,
             updatePeriod.Seconds * 1000,
            updatePeriod.Seconds * 1000);
    }

    public void StopTimer()
    {
        _timer?.Dispose();
    }
}