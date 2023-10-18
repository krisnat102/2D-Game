using UnityEngine;
using UnityEngine.UI;

public class InventorySpellController : MonoBehaviour
{
    Spell spell;

    public Button RemoveButton;
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
                    
                break;
        }
    }
}
