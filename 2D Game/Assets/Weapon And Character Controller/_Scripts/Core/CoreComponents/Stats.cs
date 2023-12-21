using System;
using Bardent.CoreSystem.StatsSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Bardent.CoreSystem
{
    public class Stats : CoreComponent
    {
        [field: SerializeField] public Stat Health { get; private set; }
        [field: SerializeField] public Stat Poise { get; private set; }
        [field: SerializeField] public Stat Stam { get; private set; }
        [field: SerializeField] public Stat Mana { get; private set; }

        //[SerializeField] protected float poiseRecoveryRate;
        //[SerializeField] protected float stamRecoveryRate;

        [Header("UI")]
        [SerializeField] private Slider HpBar;
        [SerializeField] private Slider ManaBar;
        [SerializeField] private Slider StamBar;

        protected override void Awake()
        {
            base.Awake();

            Health.Init();
            Poise.Init();
            Stam.Init();
            Mana.Init();

            if (HpBar == null || ManaBar == null || StamBar == null) return;
            HpBar.maxValue = Health.CurrentValue;
            ManaBar.maxValue = Mana.CurrentValue;
            StamBar.maxValue = Stam.CurrentValue;
        }


        private void Update()
        {
            /*TODO: Put the regenegerion in the stat class and add a bool for whenever if a stat should regen over time or not. 
            if it does, reveal the recoveryRate variable in the inspector 

            if (!Poise.CurrentValue.Equals(Poise.MaxValue))
            {
                Poise.Increase(poiseRecoveryRate * Time.deltaTime);
            }

            if (!Stam.CurrentValue.Equals(Stam.MaxValue) && StamRegen)
            {
                Stam.Increase(stamRecoveryRate * Time.deltaTime);
            }*/
            Poise.Regen();
            Stam.Regen();

            if (HpBar == null || ManaBar == null || StamBar == null) return;
            HpBar.value = Health.CurrentValue;
            ManaBar.value = Mana.CurrentValue;
            StamBar.value = Stam.CurrentValue;
        }
    }
}
