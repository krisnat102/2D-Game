using System;
using System.Linq;
using Bardent.CoreSystem;
using Inventory;
using Spells;
using UnityEngine;

namespace Krisnat.Assets.Scripts
{
    [Serializable]
    public class PlayerSaveData
    {
        public int level;
        public float maxHealth, currentHealth, maxStam, currentStam, maxMana, currentMana;
        public int strength, dexterity, intelligence;
        public int coins;
        public int currentLevel;
        public float[] position;
        public int[] itemsId, equippedItemsId, spellsId;
        public int[] activeSpellsId, activeAbilitiesId;
        public string[] itemsTakenId, bossesKilledId, bonfiresLitId, battlesId;
        public bool levelStarted;

        public PlayerSaveData(Player player)
        {
            var levelHandler = CoreClass.GameManager.instance.gameObject.GetComponent<LevelHandler>();
            var stats = player.Core.GetCoreComponent<Stats>();

            level = player.PlayerData.PlayerLevel;

            maxHealth = stats.health.MaxValue;
            //currentHealth = stats.Health.CurrentValue;

            maxStam = stats.stam.MaxValue;
            //currentStam = stats.Stam.CurrentValue;

            maxMana = stats.mana.MaxValue;
            //currentMana = stats.Mana.CurrentValue;

            strength = levelHandler.StrengthCounter;
            dexterity = levelHandler.DexterityCounter;
            intelligence = levelHandler.IntelligenceCounter;

            coins = InventoryManager.Instance.Coins;

            spellsId = SpellManager.Instance.Spells.Select(spell => spell.id).ToArray();
            activeSpellsId = SpellManager.Instance.SpellsBar.Select(spell => spell.id).ToArray();
            activeAbilitiesId = SpellManager.Instance.AbilitiesBar.Select(spell => spell.id).ToArray();
            itemsId = InventoryManager.Instance.Items.Select(item => item.id).ToArray();
            equippedItemsId = InventoryManager.Instance.EquippedItemsIds();
            itemsTakenId = CoreClass.GameManager.instance.ItemsTaken.ToArray();
            bossesKilledId = CoreClass.GameManager.instance.BossesKilled.ToArray();
            bonfiresLitId = CoreClass.GameManager.instance.BonfiresLit.ToArray();
            battlesId = CoreClass.GameManager.instance.Battles.ToArray();
            currentLevel = MenuManager.instance.CurrentLevel;
            position = new float[3];

            position[0] = CoreClass.GameManager.instance.Checkpoint.x;
            position[1] = CoreClass.GameManager.instance.Checkpoint.y;
            position[2] = CoreClass.GameManager.instance.Checkpoint.z;
            //position[0] = CoreClass.GameManager.Instance.SpawnPoint.position.x;
            //position[1] = CoreClass.GameManager.Instance.SpawnPoint.position.y;
            //position[2] = CoreClass.GameManager.Instance.SpawnPoint.position.z;

            levelStarted = true;
        }

        public PlayerSaveData(
            int level,
            float maxHealth,
            float currentHealth,
            float maxStam,
            float currentStam,
            float maxMana,
            float currentMana,
            int strength,
            int dexterity,
            int intelligence,
            int coins,
            int currentLevel,
            float[] position,
            int[] itemsId,
            int[] equippedItemsId,
            int[] spellsId,
            int[] activeSpellsId,
            int[] activeAbilitiesId,
            string[] itemsTakenId,
            string[] bossesKilledId,
            string[] bonfiresLitId,
            string[] battlesId,
            bool levelStarted)
        {
            this.level = level;
            this.maxHealth = maxHealth;
            this.currentHealth = currentHealth;
            this.maxStam = maxStam;
            this.currentStam = currentStam;
            this.maxMana = maxMana;
            this.currentMana = currentMana;
            this.strength = strength;
            this.dexterity = dexterity;
            this.intelligence = intelligence;
            this.coins = coins;
            this.currentLevel = currentLevel;
            this.position = position;
            this.itemsId = itemsId;
            this.equippedItemsId = equippedItemsId;
            this.spellsId = spellsId;
            this.activeSpellsId = activeSpellsId;
            this.activeAbilitiesId = activeAbilitiesId;
            this.itemsTakenId = itemsTakenId;
            this.bossesKilledId = bossesKilledId;
            this.bonfiresLitId = bonfiresLitId;
            this.battlesId = battlesId;
            this.levelStarted = levelStarted;
        }

        public static PlayerSaveData CreateDefault()
        {
            float[] spawnPosition;
            if (CoreClass.GameManager.instance)
            {
                spawnPosition = new float[]
                {
                    CoreClass.GameManager.instance.SpawnPoint.position.x,
                    CoreClass.GameManager.instance.SpawnPoint.position.y,
                    CoreClass.GameManager.instance.SpawnPoint.position.z
                };
            }
            else
            {
                spawnPosition = new float[]
                {
                    5,
                    -2.3f,
                    0f
                };
            }
            return new PlayerSaveData(
                level: 1,
                maxHealth: 100f,
                currentHealth: 100f,
                maxStam: 100f,
                currentStam: 100f,
                maxMana: 100f,
                currentMana: 100f,
                strength: 0,
                dexterity: 0,
                intelligence: 0,
                coins: 0,
                currentLevel: 1,
                position: spawnPosition,
                itemsId: new int[] { 104, 106 },
                equippedItemsId: new int[] { 104, 106 },
                //itemsId: Array.Empty<int>(),
                //equippedItemsId: Array.Empty<int>(),
                spellsId: Array.Empty<int>(),
                activeSpellsId: Array.Empty<int>(),
                activeAbilitiesId: Array.Empty<int>(),
                itemsTakenId: Array.Empty<string>(),
                bossesKilledId: Array.Empty<string>(),
                bonfiresLitId: Array.Empty<string>(),
                battlesId: Array.Empty<string>(),
                levelStarted: false
            );
        }
    }
}