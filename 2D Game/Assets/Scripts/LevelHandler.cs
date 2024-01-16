using Bardent.CoreSystem;
using Inventory;
using UnityEngine;

namespace Krisnat
{
    public class LevelHandler : MonoBehaviour
    {
        #region Private Variables
        [SerializeField] private int initialLevelUpCost;
        [SerializeField] private float mainStatIncrease;
        [SerializeField] private float subStatsIncrease;

        private float strengthDamage, dexterityDamage, intelligenceDamage = 1;
        private int levelUpCost;
        private Stats stats;
        private Player player;
        private PlayerData playerData;
        private int strengthCounter, dexterityCounter, intelligenceCounter = 1;
        #endregion

        #region Method Variables
        public float StrengthDamage { get => strengthDamage; set => strengthDamage = value; }
        public float DexterityDamage { get => dexterityDamage; set => dexterityDamage = value; }
        public float IntelligenceDamage { get => intelligenceDamage; set => intelligenceDamage = value; }
        public int LevelUpCost { get => levelUpCost; set => levelUpCost = value; }
        public int StrengthCounter { get => strengthCounter; set => strengthCounter = value; }
        public int DexterityCounter { get => dexterityCounter; set => dexterityCounter = value; }
        public int IntelligenceCounter { get => intelligenceCounter; set => intelligenceCounter = value; }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            stats = GetComponentInChildren<Bardent.CoreSystem.Core>().GetComponentInChildren<Stats>();
            player = GetComponent<Player>();

            playerData = player.PlayerData;
        }
        private void Update()
        {
            LevelUpCost = initialLevelUpCost * playerData.PlayerLevel / 2;
        }
        #endregion

        #region Level Up Buttons
        public void LevelUpStrengthBn()
        {
            if (InventoryManager.Instance.Coins < LevelUpCost) return;

            stats.Health.LevelUpStat(mainStatIncrease);
            StrengthDamage += mainStatIncrease / 50;
            StrengthCounter++;

            UniversalStatsIncrease();
        }
        public void LevelUpDexterityBn()
        {
            if (InventoryManager.Instance.Coins < LevelUpCost) return;

            stats.Stam.LevelUpStat(mainStatIncrease);
            DexterityDamage += mainStatIncrease / 50;
            DexterityCounter++;

            UniversalStatsIncrease();
        }
        public void LevelUpIntelligenceBn()
        {
            if (InventoryManager.Instance.Coins < LevelUpCost) return;

            stats.Mana.LevelUpStat(mainStatIncrease);
            IntelligenceDamage += mainStatIncrease / 50;
            IntelligenceCounter++;

            UniversalStatsIncrease();
        }
        #endregion

        #region Methods
        private void UniversalStatsIncrease()
        {
            stats.Health.LevelUpStat(subStatsIncrease);
            stats.Stam.LevelUpStat(subStatsIncrease);
            stats.Mana.LevelUpStat(subStatsIncrease);

            playerData.LevelUp();
            InventoryManager.Instance.SetCoins(InventoryManager.Instance.Coins - LevelUpCost, false);
        }
        #endregion
    }
}
