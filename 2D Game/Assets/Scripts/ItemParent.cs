using Inventory;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Krisnat
{
    public class ItemParent : MonoBehaviour
    {
        void Start()
        {
            List<ItemPickup> items = new List<ItemPickup>();

            items = GetComponentsInChildren<ItemPickup>(true).ToList();

            foreach(ItemPickup item in items)
            {
                if (CoreClass.GameManager.Instance.ItemsTaken.Contains(item.ItemId))
                {
                    item.gameObject.SetActive(false);
                }
                else
                {
                    item.gameObject.SetActive(true);
                }
            }
        }
    }
}
