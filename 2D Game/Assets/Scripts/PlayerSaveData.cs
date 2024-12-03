using System;
using System.Linq;
using Bardent.CoreSystem;
using Inventory;
using Spells;

namespace Krisnat.Assets.Scripts
{
    [Serializable]
    public class PlayerSaveData
    {
        public int level;
        public float maxHealth, currentHealth, maxStam, currentStam, maxMana, currentMana;
        public int strength, dexterity, intelligence;
        public int coins;
        public float[] position;
        public int[] itemsId;
        public int[] equippedItemsId;
        public int[] spellsId;
        public int[] activeSpellsId;
        public int[] activeAbilitiesId;
        public string[] itemsTakenId;
        public bool levelStarted;

        public PlayerSaveData(Player player)
        {
            var levelHandler = player.GetComponent<LevelHandler>();
            var stats = player.Core.GetCoreComponent<Stats>();

            //level = player.PlayerData.PlayerLevel;

            //maxHealth = stats.Health.MaxValue;
            //currentHealth = stats.Health.CurrentValue;

            //maxStam = stats.Stam.MaxValue;
            //currentStam = stats.Stam.CurrentValue;

            //maxMana = stats.Mana.MaxValue;
            //currentMana = stats.Mana.CurrentValue;

            //strength = levelHandler.StrengthCounter;
            //dexterity = levelHandler.DexterityCounter;
            //intelligence = levelHandler.IntelligenceCounter;

            coins = InventoryManager.Instance.Coins;
            
            spellsId = SpellManager.Instance.Spells.Select(spell => spell.id).ToArray();
            activeSpellsId = SpellManager.Instance.SpellsBar.Select(spell => spell.id).ToArray();
            activeAbilitiesId = SpellManager.Instance.AbilitiesBar.Select(spell => spell.id).ToArray();
            itemsId = InventoryManager.Instance.Items.Select(item => item.id).ToArray();
            equippedItemsId = InventoryManager.Instance.EquippedItemsIds();
            itemsTakenId = ItemPickup.itemsTaken.ToArray();
            position = new float[3];

            position[0] = CoreClass.GameManager.checkpoint.x;
            position[1] = CoreClass.GameManager.checkpoint.y;
            position[2] = CoreClass.GameManager.checkpoint.z;
            //position[0] = CoreClass.GameManager.Instance.SpawnPoint.position.x;
            //position[1] = CoreClass.GameManager.Instance.SpawnPoint.position.y;
            //position[2] = CoreClass.GameManager.Instance.SpawnPoint.position.z;
        }
    }
}