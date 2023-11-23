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

    [SerializeField] private GameObject SpellInventory;
    [SerializeField] private GameObject Inventory;

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
        //clears the inventory before opening so that items dont duplicate
        foreach (Transform spell in SpellContent)
        {
            Destroy(spell.gameObject);
        }

        //adds the items to the inventory
        foreach (var spell in Spells)
        {
            GameObject obj = Instantiate(InventorySpell, SpellContent);
            SpellController spellController = obj.GetComponent<SpellController>();
            spellController.spell = spell;

            var spellName = obj.transform.Find("SpellName").GetComponent<Text>();
            var spellIcon = obj.transform.Find("SpellIcon").GetComponent<Image>();
            var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();

            if (spellName != null || spellIcon != null || removeButton != null)
            {
                spellName.text = spell.SpellName;
                spellIcon.sprite = spell.icon;
            }
        }
        SetInventorySpells();
    }

    public void SetInventorySpells()
    {
        if (SpellContent != null)
        {
            InventorySpells = SpellContent.GetComponentsInChildren<InventorySpellController>();

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
                if (!Inventory.activeInHierarchy && !SpellInventory.activeInHierarchy)
                {
                    SpellInventory.SetActive(true);

                    ListSpells();

                    Weapon.canFire = false;
                }
                else
                {
                    Inventory.SetActive(false);
                    SpellInventory.SetActive(false);

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
            spellController.spell = spell;

            var spellName = obj.transform.Find("SpellName").GetComponent<Text>();
            //var spellName = obj.GetComponentInChildren<Text>();

            var spellIcon = obj.transform.Find("SpellIcon").GetComponent<Image>();
            //var spellIcon = obj.GetComponentInChildren<Image>();

            if (spellName != null && spellIcon != null)
            {
                spellName.text = spell.SpellName;
                spellIcon.sprite = spell.icon;
            }
        }
    }
}
