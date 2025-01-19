using Inventory;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class Shop : MonoBehaviour, IStructurable 
    {
        private AudioSource openAudio;
        private InventoryManager inventoryManager;

        [SerializeField] private List<Item> itemsForSale = new List<Item>();

        private void Start()
        {
            openAudio = GetComponent<AudioSource>();
            inventoryManager = InventoryManager.Instance;

            foreach(Item item in itemsForSale)
            {
                inventoryManager.CreateShopItem(item);
            }
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            var player = collision.GetComponent<Player>();

            if (PlayerInputHandler.Instance.UseInput && player)
            {
                PlayerInputHandler.Instance.UseUseInput();

                if (!inventoryManager.InventoryActiveInHierarchy)
                {
                    openAudio.Play();
                    inventoryManager.Shop = true;
                    inventoryManager.OpenCloseInventory(true);
                }
                else
                {
                    inventoryManager.Shop = false;
                    inventoryManager.OpenCloseInventory(false);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(inventoryManager.InventoryActiveInHierarchy && inventoryManager.Shop)
            {
                inventoryManager.Shop = false;
                inventoryManager.OpenCloseInventory(false);
            }
        }
    }
}
