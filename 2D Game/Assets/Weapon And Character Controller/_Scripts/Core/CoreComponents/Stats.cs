using Bardent.CoreSystem.StatsSystem;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Inventory;
using Krisnat;

namespace Bardent.CoreSystem
{
    public class Stats : CoreComponent
    {
        public static Stats instance;

        [field: SerializeField] public Stat health { get; set; }
        [field: SerializeField] public Stat poise { get; private set; }
        [field: SerializeField] public Stat stam { get; private set; }
        [field: SerializeField] public Stat mana { get; set; }
        public bool LowHP { get => lowHP; private set => lowHP = value; }

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
        private GameObject hpHandle, manaHandle, stamHandle;

        protected override void Awake()
        {
            base.Awake();
            instance = this;

            stats.AddRange(new List<Stat>
            {
                health, poise, stam, mana
            });

            foreach (Stat stat in stats)
            {
                stat.Init();
            }
        }

        private void Start()
        {
            if (health.CurrentValue >= 20)
            {
                Krisnat.VignetteController.instance.SetVignette(0);
            }

            if (!hpBar || !manaBar || !stamBar || !easeHpBar) return;
            hpBar.maxValue = health.MaxValue;
            manaBar.maxValue = mana.MaxValue;
            stamBar.maxValue = stam.MaxValue;
            easeHpBar.maxValue = hpBar.maxValue;
            easeManapBar.maxValue = manaBar.maxValue;
            easeStamBar.maxValue = stamBar.maxValue;
            hpHandle = hpBar.GetComponentInChildren<SearchAssist>().gameObject;
            manaHandle = manaBar.GetComponentInChildren<SearchAssist>().gameObject;
            stamHandle = stamBar.GetComponentInChildren<SearchAssist>().gameObject;
        }

        private void Update()
        {
            foreach (Stat stat in stats)
            {
                stat.Regen();
            }

            if (health.CurrentValue < 20 && !LowHP)
            {
                LowHP = true;
                Invoke("StartHpRegen", 5f);
                AudioManager.instance.PlayHeartbeatSound(0.8f, 0.8f);
                Krisnat.VignetteController.instance.ChangeVignette(0.5f, 2);
            }
            else if(health.CurrentValue >= 20 && LowHP)
            {
                LowHP = false;
                health.StopRegen();
                AudioManager.instance.StopHeartbeatSound();
                Krisnat.VignetteController.instance.ChangeVignette(0, 1);
            }

            if (hpBar == null || manaBar == null || stamBar == null) return;

            hpBar.value = health.CurrentValue;
            manaBar.value = mana.CurrentValue;
            stamBar.value = stam.CurrentValue;

            if (hpBar.value != easeHpBar.value)
            {
                easeHpBar.value = Mathf.Lerp(easeHpBar.value, hpBar.value - 2, easeHpSpeed);
            }
            if (manaBar.value != easeManapBar.value)
            {
                easeManapBar.value = Mathf.Lerp(easeManapBar.value, manaBar.value - 2, easeManaSpeed);
            }
            if (stamBar.value != easeStamBar.value)
            {
                easeStamBar.value = Mathf.Lerp(easeStamBar.value, stamBar.value - 2, easeStamSpeed);
            }

            if (hpBar.value < hpBar.maxValue / 100) hpHandle.SetActive(false);
            else hpHandle.SetActive(true);

            if (manaBar.value < manaBar.maxValue / 100) manaHandle.SetActive(false);
            else manaHandle.SetActive(true);

            if (stamBar.value < stamBar.maxValue / 100) stamHandle.SetActive(false);
            else stamHandle.SetActive(true);
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
