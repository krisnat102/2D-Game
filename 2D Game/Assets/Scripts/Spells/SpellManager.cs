using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Inventory;
using Krisnat;
using System.Linq;
using System.Collections;

namespace Spells
{
    public class SpellManager : MonoBehaviour
    {
        public static SpellManager Instance;
        #region Private Variables

        [SerializeField] private Transform spellContent, abilityContent;
        [SerializeField] private GameObject inventorySpell;

        [SerializeField] private Transform spellContentBar, abilityContentBar;
        [SerializeField] private GameObject activeSpell;

        [SerializeField] private float inventoryOpenTime = 0.3f;
        [SerializeField] private float inventoryCloseTime = 0.3f;

        [Header("Spell Description")]
        [SerializeField] private Button useButton;
        [SerializeField] private Image spellImage;
        [SerializeField] private TMP_Text spellName, spellDescription, spellValue, spellPrice;
        [SerializeField] private GameObject description;

        private GameObject spellInventory, inventory, characterTab, levelUpUI;
        private float spellInventoryScale;
        private bool spellAbilityTab;
        private List<Spell> spells, oldSpells, spellsBar, abilitiesBar, oldSpellsBar, oldAbilitiesBar = new();
        #endregion

        #region Public Variables
        public List<Spell> AllSpells { get; private set; }
        public List<Spell> Spells { get => spells; private set => spells = value; }
        public List<Spell> SpellsBar { get => spellsBar; private set => spellsBar = value; }
        public List<Spell> AbilitiesBar { get => abilitiesBar; private set => abilitiesBar = value; }
        public List<Spell> OldSpellsBar { get => oldSpellsBar; private set => oldSpellsBar = value; }
        public List<Spell> OldAbilitiesBar { get => oldAbilitiesBar; private set => oldAbilitiesBar = value; }


        public Button useButton1;
        public Image spellImage1;
        public TMP_Text spellName1, spellDescription1, spellValue1, spellPrice1;
        public GameObject description1;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            Instance = this;

            useButton1 = useButton;
            spellImage1 = spellImage;
            spellName1 = spellName;
            spellDescription1 = spellDescription;
            spellValue1 = spellValue;
            spellPrice1 = spellPrice;
            description1 = description;

            spellAbilityTab = true;

            spells = new();
            oldSpells = new();
            spellsBar = new();
            abilitiesBar = new();
        }
        private void Start()
        {
            AllSpells = CoreClass.GameManager.Instance.GetCustomAssets<Spell>("Spell", "Spells");

            inventory = InventoryManager.Instance.Inventory;
            spellInventory = InventoryManager.Instance.SpellInventory;
            characterTab = InventoryManager.Instance.CharacterTab;
            levelUpUI = UIManager.Instance.LevelUpInterface;
            spellInventoryScale = spellInventory.transform.localScale.x;

            oldSpellsBar = new();
        }

        public void Update()
        {
            if (PlayerInputHandler.Instance.SpellInventoryInput)
            {
                PlayerInputHandler.Instance.UseSpellInventoryInput();
                if (!inventory.activeInHierarchy && !spellInventory.activeInHierarchy && !characterTab.activeInHierarchy && !levelUpUI.activeInHierarchy)
                {
                    OpenCloseSpellInventory(true);
                    SpellsTabBn();
                }
                else if (characterTab.activeInHierarchy)
                {
                    InventoryManager.Instance.OpenCloseCharacterTab(false);
                }
                else if (inventory.activeInHierarchy)
                {
                    InventoryManager.Instance.OpenCloseInventory(false);
                }
                else
                {
                    OpenCloseSpellInventory(false);
                }
            }

        }
        #endregion

        #region Spell Management
        public void Add(Spell spell)
        {
            Spells.Add(spell);
        }
        public void Add(List<Spell> spell)
        {
            Spells.AddRange(spell);
        }
        public void Remove(Spell spell)
        {
            Spells.Remove(spell);
        }
        public void ClearInventory()
        {
            Spells.Clear();
            ListSpells();
        }
        public void ClearActiveBar()
        {
            SpellsBar.Clear();
            AbilitiesBar.Clear();
            ListActiveSpells();
        }

        public void ListSpells()
        {
            spells = spells.Distinct().ToList();

            foreach (var spell in Spells)
            {
                if (!oldSpells.Contains(spell))
                {
                    CreateSpell(spell);
                    oldSpells.Add(spell);
                }
            }
        }

        private GameObject CreateSpell(Spell spell)
        {
            GameObject obj;

            if (spell.spell)
            {
                obj = Instantiate(inventorySpell, spellContent);
            }
            else
            {
                obj = Instantiate(inventorySpell, abilityContent);
            }

            obj.SetActive(true);

            obj.name = spell.name;

            SpellController spellController = obj.GetComponent<SpellController>();
            spellController.SetSpell(spell);

            var spellName = obj.transform.Find("SpellName").GetComponent<Text>();
            var spellImage = obj.transform.Find("SpellIcon").GetComponent<Image>();

            spellName.text = spell.spellName;
            spellImage.sprite = spell.icon;

            return obj;
        }


        public void ListActiveSpells()
        {
            if (spellAbilityTab)
            {
                SpellsBar = SpellsBar.Distinct().ToList();

                foreach (var spell in SpellsBar)
                {
                    if (!oldSpellsBar.Contains(spell))
                    {
                        CreateActiveSpell(spell);
                        oldSpellsBar.Add(spell);
                    }
                }
            }
            else
            {
                AbilitiesBar = AbilitiesBar.Distinct().ToList();

                foreach (var spell in AbilitiesBar)
                {
                    if (!oldAbilitiesBar.Contains(spell))
                    {
                        CreateActiveSpell(spell);
                        oldAbilitiesBar.Add(spell);
                    }
                }
            }
        }

        private void CreateActiveSpell(Spell spell)
        {
            GameObject obj;
            if(spell.spell) obj = Instantiate(activeSpell, spellContentBar);
            else obj = Instantiate(activeSpell, abilityContentBar);

            obj.SetActive(true);

            SpellController spellController = obj.GetComponent<SpellController>();
            spellController.SetSpell(spell);

            var spellName = obj.transform.Find("SpellName").GetComponent<Text>();
            var spellIcon = obj.transform.Find("SpellIcon").GetComponent<Image>();

            if (spellName != null && spellIcon != null)
            {
                spellName.text = spell.spellName;
                spellIcon.sprite = spell.icon;
            }
        }

        public void DisableSelectedIndicators()
        {
            foreach (Transform spell in spellContent)
            {
                spell.GetComponent<InventorySpellController>().SelectedItemIndicator.gameObject.SetActive(false);
            }
        }
        #endregion

        #region Spell Inventory Management

        public void OpenCloseSpellInventory(bool openClose)
        {
            if (openClose)
            {
                UIManager.Instance.OpenCloseUI(spellInventory, spellInventoryScale, inventoryOpenTime, true, false, true);
                ListSpells();
            }
            else
            {
                GameObject[] uiToClose = new GameObject[2];
                uiToClose[0] = inventory; uiToClose[1] = characterTab;
                UIManager.Instance.OpenCloseUI(spellInventory, spellInventoryScale, inventoryCloseTime, true, false, false, uiToClose);
                description.SetActive(false);
            }
        }
        #endregion

        #region Buttons
        public void SpellsTabBn()
        {
            spellAbilityTab = true;
            ListSpells();
            ListActiveSpells();
            DisableSelectedIndicators();
            description1.SetActive(false);
            spellContent.parent.gameObject.SetActive(true);
            spellContentBar.parent.gameObject.SetActive(true);
            abilityContent.parent.gameObject.SetActive(false);
            abilityContentBar.parent.gameObject.SetActive(false);
        }
        public void AbilitiesTabBn()
        {
            spellAbilityTab = false;
            ListSpells();
            ListActiveSpells();
            DisableSelectedIndicators();
            description1.SetActive(false);
            spellContent.parent.gameObject.SetActive(false);
            spellContentBar.parent.gameObject.SetActive(false);
            abilityContent.parent.gameObject.SetActive(true);
            abilityContentBar.parent.gameObject.SetActive(true);
        }
        #endregion

        #region UI
        public void SpellTakenPopUp(Spell spell)
        {
            InventoryManager.Instance.UIPopUpCooldownTracker = InventoryManager.Instance.UIPopUpCooldown;

            StartCoroutine(InventoryManager.Instance.ReduceCooldownOverTime());

            var canvasTransform = UIManager.Instance.Canvas.transform;
            var itemPopUp = Instantiate(UIManager.Instance.ItemPickupPopUp, canvasTransform).GetComponent<PopUpUI>();

            foreach (var ui in ItemPickup.itemPopUps)
            {
                ui.GoUp();
            }

            ItemPickup.itemPopUps.Add(itemPopUp);

            float yOffset = Mathf.Lerp(-350, -550, (Screen.currentResolution.height - 720f) / (2160f - 720f));
            yOffset = Mathf.Clamp(yOffset, -550, -350);

            itemPopUp.transform.position = canvasTransform.position + new Vector3(0, yOffset, 0);

            var images = itemPopUp.GetComponentsInChildren<Image>();
            if (images.Length > 1) images[1].sprite = spell.icon;

            var textComponent = itemPopUp.GetComponentInChildren<TMP_Text>();
            if (textComponent != null) textComponent.text = spell.spellName;
        }

        public IEnumerator SpellPopupRoutine(Spell spell, float delay)
        {
            yield return new WaitForSeconds(delay);
            SpellTakenPopUp(spell);
        }
        #endregion
    }
}