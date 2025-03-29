using Bardent.CoreSystem.StatsSystem;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Inventory;

namespace Bardent.CoreSystem
{
    public class Stats : CoreComponent
    {
        public static Stats Instance;

        [field: SerializeField] public Stat health { get; private set; }
        [field: SerializeField] public Stat poise { get; private set; }
        [field: SerializeField] public Stat stam { get; private set; }
        [field: SerializeField] public Stat mana { get; private set; }
        List<Stat> stats = new();

        [Header("UI")]
        [Header("HP")]
        [SerializeField] private Slider hpBar;
        [SerializeField] private Slider easeHpBar;
        [SerializeField] private float easeHpSpeed;
        [Header("Mana")]
        [SerializeField] private Slider manaBar;
        [SerializeField] private Slider easeManapBar;
        [SerializeField] private float easeManaSpeed;
        [Header("Stam")]
        [SerializeField] private Slider stamBar;
        [SerializeField] private Slider easeStamBar;
        [SerializeField] private float easeStamSpeed;

        private bool lowHP;

        protected override void Awake()
        {
            base.Awake();
            Instance = this;

            stats.AddRange(new List<Stat>
            {
                health, poise, stam, mana
            });

            foreach (Stat stat in stats)
            {
                stat.Init();
            }

            if (health.CurrentValue >= 20)
            {
                Krisnat.VignetteController.instance.SetVignette(0);
            }
        }

        private void Start()
        {
            if (!hpBar || !manaBar || !stamBar || !easeHpBar) return;
            hpBar.maxValue = health.MaxValue;
            manaBar.maxValue = mana.MaxValue;
            stamBar.maxValue = stam.MaxValue;
            easeHpBar.maxValue = hpBar.maxValue;
            easeManapBar.maxValue = manaBar.maxValue;
            easeStamBar.maxValue = stamBar.maxValue;
        }

        private void Update()
        {
            foreach (Stat stat in stats)
            {
                stat.Regen();
            }

            if (health.CurrentValue < 20 && !lowHP)
            {
                lowHP = true;
                Invoke("StartHpRegen", 5f);
                AudioManager.Instance.PlayHeartbeatSound(0.8f, 0.8f);
                Krisnat.VignetteController.instance.ChangeVignette(0.5f, 2);
            }
            else if(health.CurrentValue >= 20 && lowHP)
            {
                lowHP = false;
                health.StopRegen();
                AudioManager.Instance.StopHeartbeatSound();
                Krisnat.VignetteController.instance.ChangeVignette(0, 1);
            }

            if (hpBar == null || manaBar == null || stamBar == null) return;

            hpBar.value = health.CurrentValue;
            manaBar.value = mana.CurrentValue;
            stamBar.value = stam.CurrentValue;

            if (hpBar.value != easeHpBar.value)
            {
                easeHpBar.value = Mathf.Lerp(easeHpBar.value, hpBar.value, easeHpSpeed);
            }
            if (manaBar.value != easeManapBar.value)
            {
                easeManapBar.value = Mathf.Lerp(easeManapBar.value, manaBar.value, easeManaSpeed);
            }
            if (stamBar.value != easeStamBar.value)
            {
                easeStamBar.value = Mathf.Lerp(easeStamBar.value, stamBar.value, easeStamSpeed);
            }
        }

        public float CalculatePhysicalDamageReduction(float damage)
        {
            float defense = InventoryManager.Instance.TotalArmor;
            float finalDamage = damage / Mathf.Pow(2, defense / damage);

            return Mathf.Round(finalDamage);
        }
        public float CalculateMagicalDamageReduction(float damage)
        {
            float defense = InventoryManager.Instance.TotalMagicRes;
            float finalDamage = damage / Mathf.Pow(2, defense / damage);

            return Mathf.Round(finalDamage);
        }

        public void UpdateStatBars()
        {
            if (hpBar == null || manaBar == null || stamBar == null) return;
            hpBar.maxValue = health.MaxValue;
            manaBar.maxValue = mana.MaxValue;
            stamBar.maxValue = stam.MaxValue;
        }

        private void StartHpRegen() => health.StartRegen();
    }
}
