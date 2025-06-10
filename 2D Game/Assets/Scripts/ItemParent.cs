using Inventory;
using Spells;
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

            List<SpellPickup> spells = new List<SpellPickup>();

            spells = GetComponentsInChildren<SpellPickup>(true).ToList();

            foreach (ItemPickup item in items)
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

            foreach (SpellPickup spell in spells)
            {
                if (CoreClass.GameManager.Instance.ItemsTaken.Contains(spell.ItemId))
                {
                    spell.gameObject.SetActive(false);
                }
                else
                {
                    spell.gameObject.SetActive(true);
                }
            }
        }
    }
}
