using Inventory;
using UnityEngine;

namespace Krisnat
{
    public class Shop : MonoBehaviour, IStructurable 
    {
        public void OnTriggerStay2D(Collider2D collision)
        {
            var player = collision.GetComponent<Player>();
            if (PlayerInputHandler.Instance.UseInput && player != null)
            {
                if (!InventoryManager.Instance.InventoryActiveInHierarchy)
                {
                    InventoryManager.Instance.OpenCloseInventory(true);
                    InventoryManager.Instance.Shop = true;
                }
                else
                {
                    InventoryManager.Instance.OpenCloseInventory(false);
                    InventoryManager.Instance.Shop = false;
                }
                PlayerInputHandler.Instance.UseUseInput();
            }
        }
    }
}
