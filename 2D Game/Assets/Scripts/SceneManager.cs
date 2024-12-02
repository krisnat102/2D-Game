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

        //private static bool itemTutorial = false;
        //private static bool spellTutorial = false;
        //private static bool started = false;

        private void Start()
        {
            //if (!started)
            //{
                //ClearInventory();
                //AddItems();

                //started = true;
            //}
        }

        private void Update()
        {
            if (PlayerInputHandler.Instance.Test1Input)
            {
                //AddItems();
                //PlayerInputHandler.Instance.UseTest1Input();
            }
            if (PlayerInputHandler.Instance.Test2Input)
            {
                //Test2();
                //PlayerInputHandler.Instance.UseTest2Input();
            }
        }

        private void AddItems()
        {
            InventoryManager.Instance.Add(itemsToStartWith);
            InventoryManager.Instance.Add(itemsToEquip);

            foreach (Item item in itemsToEquip)
            {
                InventoryManager.Instance.EquipItem(item);
            }
        }

        private void ClearInventory()
        {
            InventoryManager.Instance.ClearInventory();
            SpellManager.Instance.ClearInventory();
            SpellManager.Instance.ClearActiveBar();
        }
    }
}
