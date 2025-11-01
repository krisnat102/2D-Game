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
        [SerializeField] private Transform shopItemContent;

        private void Start()
        {
            openAudio = GetComponent<AudioSource>();
            inventoryManager = InventoryManager.Instance;

            foreach(Item item in itemsForSale)
            {
                inventoryManager.CreateShopItem(item, shopItemContent);
            }
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            var pickup = collision.GetComponent<Pickup>();

            if (PlayerInputHandler.Instance.UseInput && pickup)
            {
                PlayerInputHandler.Instance.UseUseInput();

                if (!inventoryManager.InventoryActiveInHierarchy)
                {
                    openAudio.Play();
                    inventoryManager.Shop = true;
                    inventoryManager.OpenCloseInventory(true);
                    shopItemContent.gameObject.SetActive(true);
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
                shopItemContent.gameObject.SetActive(false);
            }
        }
    }
}
