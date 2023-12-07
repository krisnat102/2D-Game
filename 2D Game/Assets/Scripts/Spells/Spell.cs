using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell/Create New Spell")]

public class Spell : ScriptableObject
{
    public string spellName;
    public bool spell;
    public int cost;
    public int value;
    public float speed;
    public float cooldown;
    public float range;
    public Sprite icon;
    public GameObject spellEffect;
    public GameObject spellDeath;
    [TextArea]
    public string description;
}