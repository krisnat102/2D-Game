using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance;
    public List<Spell> Spells = new List<Spell>();

    public Transform SpellContent;
    public GameObject InventorySpell;

    public InventorySpellController[] InventorySpells;

    public GameObject SpellInventory;
    public GameObject Inventory;

    public static List<Spell> SpellsBar = new List<Spell>();
    public Transform SpellContentBar;
    public GameObject ActiveSpell;

    private void Awake()
    {
        Instance = this;
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
            var spellName = obj.transform.Find("SpellName").GetComponent<Text>();
            //var spellName = obj.GetComponentInChildren<Text>();
            var spellIcon = obj.transform.Find("SpellIcon").GetComponent<Image>();
            //var spellIcon = obj.GetComponentInChildren<Image>();
            var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();
            //var removeButton = obj.GetComponentInChildren<Button>();

            if (spellName != null && spellIcon != null && removeButton != null)
            {
                Debug.Log(SpellsBar);
                spellName.text = spell.SpellName;
                spellIcon.sprite = spell.icon;
            }
        }
    }
}
