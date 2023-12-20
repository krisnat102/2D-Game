using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Core;

namespace Spells
{
    public class SpellManager : MonoBehaviour
    {
        public static SpellManager Instance;
        [SerializeField] private List<Spell> Spells = new List<Spell>();

        [SerializeField] private Transform spellContent;
        [SerializeField] private GameObject inventorySpell;

        //[SerializeField] private InventorySpellController[] InventorySpells;

        [SerializeField] private GameObject spellInventory;
        [SerializeField] private GameObject inventory;

        public static List<Spell> SpellsBar = new List<Spell>();
        public static List<Spell> AbilitiesBar = new List<Spell>();
        [SerializeField] private Transform spellContentBar;
        [SerializeField] private GameObject activeSpell;

        [Header("Item Description")]
        [SerializeField] private Button useButton;
        [SerializeField] private Image spellImage;
        [SerializeField] private TMP_Text spellName, spellDescription, spellValue, spellPrice;
        [SerializeField] private GameObject description;

        public static Button useButton1;
        public static Image spellImage1;
        public static TMP_Text spellName1, spellDescription1, spellValue1, spellPrice1;
        public static GameObject description1;

        private bool spellAbilityTab;

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

        public void Add(Spell spell)
        {
            Spells.Add(spell);
        }
        public void Remove(Spell spell)
        {
            Spells.Remove(spell);
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
                var spellImage = obj.transform.Find("SpellIcon").GetComponent<Image>();

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

        public void Update()
        {
            if (Core.GameManager.gamePaused == false)
            {
                if (PlayerInputHandler.Instance.SpellInventoryInput)
                {
                    PlayerInputHandler.Instance.UseSpellInventoryInput();
                    if (!inventory.activeInHierarchy && !spellInventory.activeInHierarchy)
                    {
                        spellInventory.SetActive(true);

                        ListSpells();

                        Weapon.canFire = false;
                    }
                    else
                    {
                        inventory.SetActive(false);
                        spellInventory.SetActive(false);

                        Weapon.canFire = true;
                    }
                }
            }
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
                    var spellIcon = obj.transform.Find("SpellIcon").GetComponent<Image>();

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
                    var spellIcon = obj.transform.Find("SpellIcon").GetComponent<Image>();

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
    }
}