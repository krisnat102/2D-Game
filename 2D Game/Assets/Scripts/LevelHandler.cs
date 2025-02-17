using Bardent.CoreSystem;
using Inventory;
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
            stats = Stats.Instance;
            player = Stats.Instance?.GetComponentInParent<Player>();

            playerData = player?.PlayerData;

            //for testing
            //playerData.SetLevel(1);
            if(playerData) LevelUpCost = CalculateLevelUpCost(playerData.PlayerLevel);

            //for testing
            //InventoryManager.Instance.SetCoins(10000, false);
        }
        private void Update()
        {
            StrengthDamage = 1 + StrengthCounter / 10f;
            DexterityDamage = 1 + DexterityCounter / 10f;
            IntelligenceDamage = 1 + IntelligenceCounter / 10f;
        }
        #endregion

        #region Level Up Buttons
        public void LevelUpStrengthBn()
        {
            if (InventoryManager.Instance.Coins < CalculateLevelUpCost(playerData.PlayerLevel + 1)) return;
            stats.Health.LevelUpStat(mainStatIncrease);
            StrengthCounter++;

            UniversalStatsIncrease();
        }
        public void LevelUpDexterityBn()
        {
            if (InventoryManager.Instance.Coins < CalculateLevelUpCost(playerData.PlayerLevel + 1)) return;
            stats.Stam.LevelUpStat(mainStatIncrease);
            DexterityCounter++;

            UniversalStatsIncrease();
        }
        public void LevelUpIntelligenceBn()
        {
            if (InventoryManager.Instance.Coins < CalculateLevelUpCost(playerData.PlayerLevel + 1)) return;
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
            LevelUpCost = CalculateLevelUpCost(playerData.PlayerLevel);
            InventoryManager.Instance.SetCoins(InventoryManager.Instance.Coins - LevelUpCost, false);
            Stats.Instance.UpdateStatBars();
            UIManager.Instance.UpdateLevelUpUI();
        }

        public int CalculateLevelUpCost(int level) => (int)((Mathf.Floor(0.02f * Mathf.Pow(level, 3) + 3.06f * Mathf.Pow(level, 2))) * levelUpModifier);

        public void SetStrength(int strength) => StrengthCounter = strength;
        public void SetDexterity(int dexterity) => DexterityCounter = dexterity;
        public void SetIntelligence(int intelligence) => IntelligenceCounter = intelligence;
        #endregion
    }
}
