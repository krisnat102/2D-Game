using UnityEngine;
using Core;

namespace Spells
{
    public class SpellPickup : MonoBehaviour
    {
        [SerializeField] private Spell spell;

        private bool isPickedUp = false;

        void Pickup()
        {
            if (!isPickedUp)
            {
                SpellManager.Instance.Add(spell);

                Destroy(gameObject);

                isPickedUp = true;
            }
        }

        private void OnTriggerStay2D(Collider2D hitInfo)
        {
            if (hitInfo.tag == "PickupRange" && InputManager.Instance.UseInput)
            {
                Pickup();
            }
        }
    }
}