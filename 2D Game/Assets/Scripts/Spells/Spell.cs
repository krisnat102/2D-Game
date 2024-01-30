using UnityEngine;
namespace Spells
{
    [CreateAssetMenu(fileName = "New Spell", menuName = "Spell/Create New Spell")]
    public class Spell : ScriptableObject
    {
        public int id;
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
        public bool useOffset;
        public Vector2 offset;
    }
}