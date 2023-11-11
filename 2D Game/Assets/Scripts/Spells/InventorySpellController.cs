using UnityEngine;
using UnityEngine.UI;

public class InventorySpellController : MonoBehaviour
{
    Spell spell;

    [SerializeField] private Button RemoveButton;

    [SerializeField] private int ActiveSpellsMax = 8;

    [SerializeField] private Spell Firebolt;
    [SerializeField] private Spell RayOfFrost;

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
        switch (spell.spellType)
        {
            case Spell.SpellType.Firebolt:
                    if (SpellManager.SpellsBar.Count < ActiveSpellsMax && !SpellManager.SpellsBar.Contains(Firebolt))
                {
                    SpellManager.SpellsBar.Add(Firebolt);
                    SpellManager.Instance.ListActiveSpells();
                    //SpellManager.SpellsBar.Remove(Firebolt);
                }
                    else
                    {
                        Debug.Log("already there");
                    }
        break;
            case Spell.SpellType.RayOfFrost:
                if (SpellManager.SpellsBar.Count < ActiveSpellsMax && !SpellManager.SpellsBar.Contains(RayOfFrost))
                {
                    SpellManager.SpellsBar.Add(RayOfFrost);
                    SpellManager.Instance.ListActiveSpells();
                    //SpellManager.SpellsBar.Remove(RayOfFrost);
                }
                else
                {
                    Debug.Log("already there");
                }
                break;
        }
    }

    public void RemoveActiveSpell()
    {
        Destroy(gameObject);

        SpellManager.SpellsBar.Remove(spell);
    }
}
