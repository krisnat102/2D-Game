using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Core;
using Inventory;
using Krisnat;
using UnityEngine.UIElements;

namespace Spells
{
    public class SpellManager : MonoBehaviour
    {
        public static SpellManager Instance;
        #region Private Variables
        [SerializeField] private List<Spell> spells = new();

        [SerializeField] private Transform spellContent;
        [SerializeField] private GameObject inventorySpell;

        //[SerializeField] private InventorySpellController[] InventorySpells;

        [SerializeField] private GameObject spellInventory;
        [SerializeField] private GameObject inventory;

        [SerializeField] private Transform spellContentBar;
        [SerializeField] private GameObject activeSpell;

        [SerializeField] private float inventoryOpenTime = 0.3f;
        [SerializeField] private float inventoryCloseTime = 0.3f;

        [Header("Item Description")]
        [SerializeField] private UnityEngine.UI.Button useButton;
        [SerializeField] private UnityEngine.UI.Image spellImage;
        [SerializeField] private TMP_Text spellName, spellDescription, spellValue, spellPrice;
        [SerializeField] private GameObject description;
        private bool spellAbilityTab;
        #endregion

        #region Public Variables
        public List<Spell> AllSpells { get; private set; }
        public List<Spell> Spells { get => spells; set => spells = value; }

        public List<Spell> SpellsBar = new();
        public List<Spell> AbilitiesBar = new();

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
        }
        private void Start()
        {
            AllSpells = Core.GameManager.Instance.GetCustomAssets<Spell>("Spell", "CreatedAssets");
        }

        public void Update()
        {
            if (PlayerInputHandler.Instance.SpellInventoryInput)
            {
                PlayerInputHandler.Instance.UseSpellInventoryInput();
                if (!inventory.activeInHierarchy && !spellInventory.activeInHierarchy)
                {
                    spellInventory.SetActive(true);
                    var scale = spellInventory.transform.localScale.x;
                    spellInventory.transform.localScale = new Vector3(0.05f, 0.05f, spellInventory.transform.localScale.z);
                    UIManager.Instance.OpenUIAnimation(spellInventory, scale, inventoryOpenTime, true);

                    ListSpells();

                    //Weapon.canFire = false;
                }
                else
                {
                    UIManager.Instance.OpenUIAnimation(spellInventory, 0.05f, inventoryCloseTime, false);

                    //Weapon.canFire = true;
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

        public void ListSpells()
        {
            SpellController spellController;

            //clears the inventory before opening so that items dont duplicate
            foreach (Transform spell in spellContent)
            {
                Destroy(spell.gameObject);
            }

            //adds the items to the inventory
            foreach (var spell in Spells)
            {
                GameObject obj = Instantiate(inventorySpell, spellContent);

                obj.SetActive(true);

                //InventorySpells = spellContent.GetComponentsInChildren<InventorySpellController>();

                obj.name = spell.name;

                spellController = obj.GetComponent<SpellController>();
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