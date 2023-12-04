using UnityEngine;

public class SpellController : MonoBehaviour
{
    private Spell spell;

    public void SetSpell(Spell spell)
    {
        this.spell = spell;
    }
    public Spell GetSpell()
    {
        return spell;
    }
}
