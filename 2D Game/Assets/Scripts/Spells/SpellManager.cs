using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Inventory;
using Krisnat;
using UnityEngine.UIElements;
using System.Linq;

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
        [SerializeField] private UnityEngine.UI.Button useButton;
        [SerializeField] private UnityEngine.UI.Image spellImage;
        [SerializeField] private TMP_Text spellName, spellDescription, spellValue, spellPrice;
        [SerializeField] private GameObject description;

        private GameObject spellInventory, inventory, characterTab;
        private float spellInventoryScale;
        private bool spellAbilityTab;
        private List<Spell> spells, oldSpells, spellsBar, abilitiesBar = new();
        #endregion

        #region Public Variables
        public List<Spell> AllSpells { get; private set; }
        public List<Spell> Spells { get => spells; private set => spells = value; }
        public List<Spell> SpellsBar { get => spellsBar; private set => spellsBar = value; }
        public List<Spell> AbilitiesBar { get => abilitiesBar; private set => abilitiesBar = value; }


        public UnityEngine.UI.Button useButton1;
        public UnityEngine.UI.Image spellImage1;
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
            AllSpells = CoreClass.GameManager.Instance.GetCustomAssets<Spell>("Spell", "CreatedAssets");

            inventory = InventoryManager.Instance.Inventory;
            spellInventory = InventoryManager.Instance.SpellInventory;
            characterTab = InventoryManager.Instance.CharacterTab;
            spellInventoryScale = spellInventory.transform.localScale.x;
        }

        public void Update()
        {
            if (PlayerInputHandler.Instance.SpellInventoryInput)
            {
                PlayerInputHandler.Instance.UseSpellInventoryInput();
                if (!inventory.activeInHierarchy && !spellInventory.activeInHierarchy && !characterTab.activeInHierarchy)
                {
                    OpenCloseSpellInventory(true);
                    ListSpells();
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
            oldSpells.Clear();

            spells = spells.Distinct().ToList();

            foreach (Transform trans in spellContent)
            {
                Spell spell = trans.GetComponent<SpellController>().GetSpell();

                if (oldSpells.Contains(spell))
                {
                    Destroy(trans.gameObject);
                }
                else
                {
                    oldSpells.Add(spell);
                    if (!spells.Contains(spell)) Destroy(trans.gameObject);
                }
            }

            //adds the items to the inventory
            foreach (var spell in Spells)
            {
                GameObject obj;

                if (!oldSpells.Contains(spell)) obj = CreateSpell(spell);
            }

            //SetInventorySpells();
        }

        /*public void SetInventorySpells()
        {
            {
                System.Array.Resize(ref InventorySpells, Spells.Count);

                for (int i = 0; i < Spells.Count; i++)
                {
                    InventorySpells[i].AddSpell(Spells[i]);
                }
            }
        }*/

        private GameObject CreateSpell(Spell spell)
        {
            Debug.Log(spell.spellName);
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

            //InventorySpells = spellContent.GetComponentsInChildren<InventorySpellController>();

            obj.name = spell.name;

            SpellController spellController = obj.GetComponent<SpellController>();
            spellController.SetSpell(spell);

            var spellName = obj.transform.Find("SpellName").GetComponent<Text>();
            var spellImage = obj.transform.Find("SpellIcon").GetComponent<UnityEngine.UI.Image>();

            spellName.text = spell.spellName;
            spellImage.sprite = spell.icon;

            if (spellAbilityTab)
                if (!spell.spell)
                {
                    obj.SetActive(false);
                }
            if (!spellAbilityTab)
                if (spell.spell)
                {
                    obj.SetActive(false);
                }

            return obj;
        }


        public void ListActiveSpells()
        {
            //clears the inventory before opening so that items dont duplicate
            foreach (Transform spell in spellContentBar)
            {
                Destroy(spell.gameObject);
            }
            if (spellAbilityTab)
            {
                //adds the items to the inventory
                foreach (var spell in SpellsBar)
                {
                    GameObject obj = Instantiate(activeSpell, spellContentBar);

                    obj.SetActive(true);

                    SpellController spellController = obj.GetComponent<SpellController>();
                    spellController.SetSpell(spell);

                    var spellName = obj.transform.Find("SpellName").GetComponent<Text>();
                    var spellIcon = obj.transform.Find("SpellIcon").GetComponent<UnityEngine.UI.Image>();

                    if (spellName != null && spellIcon != null)
                    {
                        spellName.text = spell.spellName;
                        spellIcon.sprite = spell.icon;
                    }

                    if (!spell.spell)
                    {
                        obj.SetActive(false);
                    }
                }
            }
            else if (!spellAbilityTab)
            {
                foreach (var spell in AbilitiesBar)
                {
                    GameObject obj = Instantiate(activeSpell, spellContentBar);

                    obj.SetActive(true);

                    SpellController spellController = obj.GetComponent<SpellController>();
                    spellController.SetSpell(spell);

                    var spellName = obj.transform.Find("SpellName").GetComponent<Text>();
                    var spellIcon = obj.transform.Find("SpellIcon").GetComponent<UnityEngine.UI.Image>();

                    if (spellName != null && spellIcon != null)
                    {
                        spellName.text = spell.spellName;
                        spellIcon.sprite = spell.icon;
                    }

                    if (spell.spell)
                    {
                        obj.SetActive(false);
                    }
                }
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
        }
        public void AbilitiesTabBn()
        {
            spellAbilityTab = false;
            ListSpells();
            ListActiveSpells();
        }
        #endregion
    }
}