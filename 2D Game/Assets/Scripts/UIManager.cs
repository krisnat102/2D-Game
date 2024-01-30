using Bardent.CoreSystem;
using Inventory;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Krisnat
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("Purse")]
        [SerializeField] private GameObject purse;

        [Header("Character Tab")]
        [SerializeField] private GameObject characterTab;
        [SerializeField] private TMP_Text levelText, hpText, manaText, stamText, armorText, magicResText, weightText;

        [Header("Level Up Interface")]
        [SerializeField] private GameObject levelUpInterface;
        [SerializeField] private TMP_Text levelTextLevelUpInterface, levelUpCost, hpLevelUpText, manaLevelUpText, stamLevelUpText, strLevelUpText, dexLevelUpText, intLevelUpText;

        [SerializeField] private Slider bowChargeTimeSlider;

        private Vector3 oldPosition;
        private PlayerData playerData;
        private LevelHandler levelHandler;
        private bool purseAnimationTracker = false;

        public GameObject LevelUpInterface { get => levelUpInterface; set => levelUpInterface = value; }
        public Slider BowChargeTimeSlider { get => bowChargeTimeSlider; set => bowChargeTimeSlider = value; }

        private void Awake()
        {
            Instance = this;
            playerData = Stats.Instance.gameObject.GetComponentInParent<Player>().PlayerData;
            levelHandler = Stats.Instance.gameObject.GetComponentInParent<LevelHandler>();
        }

        private void Update()
        {
            levelUpCost.text = "Cost - " + levelHandler.LevelUpCost.ToString();
            levelTextLevelUpInterface.text = "Level - " + playerData.PlayerLevel.ToString();

            levelText.text = playerData.PlayerLevel.ToString();
            hpText.text = Stats.Instance.Health.MaxValue.ToString();
            manaText.text = Stats.Instance.Mana.MaxValue.ToString();
            stamText.text = Stats.Instance.Stam.MaxValue.ToString();
            armorText.text = InventoryManager.Instance.TotalArmor.ToString();
            magicResText.text = InventoryManager.Instance.TotalMagicRes.ToString();
            weightText.text = InventoryManager.Instance.TotalWeight.ToString() + "/50";

            hpLevelUpText.text = "HP - " + Stats.Instance.Health.MaxValue.ToString();
            manaLevelUpText.text = "Mana - " + Stats.Instance.Mana.MaxValue.ToString();
            stamLevelUpText.text = "Stam - " + Stats.Instance.Stam.MaxValue.ToString();
            strLevelUpText.text = "STR - " + levelHandler.StrengthCounter.ToString();
            dexLevelUpText.text = "DEX - " + levelHandler.DexterityCounter.ToString();
            intLevelUpText.text = "INT - " + levelHandler.IntelligenceCounter.ToString();
        }

        public void MovePurseAnimation(float animationDistance, float animationDuration)
        {
            if (!purseAnimationTracker)
            {
                purseAnimationTracker = true;
                purse.transform.LeanMove(new Vector2(purse.transform.position.x, purse.transform.position.y + animationDistance), animationDuration);
                oldPosition = purse.transform.position;
            }
            else
            {
                purseAnimationTracker = false;
                purse.transform.LeanMove(oldPosition, animationDuration);
            }
        }

        public void OpenUIAnimation(GameObject menu, float targetSize, float duration, bool openClose)
        {
            if(openClose) StartCoroutine(OpenUIAnimationCoroutine(menu, targetSize, duration));
            else StartCoroutine(CloseUIAnimationCoroutine(menu, targetSize, duration));
        }

        private IEnumerator OpenUIAnimationCoroutine(GameObject menu, float targetSize, float duration)
        {
            Vector3 initialScale = menu.transform.localScale;
            Vector3 targetScale = new Vector3(targetSize, targetSize, menu.transform.localScale.z);

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                menu.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            menu.transform.localScale = targetScale;
        }

        private IEnumerator CloseUIAnimationCoroutine(GameObject menu, float targetSize, float duration)
        {
            Vector3 initialScale = menu.transform.localScale;
            Vector3 targetScale = new Vector3(targetSize, targetSize, menu.transform.localScale.z);

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                menu.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            menu.transform.localScale = initialScale;
            menu.SetActive(false);
        }

        public void OpenCharacterTabBn()
        {
            InventoryManager.Instance.OpenCloseInventory(false);

            characterTab.SetActive(true);
        }
    }
}
