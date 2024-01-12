using UnityEngine;
using Core;
using Inventory;

namespace Spells
{
    public class SpellPickup : MonoBehaviour
    {
        [SerializeField] private Spell spell;
        [SerializeField] private bool forSale;
        [SerializeField] private int price;
        [SerializeField] private bool chest;

        private bool isPickedUp = false;
        private Animator animator;

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
                        InventoryManager.Instance.SetCoins(InventoryManager.Instance.Coins - price);
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
        }
    }
}