using Inventory;
using Spells;

namespace Krisnat
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Pickup : MonoBehaviour
    {
        [SerializeField] private GameObject message;

        private bool isPickedUp = false;
        private List<Collider2D> potentialItems = new List<Collider2D>();
        private Collider2D oldClosestItem;
        private GameObject oldClosestItemImage;

        private void OnTriggerEnter2D(Collider2D other)
        {
            var itemPickup = other.GetComponent<ItemPickup>();
            var spellPickup = other.GetComponent<SpellPickup>();

            if ((itemPickup != null || spellPickup != null) && !potentialItems.Contains(other))
            {
                potentialItems.Add(other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var itemPickup = other.GetComponent<ItemPickup>();
            var spellPickup = other.GetComponent<SpellPickup>();

            if ((itemPickup != null || spellPickup != null) && potentialItems.Contains(other))
            {
                potentialItems.Remove(other);
                other.GetComponent<Transform>().Find("Canvas").Find("PickUpKeyImage").gameObject.SetActive(false);
                if (itemPickup && itemPickup.Note)
                {
                    if(message) message.SetActive(false);
                }
            }
        }

        private void Update()
        {
            if (FindClosestItem() != null)
            {
                var closestItemImage = FindClosestItem().GetComponent<Transform>().Find("Canvas").Find("PickUpKeyImage").gameObject;
                if (oldClosestItem != null)
                {
                    oldClosestItemImage = oldClosestItem.GetComponent<Transform>().Find("Canvas").Find("PickUpKeyImage").gameObject;
                }

                if (closestItemImage != null && oldClosestItem == FindClosestItem())
                {
                    closestItemImage.SetActive(true);
                }
                else if ((oldClosestItem != FindClosestItem() && oldClosestItemImage != null))
                {
                    oldClosestItemImage.SetActive(false);
                }

                oldClosestItem = FindClosestItem();
            }
            if (PlayerInputHandler.Instance.UseInput && potentialItems.Count > 0)
            {
                Collider2D closestItem = FindClosestItem();
                if (closestItem != null && !isPickedUp)
                {
                    var itemPickup = closestItem.GetComponent<ItemPickup>();
                    var spellPickup = closestItem.GetComponent<SpellPickup>();

                    if (itemPickup != null)
                    {
                        itemPickup.Pickup();
                    }
                    if (spellPickup != null)
                    {
                        spellPickup.Pickup();
                    }

                    isPickedUp = true;
                }
            }
            else
            {
                isPickedUp = false;
            }
        }

        private Collider2D FindClosestItem()
        {
            Collider2D closestItem = null;
            float closestDistance = float.MaxValue;

            foreach (var item in potentialItems)
            {
                float distance = Vector2.Distance(transform.position, item.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = item;
                }
            }

            return closestItem;
        }
    }
}
