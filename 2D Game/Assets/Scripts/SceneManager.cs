using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class SceneManager : MonoBehaviour
    {
        [SerializeField] private List<Item> itemsToStartWith;
        [SerializeField] private List<Item> itemsToEquip;

        private void Start()
        {
            InventoryManager.Instance.Add(itemsToStartWith);
            InventoryManager.Instance.Add(itemsToEquip);

            foreach(Item item in itemsToEquip) 
            {
                InventoryManager.Instance.EquipItem(item);
            }
        }
    }
}
