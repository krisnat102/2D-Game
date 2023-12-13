using UnityEngine;

namespace Inventory
{
    public class ItemController : MonoBehaviour
    {

        private Item item;

        public Item GetItem()
        {
            return this.item;
        }

        public void SetItem(Item item)
        {
            this.item = item;
        }

    }
}