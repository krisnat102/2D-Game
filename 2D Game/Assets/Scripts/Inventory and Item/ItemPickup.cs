using UnityEngine;
using Core;

namespace Inventory
{
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private Item item;

        private bool isPickedUp = false;

        void Pickup()
        {
            if (!isPickedUp)
            {
                InventoryManager.Instance.Add(item);

                Destroy(gameObject);

                isPickedUp = true;
            }
        }

        private void OnTriggerStay2D(Collider2D hitInfo)
        {
            if (hitInfo.tag == "PickupRange" && InputManager.Instance.UseInput) Pickup();
        }
    }
}