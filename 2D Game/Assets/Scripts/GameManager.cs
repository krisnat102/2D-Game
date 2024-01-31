using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bardent.CoreSystem;
using Inventory;
using Krisnat;
using Krisnat.Assets.Scripts;
using Spells;
using UnityEditor;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public bool gamePaused = false;

        [SerializeField] private GameObject deathScreen;
        [SerializeField] private GameObject playerGO;
        [SerializeField] private Bardent.CoreSystem.Death death;
        [SerializeField] private Transform camera;

        private Player player;
        private LevelHandler levelHandler;

        #region Unity Methods
        private void Update()
        {
            if (death.IsDead)
            {
                deathScreen.SetActive(true);
            }
        }
        private void Awake()
        {
            Instance = this;

            player = playerGO.GetComponent<Player>();
            levelHandler = playerGO.GetComponent<LevelHandler>();
        }
        #endregion

        #region Player Methods
        public void TryAgain()
        {
            Application.LoadLevel(Application.loadedLevel);

            death.IsDead = false;

            Stats.Instance.Health.Increase(Stats.Instance.Health.MaxValue - Stats.Instance.Health.CurrentValue);
            Stats.Instance.Mana.Increase(Stats.Instance.Mana.MaxValue - Stats.Instance.Mana.CurrentValue);
            Stats.Instance.Stam.Increase(Stats.Instance.Stam.MaxValue - Stats.Instance.Stam.CurrentValue);

            playerGO.SetActive(true);
        }

        public void SavePlayer()
        {
            SaveSystem.SavePlayer(player);
        }
        public void LoadPlayer()
        {
            PlayerSaveData data = SaveSystem.LoadPlayer();

            List<Item> loadItems = new();
            List<Spell> loadSpells = new();
            InventoryManager.Instance.ClearInventory();
            SpellManager.Instance.ClearInventory();

            foreach (int id in data.itemsId)
            {
                loadItems.AddRange(InventoryManager.Instance.AllItems.Where(item => item.id == id).ToList());
            }
            foreach (int id in data.spellsId)
            {
                loadSpells.AddRange(SpellManager.Instance.AllSpells.Where(spell => spell.id == id).ToList());
            }

            foreach (Item item in loadItems)
            {
                Debug.Log(item.name);
            }

            player.PlayerData.SetLevel(data.level);
            Stats.Instance.Health.SetMaxStat(data.maxHealth);
            Stats.Instance.Mana.SetMaxStat(data.maxMana);
            Stats.Instance.Stam.SetMaxStat(data.maxStam);
            Stats.Instance.Health.SetCurrentStat(data.currentHealth);
            Stats.Instance.Mana.SetCurrentStat(data.currentMana);
            Stats.Instance.Stam.SetCurrentStat(data.currentStam);
            levelHandler.SetStrength(data.strength);
            levelHandler.SetDexterity(data.dexterity);
            levelHandler.SetIntelligence(data.intelligence);
            InventoryManager.Instance.SetCoins(data.coins, false);
            InventoryManager.Instance.Add(loadItems);
            SpellManager.Instance.Add(loadSpells);

            var playerTransform = player.transform;

            playerTransform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            camera.position = new Vector3(data.position[0], data.position[1], data.position[2]);
        }
        #endregion

        #region General Core Methods
        [MenuItem("Tools/Load Custom Assets")]
        private static void LoadItemNames()
        {
            // Define the search filter, for example, search for all ScriptableObject assets
            string searchFilter = "t:Item";

            // Specify the folder in which you want to search for assets (can be "Assets" for the entire project)
            string folderPath = "Assets/CreatedAssets";

            // Use AssetDatabase.FindAssets to get all asset GUIDs that match the search filter in the specified folder
            string[] assetGuids = AssetDatabase.FindAssets(searchFilter, new[] { folderPath });

            // Load the assets based on their paths
            foreach (string assetGuid in assetGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);

                // Load the asset using AssetDatabase.LoadAssetAtPath
                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

                if (asset != null)
                {
                    Debug.Log("Loaded custom asset: " + asset.name + " at path: " + assetPath);
                }
                else
                {
                    Debug.LogWarning("Failed to load asset at path: " + assetPath);
                }
            }
        }

        public List<T> GetCustomAssets<T>(string customAssetType, string location) where T : UnityEngine.Object
        {
            List<T> loadedAssets = new();

            string searchFilter = "t:" + customAssetType;
            string folderPath = "Assets/" + location;

            string[] assetGuids = AssetDatabase.FindAssets(searchFilter, new[] { folderPath });
            if (assetGuids == null) Debug.Log(customAssetType + "not found at " + location);

            foreach (string assetGuid in assetGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);

                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                if (asset != null)
                {
                    loadedAssets.Add(asset);
                }
            }
            /*foreach (T item in loadedAssets)
            {
                Debug.Log(item.name);
            }*/
            return loadedAssets;
        }
        #endregion
    }
}