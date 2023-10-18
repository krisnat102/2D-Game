using UnityEngine;
using UnityEngine.UI;

public class InventorySpellController : MonoBehaviour
{
    Spell spell;

    public Button RemoveButton;

    [SerializeField] private int ActiveSpellsMax = 8;

    public Spell Firebolt;
    public void RemoveSpell()
    {
        SpellManager.Instance.Remove(spell);

        Destroy(gameObject);
    }

    public void AddSpell(Spell newSpell)
    {
        spell = newSpell;
    }

    public void UseItem()
    {
        switch (spell.spellType)
        {
        case Spell.SpellType.Firebolt:
            if (SpellManager.SpellsBar.Count < ActiveSpellsMax && !SpellManager.SpellsBar.Contains(Firebolt))
            {
                RemoveButton.IsActive();
                SpellManager.SpellsBar.Add(Firebolt);
                SpellManager.Instance.ListActiveSpells();
            }
        break;
        }
    }
}
