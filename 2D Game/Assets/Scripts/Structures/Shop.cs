using Inventory;
using UnityEngine;

namespace Krisnat
{
    public class Shop : MonoBehaviour, IStructurable 
    {
        private AudioSource openAudio;

        private void Start()
        {
            openAudio = GetComponent<AudioSource>();
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            var player = collision.GetComponent<Player>();

            if (PlayerInputHandler.Instance.UseInput && player)
            {
                PlayerInputHandler.Instance.UseUseInput();

                if (!InventoryManager.Instance.InventoryActiveInHierarchy)
                {
                    openAudio.Play();
                    InventoryManager.Instance.Shop = true;
                    InventoryManager.Instance.OpenCloseInventory(true);
                }
                else
                {
                    InventoryManager.Instance.Shop = false;
                    InventoryManager.Instance.OpenCloseInventory(false);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(InventoryManager.Instance.InventoryActiveInHierarchy && InventoryManager.Instance.Shop)
            {
                InventoryManager.Instance.Shop = false;
                InventoryManager.Instance.OpenCloseInventory(false);
            }
        }
    }
}
