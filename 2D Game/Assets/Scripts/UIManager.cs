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
        #region Variables
        public static UIManager Instance;

        [Header("General")]
        [SerializeField] private GameObject canvas;

        [Header("Character Tab")]
        [SerializeField] private GameObject characterTab;
        [SerializeField] private TMP_Text levelText, hpText, manaText, stamText, armorText, magicResText, weightText;

        [Header("Level Up Interface")]
        [SerializeField] private GameObject levelUpInterface;
        [SerializeField] private TMP_Text levelTextLevelUpInterface, levelUpCost, hpLevelUpText, manaLevelUpText, stamLevelUpText, strLevelUpText, dexLevelUpText, intLevelUpText;
        [SerializeField] private float levelUpClosingDuration = 0.2f;

        [Header("Weapon Interface")]
        [SerializeField] private Slider bowChargeTimeSlider;

        [Header("Item Interface")]
        [SerializeField] private GameObject itemPickupPopUp;
        [SerializeField] private float moveDuration;
        [SerializeField] private float moveDistance;

        [Header("Messages")]
        [SerializeField] private GameObject noteUI;
        [SerializeField] private TMP_Text noteText;
        [SerializeField] private float noteScale;
        [SerializeField] private float noteOpeningSpeed;

        private bool inventoryAnimationActive;
        private PlayerData playerData;
        private LevelHandler levelHandler;

        public GameObject LevelUpInterface { get => levelUpInterface; private set => levelUpInterface = value; }
        public Slider BowChargeTimeSlider { get => bowChargeTimeSlider; private set => bowChargeTimeSlider = value; }
        public GameObject ItemPickupPopUp { get => itemPickupPopUp; private set => itemPickupPopUp = value; }
        public GameObject Canvas { get => canvas; private set => canvas = value; }
        public float MoveDuration { get => moveDuration; private set => moveDuration = value; }
        public float MoveDistance { get => moveDistance; private set => moveDistance = value; }
        public GameObject NoteUI { get => noteUI; private set => noteUI = value; }
        public TMP_Text NoteText { get => noteText; private set => noteText = value; }
        public float NoteScale { get => noteScale; private set => noteScale = value; }
        public float NoteOpeningSpeed { get => noteOpeningSpeed; private set => noteOpeningSpeed = value; }
        public bool NoteOpen { get; set; }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            Instance = this;
            playerData = Stats.Instance?.gameObject.GetComponentInParent<Player>().PlayerData;
            levelHandler = Stats.Instance?.gameObject.GetComponentInParent<LevelHandler>();
            NoteOpen = false;
        }

        private void Update()
        {
            if(levelUpCost) levelUpCost.text = "Cost - " + levelHandler?.LevelUpCost.ToString();
            if (levelTextLevelUpInterface) levelTextLevelUpInterface.text = "Level - " + playerData?.PlayerLevel.ToString();

            levelText.text = playerData?.PlayerLevel.ToString();
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

            if (PlayerInputHandler.Instance.MenuInput && LevelUpInterface.activeInHierarchy)
            {
                OpenCloseUIAnimation(LevelUpInterface, 0.05f, levelUpClosingDuration, false);
                PlayerInputHandler.Instance.UseMenuInput();
            }

            if (PlayerInputHandler.Instance.UseInput && NoteUI.activeInHierarchy)
            {
                Transform[] noteChildren = NoteUI.GetComponentsInChildren<Transform>();

                foreach (Transform child in noteChildren)
                {
                    child.gameObject.SetActive(false);
                }
                PlayerInputHandler.Instance.UseUseInput();

                NoteOpen = false;
            }
        }
        #endregion

        #region Core UI Methods

        #region Open or Close UI Animation
        public void OpenCloseUIAnimation(GameObject menu, float targetSize, float duration, bool openClose)
        {
            if (openClose)
            {
                inventoryAnimationActive = true;
                StartCoroutine(OpenUIAnimationCoroutine(menu, targetSize, duration));
            }
            else
            {
                inventoryAnimationActive = true;
                if (menu) StartCoroutine(CloseUIAnimationCoroutine(menu, targetSize, duration));
            }
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
            inventoryAnimationActive = false;
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
            inventoryAnimationActive = false;
            menu.SetActive(false);
        }
        #endregion

        #region Open or Close UI
        public void OpenCloseUI(GameObject ui, float initialScale, float openingSpeed, bool stopAttack, bool stopAllInputs, bool openOrClose)
        {
            OpenCloseUI(ui, initialScale, openingSpeed, stopAttack, stopAllInputs, openOrClose, null);
        }
        public void OpenCloseUI(GameObject ui, float initialScale, float openingSpeed, bool stopAttack, bool stopAllInputs, bool openOrClose, GameObject[] uiToClose)
        {
            if (inventoryAnimationActive) return;
            if (openOrClose)
            {
                ui.SetActive(true);
                ui.transform.localScale = new Vector3(0.05f, 0.05f, ui.transform.localScale.z);
                OpenCloseUIAnimation(ui, initialScale, openingSpeed, true);

                if (stopAttack) PlayerInputHandler.Instance.StopAttack = true;
                if (stopAllInputs) PlayerInputHandler.Instance.StopAllInputs = true;
            }
            else
            {
                OpenCloseUIAnimation(ui, 0.05f, openingSpeed, false);

                if (stopAttack) PlayerInputHandler.Instance.StopAttack = false;
                if (stopAllInputs) PlayerInputHandler.Instance.StopAllInputs = false;

                if (InventoryManager.Instance.Shop)
                {
                    InventoryManager.Instance.Shop = false;
                }

                if (uiToClose != null)
                {
                    foreach (var uiElement in uiToClose)
                    {
                        uiElement?.SetActive(false);
                    }
                }
            }
        }
        #endregion

        #endregion

        #region UI Animation
        /*public void MovePurseAnimation(float animationDistance, float animationDuration)
        {
            if (!purseAnimationTracker)
            {
                purseAnimationTracker = true;
                purse.transform.LeanMove(new Vector2(purse.transform.position.x, purse.transform.position.y + animationDistance), animationDuration);
                oldPosition = purse.transform.position;
            }
            else
            {
                Invoke("StopPurseTracker", animationDuration);
                purse.transform.LeanMove(oldPosition, animationDuration);
            }
        }
        private void StopPurseTracker() => purseAnimationTracker = false;*/
        #endregion
    }
}
