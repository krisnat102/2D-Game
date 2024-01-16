using Bardent.CoreSystem;
using Inventory;
using Krisnat;
using Krisnat.Assets.Scripts;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static bool gamePaused = false;

        [SerializeField] private GameObject deathScreen;
        [SerializeField] private GameObject playerGO;
        [SerializeField] private Bardent.CoreSystem.Death death;

        private Player player;
        private LevelHandler levelHandler;
        private Transform camera;

        private void Update()
        {
            if (death.IsDead == true)
            {
                deathScreen.SetActive(true);
            }
        }
        private void Awake() {
            player = playerGO.GetComponent<Player>();
            levelHandler = playerGO.GetComponent<LevelHandler>();
            camera = transform.Find("Camera");
        }

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

            var playerTransform = player.transform;
            
            playerTransform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            camera.position = new Vector3(data.position[0], data.position[1], data.position[2]);
        }
    }
}