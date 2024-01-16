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
        private void Awake()
        {
            stats = GetComponentInChildren<Bardent.CoreSystem.Core>().GetComponentInChildren<Stats>();
            player = GetComponent<Player>();

            playerData = player.PlayerData;
        }
        private void Update()
        {
            LevelUpCost = initialLevelUpCost * playerData.PlayerLevel / 2;
            StrengthDamage = 1 + StrengthCounter / 10;
            DexterityDamage = 1 + DexterityCounter / 10;
            IntelligenceDamage = 1 + IntelligenceCounter / 10;
        }
        #endregion

        #region Level Up Buttons
        public void LevelUpStrengthBn()
        {
            if (InventoryManager.Instance.Coins < LevelUpCost) return;

            stats.Health.LevelUpStat(mainStatIncrease);
            StrengthCounter++;

            UniversalStatsIncrease();
        }
        public void LevelUpDexterityBn()
        {
            if (InventoryManager.Instance.Coins < LevelUpCost) return;

            stats.Stam.LevelUpStat(mainStatIncrease);
            DexterityCounter++;

            UniversalStatsIncrease();
        }
        public void LevelUpIntelligenceBn()
        {
            if (InventoryManager.Instance.Coins < LevelUpCost) return;

            stats.Mana.LevelUpStat(mainStatIncrease);
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

        public void SetStrength(int strength) => StrengthCounter = strength;
        public void SetDexterity(int dexterity) => DexterityCounter = dexterity;
        public void SetIntelligence(int intelligence) => IntelligenceCounter = intelligence;
        #endregion
    }
}
