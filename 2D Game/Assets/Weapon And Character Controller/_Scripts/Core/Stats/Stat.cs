using System;
using UnityEngine;

namespace Bardent.CoreSystem.StatsSystem
{
    [Serializable]
    public class Stat
    {
        public event Action OnCurrentValueZero;

        [field: SerializeField] public float MaxValue { get; private set; }
        [field: SerializeField] public bool Regenerative { get; private set; }
        [field: SerializeField] public float recoveryRate;
        [field: SerializeField] private float recoveryTransitionSpeed = 1f;
        private float recoveryMultiplier = 1f;

        public float CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = Mathf.Clamp(value, 0f, MaxValue);

                if (currentValue <= 0f)
                {
                    OnCurrentValueZero?.Invoke();
                }
            }
        }

        private float stopRegenTime;
        private float currentValue;

        public void Init() => CurrentValue = MaxValue;

        public void Increase(float amount) => CurrentValue += amount;
        public void Decrease(float amount) => CurrentValue -= amount;

        public void Regen()
        {
            if (!Regenerative || Time.time < stopRegenTime) return;

            recoveryMultiplier = Mathf.MoveTowards(recoveryMultiplier, 1f, recoveryTransitionSpeed * Time.deltaTime);

            Increase(recoveryRate * recoveryMultiplier * Time.deltaTime);
        }

        public void StopRegen(float time)
        {
            if (Regenerative)
            {
                stopRegenTime = Time.time + time;
                recoveryMultiplier = 0.1f;
            }
        }

        public void StopRegen() => Regenerative = false;
        public void StartRegen() => Regenerative = true;

        public void LevelUpStat(float levelUpValue) => MaxValue += levelUpValue;

        public void SetMaxStat(float maxStat) => MaxValue = maxStat;
        public void SetCurrentStat(float currentStat) => CurrentValue = currentStat;
    }
}