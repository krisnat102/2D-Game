using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private Item item;
        [SerializeField] private bool forSale;
        [SerializeField] private int price;
        [SerializeField] private bool chest;
        [SerializeField] private float offset = 0.2f;

        private bool isPickedUp = false;
        private Animator animator;
        private GameObject itemPrice;

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
                    if (InventoryManager.Instance.Coins >= price)
                    {
                        AudioManager.Instance.BuySound.Play();
                        InventoryManager.Instance.SetCoins(InventoryManager.Instance.Coins - price, false);
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

        private void Awake()
        {
            animator = GetComponent<Animator>();

            if (forSale)
            {
                itemPrice = GetComponentInChildren<Canvas>()?.GetComponent<Transform>()?.Find("ItemPrice")?.gameObject;
                itemPrice.SetActive(true);
                itemPrice.GetComponentInChildren<TMP_Text>().text = price.ToString();
                if (price < 10)
                {
                    itemPrice.GetComponentInChildren<Image>().gameObject.transform.position -= new Vector3(offset, 0);
                }
                else if (price > 99)
                {
                    itemPrice.GetComponentInChildren<Image>().gameObject.transform.position += new Vector3(offset, 0);
                }
            }
        }
    }
}