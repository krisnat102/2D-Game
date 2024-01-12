using UnityEngine;
using Core;

namespace Spells
{
    public class SpellPickup : MonoBehaviour
    {
        [SerializeField] private Spell spell;

        private bool isPickedUp = false;

        public void Pickup()
        {
            if (!isPickedUp)
            {
                SpellManager.Instance.Add(spell);

                Destroy(gameObject);

                isPickedUp = true;
            }
        }
    }
}