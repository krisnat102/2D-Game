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

        [field: SerializeField] public Stat Health { get; private set; }
        [field: SerializeField] public Stat Poise { get; private set; }
        [field: SerializeField] public Stat Stam { get; private set; }
        [field: SerializeField] public Stat Mana { get; private set; }
        List<Stat> stats = new();

        [Header("UI")]
        [SerializeField] private Slider HpBar;
        [SerializeField] private Slider ManaBar;
        [SerializeField] private Slider StamBar;

        protected override void Awake()
        {
            base.Awake();
            Instance = this;

            stats.AddRange(new List<Stat>
            {
                Health, Poise, Stam, Mana
            });

            foreach (Stat stat in stats)
            {
                stat.Init();
            }

            if (HpBar == null || ManaBar == null || StamBar == null) return;
            HpBar.maxValue = Health.MaxValue;
            ManaBar.maxValue = Mana.MaxValue;
            StamBar.maxValue = Stam.MaxValue;
        }


        private void Update()
        {
            foreach (Stat stat in stats)
            {
                stat.Regen();
            }

            if (HpBar == null || ManaBar == null || StamBar == null) return;
            HpBar.value = Health.CurrentValue;
            ManaBar.value = Mana.CurrentValue;
            StamBar.value = Stam.CurrentValue;
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
            if (HpBar == null || ManaBar == null || StamBar == null) return;
            HpBar.maxValue = Health.MaxValue;
            ManaBar.maxValue = Mana.MaxValue;
            StamBar.maxValue = Stam.MaxValue;
        }
    }
}
