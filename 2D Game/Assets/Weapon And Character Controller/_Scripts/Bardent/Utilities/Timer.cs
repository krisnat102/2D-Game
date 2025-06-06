using System;
using UnityEngine;

namespace Bardent.Utilities
{
    public class Timer
    {
        public event Action OnTimerDone;

        private float startTime;
        private float duration;
        private float targetTime;

        private bool isActive;

        public bool Finished { get; private set; }


        public Timer(float duration)
        {
            this.duration = duration;
        }

        public void StartTimer()
        {
            startTime = Time.time;
            targetTime = startTime + duration;
            isActive = true;
            Finished = false;
        }

        public void StopTimer()
        {
            isActive = false;
        }

        public void Tick()
        {
            if (!isActive) return;

            if (Time.time >= targetTime)
            {
                OnTimerDone?.Invoke();
                StopTimer();
                Finished = true;
            }
        }
    }
}