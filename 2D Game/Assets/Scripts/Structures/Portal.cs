using Inventory;
using Krisnat.Assets.Scripts;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Krisnat
{
    public class Portal : MonoBehaviour, IStructurable
    {
        [SerializeField] private bool loadNextScene;
        [SerializeField] private string sceneToLoad;
        [SerializeField] private Vector3 distanceTravel;
        [SerializeField] private Item key;
        [SerializeField] private GameObject uiPopUp;
        [SerializeField] private bool oneTimeUse;

        private LevelLoader levelLoader;
        private ChallengeRoom challengeRoom;
        private bool locked = true;
        private int buildIndexToLoad = 128;

        public string PortalId { get; private set; }

        private void Start()
        {
            levelLoader = GetComponent<LevelLoader>();
            challengeRoom = GetComponent<ChallengeRoom>();

            if(int.TryParse(sceneToLoad, out _)){
                buildIndexToLoad = int.Parse(sceneToLoad);
            }

            if (oneTimeUse)
            {
                if (string.IsNullOrEmpty(PortalId)) PortalId = gameObject.name;

                PlayerSaveData data = SaveSystem.LoadPlayer();
                if (data != null && data.portalsId != null && data.portalsId.Contains(PortalId))
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            var player = collision.GetComponent<Player>();

            if (PlayerInputHandler.Instance.UseInput && player)
            {
                Teleport(player);
            }
        }

        private void Teleport(Player player)
        {
            PlayerInputHandler.Instance.UseUseInput();

            if (challengeRoom)
            {
                CoreClass.GameManager.instance.ActiveChallengeRoom = challengeRoom;
                challengeRoom.EnterRoom(player);
            }

            if (locked && key)
            {
                uiPopUp?.SetActive(true);

                if (!InventoryManager.Instance.Items.Contains(key))
                {
                    uiPopUp.GetComponentInChildren<TMP_Text>().text = key.itemName + " Needed";
                    return;
                }

                //InventoryManager.Instance.Remove(key);
                uiPopUp.GetComponentInChildren<TMP_Text>().text = key.itemName + " Used";
                locked = false;
            }

            if (oneTimeUse)
            {
                CoreClass.GameManager.instance.TempPortalsUsed.Add(PortalId);
                gameObject.SetActive(false);
            }

            if (sceneToLoad == "" && distanceTravel != Vector3.zero)
            {
                player.transform.position += distanceTravel;

                return;
            }
            CoreClass.GameManager.instance.Checkpoint = CoreClass.GameManager.instance.SpawnPoint.position;
            SaveSystem.SavePlayer(player);

            if (loadNextScene)
            {
                if (levelLoader) levelLoader.LoadNextLevel();
                else UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);

                return;
            }

            if (levelLoader)
            {
                if (buildIndexToLoad != 128) levelLoader.LoadLevel(buildIndexToLoad);
                else levelLoader.LoadLevel(sceneToLoad);
            }
            else
            {
                if (buildIndexToLoad != 128) UnityEngine.SceneManagement.SceneManager.LoadScene(buildIndexToLoad);
                else UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
