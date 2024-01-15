using UnityEngine;
using Core;
using Inventory;
using TMPro;
using UnityEngine.UI;

namespace Spells
{
    public class SpellPickup : MonoBehaviour
    {
        [SerializeField] private Spell spell;
        [SerializeField] private bool forSale;
        [SerializeField] private int price;
        [SerializeField] private bool chest;
        [SerializeField] private float offset = 0.2f;

        private bool isPickedUp = false;
        private Animator animator;
        private GameObject itemPrice;

        public void Pickup()
        {
            if (!isPickedUp && spell != null)
            {
                if (chest)
                {
                    animator?.SetTrigger("Open");
                    SpellManager.Instance.Add(spell);
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
                SpellManager.Instance.Add(spell);

                Destroy(gameObject);

                isPickedUp = true;
            }
        }

        private void Start()
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