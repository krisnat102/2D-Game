using Bardent.CoreSystem;
using Inventory;
using System;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

namespace Krisnat
{
    public class LevelHandler : MonoBehaviour
    {
        #region Private Variables
        [SerializeField] private float levelUpModifier = 1.5f;
        [SerializeField] private float mainStatIncrease;
        [SerializeField] private float subStatsIncrease;

        private float strengthDamage, dexterityDamage, intelligenceDamage = 1;
        private int levelUpCost;
        private Stats stats;
        private Player player;
        private PlayerData playerData;
        private int strengthCounter = 1;
        private int dexterityCounter = 1;
        private int intelligenceCounter = 1;
        #endregion 

        #region Method Variables
        public float StrengthDamage { get => strengthDamage; private set => strengthDamage = value; }
        public float DexterityDamage { get => dexterityDamage; private set => dexterityDamage = value; }
        public float IntelligenceDamage { get => intelligenceDamage; private set => intelligenceDamage = value; }
        public int LevelUpCost { get => levelUpCost; private set => levelUpCost = value; }
        public int StrengthCounter { get => strengthCounter; private set => strengthCounter = value; }
        public int DexterityCounter { get => dexterityCounter; private set => dexterityCounter = value; }
        public int IntelligenceCounter { get => intelligenceCounter; private set => intelligenceCounter = value; }
        #endregion

        #region Unity Methods
        private void Start()
        {
            try
            {
                stats = Stats.instance;
                player = stats?.GetComponentInParent<Player>();

                playerData = player?.PlayerData;

                if (playerData) LevelUpCost = CalculateLevelUpCost(playerData.PlayerLevel);
            }
            catch
            {
            }

            //for testing
            //playerData.SetLevel(1);
            //InventoryManager.Instance.SetCoins(10000, false);
        }
        private void Update()
        {
            StrengthDamage = 1 + StrengthCounter / 10f;
            DexterityDamage = 1 + DexterityCounter / 10f;
            IntelligenceDamage = 1 + IntelligenceCounter / 10f;

            //Debug.Log(StrengthDamage);
        }
        #endregion

        #region Level Up Buttons
        public void LevelUpStrengthBn()
        {
            if (InventoryManager.Instance.Coins < CalculateLevelUpCost(playerData.PlayerLevel + 1)) return;
            stats.health.LevelUpStat(mainStatIncrease);
            StrengthCounter++;

            UniversalStatsIncrease();
        }
        public void LevelUpDexterityBn()
        {
            if (InventoryManager.Instance.Coins < CalculateLevelUpCost(playerData.PlayerLevel + 1)) return;
            stats.stam.LevelUpStat(mainStatIncrease);
            DexterityCounter++;

            UniversalStatsIncrease();
        }
        public void LevelUpIntelligenceBn()
        {
            if (InventoryManager.Instance.Coins < CalculateLevelUpCost(playerData.PlayerLevel + 1)) return;
            stats.mana.LevelUpStat(mainStatIncrease);
            IntelligenceCounter++;

            UniversalStatsIncrease();
        }
        #endregion

        #region Methods
        private void UniversalStatsIncrease()
        {
            stats.health.LevelUpStat(subStatsIncrease);
            stats.stam.LevelUpStat(subStatsIncrease);
            stats.mana.LevelUpStat(subStatsIncrease);

            playerData.LevelUp();
            LevelUpCost = CalculateLevelUpCost(playerData.PlayerLevel);
            InventoryManager.Instance.SetCoins(InventoryManager.Instance.Coins - LevelUpCost, false);
            Stats.instance.UpdateStatBars();
            UIManager.instance.UpdateLevelUpUI();
        }

        public int CalculateLevelUpCost(int level) => (int)((Mathf.Floor(0.02f * Mathf.Pow(level, 3) + 3.06f * Mathf.Pow(level, 2))) * levelUpModifier);

        public void SetStrength(int strength) => StrengthCounter = strength;
        public void SetDexterity(int dexterity) => DexterityCounter = dexterity;
        public void SetIntelligence(int intelligence) => IntelligenceCounter = intelligence;
        #endregion
    }
}
