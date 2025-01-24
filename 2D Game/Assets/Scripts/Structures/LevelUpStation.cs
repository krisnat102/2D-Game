using Krisnat.Assets.Scripts;
using System.Linq;
using UnityEngine;

namespace Krisnat
{
    public class LevelUpStation : MonoBehaviour, IStructurable
    {
        [SerializeField] private float openingAnimationDuration = 0.2f;
        [SerializeField] private float closingAnimationDuration = 0.2f;
        [SerializeField] private string bonfireId;
        [SerializeField] private GameObject fire;
        [SerializeField] private Transform spawnPoint;
        private bool triggered;

        public string BonfireId { get => bonfireId; private set => bonfireId = value; }

        private void Start()
        {
            if (string.IsNullOrEmpty(BonfireId)) BonfireId = gameObject.name;

            PlayerSaveData data = SaveSystem.LoadPlayer();
            if (data != null && data.bonfiresLitId != null && data.bonfiresLitId.Contains(BonfireId))
            {
                triggered = true;
                fire.SetActive(true);
            }
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            var player = collision.GetComponent<Player>();
            var levelUpUI = UIManager.Instance.LevelUpInterface;
            var scale = levelUpUI.transform.localScale.x;

            if (PlayerInputHandler.Instance.UseInput && player != null)
            {
                if (triggered)
                {
                    if (!UIManager.Instance.LevelUpInterface.activeInHierarchy)
                    {
                        levelUpUI.SetActive(true);
                        levelUpUI.transform.localScale = new Vector3(0.05f, 0.05f, levelUpUI.transform.localScale.z);
                        UIManager.Instance.OpenCloseUIAnimation(levelUpUI, scale, openingAnimationDuration, true, true, false);
                    }
                    else
                    {
                        UIManager.Instance.OpenCloseUIAnimation(levelUpUI, 0.05f, closingAnimationDuration, false, true, false);
                    }
                }
                else
                {
                    fire.SetActive(true);
                    triggered = true;
                    CoreClass.GameManager.Instance.BonfiresLit.Add(BonfireId);
                }

                //Saving the game, position and level
                CoreClass.GameManager.checkpoint = spawnPoint.position;
                MenuManager.Instance.CurrentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
                SaveSystem.SavePlayer(player);

                PlayerInputHandler.Instance.UseUseInput();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var player = collision.GetComponent<Player>();
            var levelUpUI = UIManager.Instance.LevelUpInterface;

            if (player && levelUpUI && levelUpUI.activeInHierarchy)
            {
                UIManager.Instance.OpenCloseUIAnimation(levelUpUI, 0.05f, closingAnimationDuration, false, true, false);
            }
        }
    }
}
