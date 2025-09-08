using System.Collections.Generic;
using UnityEngine;

namespace Snog.Timers
{
    public class Timer
    {
        public float Duration { get; private set; }
        public float TimeRemaining { get; private set; }
        public bool IsRunning { get; private set; }

        public System.Action OnTimerEnd;

        public Timer(float duration, System.Action onTimerEnd = null)
        {
            Duration = duration;
            TimeRemaining = duration;
            IsRunning = true;
            OnTimerEnd = onTimerEnd;
        }

        private void Update()
        {
            UpdateTimer(Time.deltaTime);
        }

        public void UpdateTimer(float deltaTime)
        {
            if (!IsRunning) return;

            TimeRemaining -= deltaTime;
            if (TimeRemaining <= 0f)
            {
                IsRunning = false;
                TimeRemaining = 0f;
                // OnTimerEnd?.Invoke(); // Call the event if it's set
            }
        }

        public void Reset()
        {
            TimeRemaining = Duration;
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }
    }
}
