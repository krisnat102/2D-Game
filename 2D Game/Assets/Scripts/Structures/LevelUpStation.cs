using Bardent.CoreSystem;
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
        [SerializeField] private AudioSource ignitingSFX, openSFX;
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
            var pickup = collision.GetComponent<Pickup>();

            if (PlayerInputHandler.Instance.UseInput && pickup != null)
            {
                PlayerInputHandler.Instance.UseUseInput();

                if (triggered)
                {
                    ActivateUI();
                }
                else
                {
                    IgniteBonfire();
                }

                Save(Player.instance);

                Player.instance.onRest.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var pickup = collision.GetComponent<Pickup>();
            var levelUpUI = UIManager.instance.LevelUpInterface;

            if (pickup && levelUpUI && levelUpUI.activeInHierarchy)
            {
                UIManager.instance.OpenCloseUIAnimation(levelUpUI, 0.05f, closingAnimationDuration, false, true, false);
            }
        }

        private void ActivateUI()
        {
            var levelUpUI = UIManager.instance.LevelUpInterface;
            var scale = levelUpUI.transform.localScale.x;

            if (!levelUpUI.activeInHierarchy)
            {
                levelUpUI.SetActive(true);
                levelUpUI.transform.localScale = new Vector3(0.05f, 0.05f, levelUpUI.transform.localScale.z);

                UIManager.instance.OpenCloseUIAnimation(levelUpUI, scale, openingAnimationDuration, true, true, false);
                UIManager.instance.UpdateLevelUpUI();

                openSFX.Play();
            }
            else
            {
                UIManager.instance.OpenCloseUIAnimation(levelUpUI, 0.05f, closingAnimationDuration, false, true, false);
            }
        }

        private void IgniteBonfire()
        {
            ignitingSFX.Play();
            fire.SetActive(true);
            triggered = true;
            CoreClass.GameManager.instance.BonfiresLit.Add(BonfireId);
        }

        private void Save(Player player)
        {
            CoreClass.GameManager.instance.Checkpoint = spawnPoint.position;
            MenuManager.instance.CurrentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

            Stats.instance.Replenish();

            SaveSystem.SavePlayer(player);

            PlayerInputHandler.Instance.UseUseInput();
        }
    }
}
