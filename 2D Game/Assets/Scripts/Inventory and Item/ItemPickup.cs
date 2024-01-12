using UnityEngine;
using Core;

namespace Inventory
{
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private Item item;
        [SerializeField] private bool forSale;

        private bool isPickedUp = false;

        public void Pickup()
        {
            if (!isPickedUp)
            {
                InventoryManager.Instance.Add(item);

                Destroy(gameObject);

                isPickedUp = true;
            }
        }
    }
}