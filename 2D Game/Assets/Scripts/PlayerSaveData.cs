using System;
using System.Collections.Generic;
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
        public int[] spellsId;

        public PlayerSaveData(Player player)
        {
            var levelHandler = player.GetComponent<LevelHandler>();
            var stats = player.Core.GetCoreComponent<Stats>();

            level = player.PlayerData.PlayerLevel;

            maxHealth = stats.Health.MaxValue;
            currentHealth = stats.Health.CurrentValue;

            maxStam = stats.Stam.MaxValue;
            currentStam = stats.Stam.CurrentValue;

            maxMana = stats.Mana.MaxValue;
            currentMana = stats.Mana.CurrentValue;

            strength = levelHandler.StrengthCounter;
            dexterity = levelHandler.DexterityCounter;
            intelligence = levelHandler.IntelligenceCounter;

            coins = InventoryManager.Instance.Coins;
            List<Spell> allSpellsArray = SpellManager.Instance.AllSpells;
            List<Item> allItemsArray = InventoryManager.Instance.AllItems;

            spellsId = SpellManager.Instance.Spells.Select(spell => spell.id).ToArray();
            itemsId = InventoryManager.Instance.Items.Select(item => item.id).ToArray();

            position = new float[3];

            position[0] = player.transform.position.x;
            position[1] = player.transform.position.y;
            position[2] = player.transform.position.z;

            foreach (int i in itemsId)
            {
                UnityEngine.Debug.Log(i);
            }
        }
    }
}