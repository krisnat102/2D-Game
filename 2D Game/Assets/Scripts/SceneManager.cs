using Inventory;
using Spells;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class SceneManager : MonoBehaviour
    {
        [SerializeField] private List<Item> itemsToStartWith;
        [SerializeField] private List<Item> itemsToEquip;

        private void Update()
        {
            if (PlayerInputHandler.Instance.Test1Input)
            {
                Test1();
                PlayerInputHandler.Instance.UseTest1Input();
            }
            if (PlayerInputHandler.Instance.Test2Input)
            {
                Test2();
                PlayerInputHandler.Instance.UseTest2Input();
            }
        }

        private void Test1()
        {
            InventoryManager.Instance.Add(itemsToStartWith);
            InventoryManager.Instance.Add(itemsToEquip);

            foreach (Item item in itemsToEquip)
            {
                InventoryManager.Instance.EquipItem(item);
            }
        }

        private void Test2()
        {
            InventoryManager.Instance.ClearInventory();
            SpellManager.Instance.ClearInventory();
        }
    }
}
