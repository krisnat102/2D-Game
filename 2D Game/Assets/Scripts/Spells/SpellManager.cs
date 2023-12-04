using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance;
    [SerializeField] private List<Spell> Spells = new List<Spell>();

    [SerializeField] private Transform SpellContent;
    [SerializeField] private GameObject InventorySpell;

    [SerializeField] private InventorySpellController[] InventorySpells;

    [SerializeField] private GameObject spellInventory;
    [SerializeField] private GameObject inventory;

    public static List<Spell> SpellsBar = new List<Spell>();
    [SerializeField] private Transform SpellContentBar;
    [SerializeField] private GameObject ActiveSpell;

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
        foreach (Transform spell in SpellContent)
        {
            Destroy(spell.gameObject);
        }

        //adds the items to the inventory
        foreach (var spell in Spells)
        {
            /*GameObject obj = Instantiate(InventorySpell, SpellContent);
            obj.SetActive(true);

            SpellController spellController = obj.GetComponent<SpellController>();
            spellController.spell = spell;

            var spellName = obj.transform.Find("SpellName").GetComponent<Text>();
            var spellIcon = obj.transform.Find("SpellIcon").GetComponent<Image>();
            var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();

            if (spellName != null || spellIcon != null || removeButton != null)
            {
                spellName.text = spell.spellName;
                spellIcon.sprite = spell.icon;
            }
            if (spellAbilityTab)
            {
                if (!spell.spell)
                {
                    obj.SetActive(false);
                }
            }
            else
            {
                if (spell.spell)
                {
                    obj.SetActive(false);
                }
            }
        }
        SetInventorySpells();
            */

            GameObject obj = Instantiate(InventorySpell, SpellContent);

            obj.SetActive(true);

            InventorySpells = SpellContent.GetComponentsInChildren<InventorySpellController>();

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

        SetInventorySpells();
    }

    public void SetInventorySpells()
    {
        //if (SpellContent.GetComponentsInChildren<InventorySpellController>().Length < Spells.Count)
        {
            //InventorySpells = SpellContent.GetComponentsInChildren<InventorySpellController>();
            System.Array.Resize(ref InventorySpells, Spells.Count);

            for (int i = 0; i < Spells.Count; i++)
            {
                InventorySpells[i].AddSpell(Spells[i]);
            }
        }
    }

    public void Update()
    {
        if (GameManager.gamePaused == false)
        {
            if (Input.GetButtonDown("SpellInventory"))
            {
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
        foreach (Transform spell in SpellContentBar)
        {
            Destroy(spell.gameObject);
        }

        //adds the items to the inventory
        foreach (var spell in SpellsBar)
        {
            GameObject obj = Instantiate(ActiveSpell, SpellContentBar);
            SpellController spellController = obj.GetComponent<SpellController>();
            spellController.SetSpell(spell);

            var spellName = obj.transform.Find("SpellName").GetComponent<Text>();
            //var spellName = obj.GetComponentInChildren<Text>();

            var spellIcon = obj.transform.Find("SpellIcon").GetComponent<Image>();
            //var spellIcon = obj.GetComponentInChildren<Image>();

            if (spellName != null && spellIcon != null)
            {
                spellName.text = spell.spellName;
                spellIcon.sprite = spell.icon;
            }
        }
    }

    public void SpellsTabBn()
    {
        spellAbilityTab = true;
        ListSpells();
    }
    public void AbilitiesTabBn()
    {
        spellAbilityTab = false;
        ListSpells();
    }
}
