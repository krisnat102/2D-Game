using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySpellController : MonoBehaviour
{
    Spell spell;

    [SerializeField] private int ActiveSpellsMax = 8;

    private Button useButton;
    private Image spellImage;
    private TMP_Text spellName, spellPrice, spellValue, spellDescription;
    private GameObject description;

    public void RemoveSpell()
    {
        SpellManager.Instance.Remove(spell);

        Destroy(gameObject);
    }

    public void AddSpell(Spell newSpell)
    {
        spell = newSpell;
    }

    public void UseSpell()
    {
        GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        SpellController spellController = button.GetComponent<SpellController>();
        Spell spell = spellController.spell;

        if (SpellManager.SpellsBar.Count < ActiveSpellsMax && !SpellManager.SpellsBar.Contains(spell))
        {
            SpellManager.SpellsBar.Add(spell);
            SpellManager.Instance.ListActiveSpells();
        }
        else Debug.Log("already there");
    }

    public void RemoveActiveSpell()
    {
        GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        SpellController spellController = button.GetComponent<SpellController>();

        Destroy(gameObject);

        SpellManager.SpellsBar.Remove(spellController.spell);
        Debug.Log(spellController.spell.name);
    }

    public void Description()
    {
        GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        SpellController spellController = button.GetComponent<SpellController>();

        description = SpellManager.description1;
        description.SetActive(true);

        spellImage = SpellManager.spellImage1;
        spellName = SpellManager.spellName1;
        spellDescription = SpellManager.spellDescription1;
        spellValue = SpellManager.spellValue1;
        spellPrice = SpellManager.spellPrice1;

        spellImage.sprite = spellController.spell.icon;
        spellName.text = spellController.spell.spellName.ToUpper();
        spellDescription.text = spellController.spell.description;
        if (spellController.spell.value != 0)
        {
            spellValue.text = "DMG - " + spellController.spell.value.ToString();
        }
        else spellValue.text = null;
        spellPrice.text = "COST - " + spellController.spell.cost.ToString();

        useButton = SpellManager.useButton1;
        SpellController useButtonSpellController = useButton.GetComponent<SpellController>();
        useButtonSpellController.spell = spellController.spell;
    }
}
