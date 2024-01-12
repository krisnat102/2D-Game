using UnityEngine;

namespace Inventory
{
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private Item item;
        [SerializeField] private bool forSale;
        [SerializeField] private int price;
        [SerializeField] private bool chest;

        private bool isPickedUp = false;
        private Animator animator;

        //TODO: Make a hold time for chests so that it takes a certain amount of time to open them

        public void Pickup()
        {
            if (!isPickedUp && item != null)
            {
                if (chest)
                {
                    animator?.SetTrigger("Open");
                    InventoryManager.Instance.Add(item);
                    return;
                }
                if (forSale)
                {
                    InventoryManager.Instance.StartCoinAnimation();
                    if(InventoryManager.Instance.Coins >= price)
                    {
                        AudioManager.Instance.BuySound.Play();
                        InventoryManager.Instance.SetCoins(InventoryManager.Instance.Coins - price);
                    }
                    else
                    {
                        return;
                    }
                }
                InventoryManager.Instance.Add(item);

                Destroy(gameObject);

                isPickedUp = true;
            }
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
        }
    }
}