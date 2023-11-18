using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell/Create New Spell")]

public class Spell : ScriptableObject
{
    public int id;
    public string SpellName;
    public bool spell;
    public int cost;
    public int value;
    public float speed;
    public float cooldown;
    public float range;
    public Sprite icon;
    public GameObject spellEffect;
    public GameObject spellDeath;
    public SpellType spellType;

    public enum SpellType
    {
        Firebolt,
        RayOfFrost,
    }
}