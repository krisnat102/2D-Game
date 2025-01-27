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

namespace CoreClass
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public static Vector3 checkpoint;
        private bool gamePaused = false;

        [SerializeField] private GameObject deathScreen;
        [SerializeField] private GameObject playerGO;
        [SerializeField] private Bardent.CoreSystem.Death death;
        [SerializeField] private new Transform camera;
        [SerializeField] private Transform spawnPoint;

        private Player player;
        private LevelHandler levelHandler;
        private InventoryManager inventoryManager;
        private SpellManager spellManager;
        private Stats stats;

        public static bool levelStarted = false;
        public bool GamePaused { get => gamePaused; set => gamePaused = value; }
        public GameObject PlayerGO { get => playerGO; private set => playerGO = value; }
        public Transform SpawnPoint { get => spawnPoint; private set => spawnPoint = value; }
        public List<string> ItemsTaken { get; private set; }
        public List<string> BossesKilled { get; private set; }
        public List<string> BonfiresLit { get; private set; }
        public List<string> Battles { get; private set; }
        public Player Player { get => player; private set => player = value; }

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

            ItemsTaken = new List<string>();
            BossesKilled = new List<string>();
            BonfiresLit = new List<string>();
            Battles = new List<string>();

            Player = playerGO?.GetComponent<Player>();
            levelHandler = CoreClass.GameManager.Instance.GetComponent<LevelHandler>();

            if (checkpoint == Vector3.zero) checkpoint = SpawnPoint.position;
        }

        private void Start()
        {
            inventoryManager = InventoryManager.Instance;
            spellManager = SpellManager.Instance;
            stats = Stats.Instance;

            LoadPlayer();
        }
        #endregion

        #region Player Methods
        public void TryAgain()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

            death.IsDead = false;
        }

        public void SavePlayer()
        {
            SaveSystem.SavePlayer(Player);
        }
        public void LoadPlayer()
        {
            PlayerSaveData data = SaveSystem.LoadPlayer();

            if (data == null) return;

            List<Item> loadItems = new();
            List<Item> loadEquippedItems = new();
            List<Spell> loadSpells = new();
            List<Spell> loadActiveSpells = new();
            List<Spell> loadActiveAbilities = new();
            inventoryManager.ClearInventory();
            spellManager.ClearInventory();
            spellManager.ClearActiveBar();

            foreach (int id in data.itemsId)
            {
                loadItems.AddRange(inventoryManager.AllItems.Where(item => item.id == id).ToList());
            }
            foreach (int id in data.equippedItemsId)
            {
                loadEquippedItems.AddRange(inventoryManager.AllItems.Where(item => item.id == id).ToList());
            }
            foreach (int id in data.spellsId)
            {
                loadSpells.AddRange(spellManager.AllSpells.Where(spell => spell.id == id).ToList());
            }
            foreach (int id in data.activeSpellsId)
            {
                loadActiveSpells.AddRange(spellManager.AllSpells.Where(spell => spell.id == id).ToList());
            }
            foreach (int id in data.activeAbilitiesId)
            {
                loadActiveAbilities.AddRange(spellManager.AllSpells.Where(spell => spell.id == id).ToList());
            }

            if (data.itemsTakenId != null) ItemsTaken = data.itemsTakenId.ToList();
            else ItemsTaken.Clear();

            if (data.bossesKilledId != null) BossesKilled = data.bossesKilledId.ToList();
            else BossesKilled.Clear();

            if (data.bonfiresLitId != null) BonfiresLit = data.bonfiresLitId.ToList();
            else BonfiresLit.Clear();

            if (data.battlesId != null) Battles = data.battlesId.ToList();
            else Battles.Clear();

            //stats.Health.SetCurrentStat(data.currentHealth);
            //stats.Mana.SetCurrentStat(data.currentMana);
            //stats.Stam.SetCurrentStat(data.currentStam);
            player.PlayerData.SetLevel(data.level);
            stats.Health.SetMaxStat(data.maxHealth);
            stats.Mana.SetMaxStat(data.maxMana);
            stats.Stam.SetMaxStat(data.maxStam);
            levelHandler.SetStrength(data.strength);
            levelHandler.SetDexterity(data.dexterity);
            levelHandler.SetIntelligence(data.intelligence);
            inventoryManager.SetCoins(data.coins, false);
            stats.Health.SetCurrentStat(stats.Health.MaxValue);
            stats.Mana.SetCurrentStat(stats.Mana.MaxValue);
            stats.Stam.SetCurrentStat(stats.Stam.MaxValue);
            inventoryManager.SetCoins(data.coins, false);
            inventoryManager.Add(loadItems);
            spellManager.Add(loadSpells);
            MenuManager.Instance.CurrentLevel = data.currentLevel;
            levelStarted = data.levelStarted;

            foreach (int id in inventoryManager.EquippedItemsIds())
            {
                foreach (Item item in inventoryManager.AllItems.Where(item => item.id == id))
                {
                    inventoryManager.UnequipItem(item);
                }
            }
            foreach (Item item in loadEquippedItems)
            {
                inventoryManager.EquipItem(item);
            }

            foreach (Spell spell in loadActiveSpells)
            {
                spellManager.SpellsBar.Add(spell);
            }
            foreach (Spell spell in loadActiveAbilities)
            {
                spellManager.AbilitiesBar.Add(spell);
            }

            var playerTransform = Player.transform;
            Vector3 checkpointPosition = new Vector3(data.position[0], data.position[1], data.position[2]);

            playerTransform.position = checkpointPosition;
            camera.position = checkpointPosition;
            checkpoint = checkpointPosition;
            //checkpoint = spawnPoint.position;

            stats.UpdateStatBars();
        }

        public void LoadPlayer(PlayerSaveData data)
        {
            if (data == null) return;

            List<Item> loadItems = new();
            List<Item> loadEquippedItems = new();
            List<Spell> loadSpells = new();
            List<Spell> loadActiveSpells = new();
            List<Spell> loadActiveAbilities = new();
            inventoryManager.ClearInventory();
            spellManager.ClearInventory();
            spellManager.ClearActiveBar();

            foreach (int id in data.itemsId)
            {
                loadItems.AddRange(inventoryManager.AllItems.Where(item => item.id == id).ToList());
            }
            foreach (int id in data.equippedItemsId)
            {
                loadEquippedItems.AddRange(inventoryManager.AllItems.Where(item => item.id == id).ToList());
            }
            foreach (int id in data.spellsId)
            {
                loadSpells.AddRange(spellManager.AllSpells.Where(spell => spell.id == id).ToList());
            }
            foreach (int id in data.activeSpellsId)
            {
                loadActiveSpells.AddRange(spellManager.AllSpells.Where(spell => spell.id == id).ToList());
            }
            foreach (int id in data.activeAbilitiesId)
            {
                loadActiveAbilities.AddRange(spellManager.AllSpells.Where(spell => spell.id == id).ToList());
            }

            if (data.itemsTakenId != null) ItemsTaken = data.itemsTakenId.ToList();
            else ItemsTaken.Clear();

            if (data.bossesKilledId != null) BossesKilled = data.bossesKilledId.ToList();
            else BossesKilled.Clear();

            if (data.bonfiresLitId != null) BonfiresLit = data.bonfiresLitId.ToList();
            else BonfiresLit.Clear();

            if (data.battlesId != null) Battles = data.battlesId.ToList();
            else Battles.Clear();

            Player.PlayerData.SetLevel(data.level);
            stats.Health.SetMaxStat(data.maxHealth);
            stats.Mana.SetMaxStat(data.maxMana);
            stats.Stam.SetMaxStat(data.maxStam);
            stats.Health.SetCurrentStat(data.currentHealth);
            stats.Mana.SetCurrentStat(data.currentMana);
            stats.Stam.SetCurrentStat(data.currentStam);
            levelHandler.SetStrength(data.strength);
            levelHandler.SetDexterity(data.dexterity);
            levelHandler.SetIntelligence(data.intelligence);
            stats.Health.SetCurrentStat(data.maxHealth);
            stats.Mana.SetCurrentStat(data.maxMana);
            stats.Stam.SetCurrentStat(data.maxStam);
            inventoryManager.SetCoins(data.coins, false);
            inventoryManager.Add(loadItems);
            spellManager.Add(loadSpells);
            MenuManager.Instance.CurrentLevel = data.currentLevel;

            foreach (int id in inventoryManager.EquippedItemsIds())
            {
                foreach (Item item in inventoryManager.AllItems.Where(item => item.id == id))
                {
                    inventoryManager.UnequipItem(item);
                }
            }
            foreach (Item item in loadEquippedItems)
            {
                inventoryManager.EquipItem(item);
            }

            foreach (Spell spell in loadActiveSpells)
            {
                spellManager.SpellsBar.Add(spell);
            }
            foreach (Spell spell in loadActiveAbilities)
            {
                spellManager.AbilitiesBar.Add(spell);
            }

            var playerTransform = Player.transform;

            playerTransform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            camera.position = new Vector3(data.position[0], data.position[1], data.position[2]);

            stats.UpdateStatBars();
        }
        #endregion

        #region General Core Methods
#if UNITY_EDITOR
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
#endif

        /*public List<T> GetCustomAssets<T>(string customAssetType, string location) where T : UnityEngine.Object
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

            return loadedAssets;
        }*/

        public List<T> GetCustomAssets<T>(string customAssetType, string location) where T : UnityEngine.Object
        {
            List<T> loadedAssets = new();

            // Load all assets of type T from the specified Resources subfolder
            string resourcePath = location; // Resources folder path, e.g., "MyAssets/Subfolder"
            T[] assets = Resources.LoadAll<T>(resourcePath);

            if (assets == null || assets.Length == 0)
            {
                Debug.LogWarning(customAssetType + " not found at " + location);
                return loadedAssets;
            }

            foreach (T asset in assets)
            {
                loadedAssets.Add(asset);
            }

            return loadedAssets;
        }

        public T GetComponentOnlyInChildren<T>(Transform transform) where T : Component
        {
            // Iterate through all direct and nested children of this object
            foreach (Transform child in transform)
            {
                // Use GetComponentInChildren on each child, which will ignore the parent (this object)
                T foundComponent = child.GetComponentInChildren<T>(true);
                if (foundComponent != null)
                {
                    return foundComponent;
                }
            }
            return null; // Return null if no component is found in any children
        }

        public void DeactivateObject(float duration, GameObject objectToDeactivate) => StartCoroutine(DeactivateObjectCoroutine(duration, objectToDeactivate));
        public void ChangeBool(float duration, Action<bool[]> boolSetter, bool newValue) => StartCoroutine(ChangeBoolCoroutine(duration, boolSetter, new[] { newValue }));

        #region Core Coroutines
        IEnumerator DeactivateObjectCoroutine(float duration, GameObject obj)
        {
            yield return new WaitForSeconds(duration);
            obj?.gameObject.SetActive(false);
        }

        IEnumerator ChangeBoolCoroutine(float time, Action<bool[]> boolSetter, bool[] newValue)
        {
            yield return new WaitForSeconds(time);
            boolSetter(newValue);
        }
        #endregion
        #endregion
    }
}