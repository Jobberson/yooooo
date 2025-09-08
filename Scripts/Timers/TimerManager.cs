using System.Collections.Generic;
using UnityEngine;
using Snog.Timers;

public class TimerManager : MonoBehaviour
{
    private List<Timer> timers = new();

    public void AddTimer(float duration, System.Action onTimerEnd = null)
    {
        Timer newTimer = new(duration, onTimerEnd);
        timers.Add(newTimer);
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        foreach (var timer in timers)
        {
            timer.UpdateTimer(deltaTime);
        }

        // Optional: remove finished timers
        timers.RemoveAll(t => !t.IsRunning);
    }
}
