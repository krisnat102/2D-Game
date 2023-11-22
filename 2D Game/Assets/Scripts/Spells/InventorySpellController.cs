using UnityEngine;
using UnityEngine.UI;

public class InventorySpellController : MonoBehaviour
{
    Spell spell;

    [SerializeField] private Button RemoveButton;

    [SerializeField] private int ActiveSpellsMax = 8;

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
}
