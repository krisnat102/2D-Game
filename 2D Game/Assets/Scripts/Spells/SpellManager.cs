using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance;
    public List<Spell> Spells = new List<Spell>();

    public void Add(Spell spell)
    {
        Spells.Add(spell);
    }
    public void Remove(Spell spell)
    {
        Spells.Remove(spell);
    }
}
