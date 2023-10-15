using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    Item item;

    public Button RemoveButton;
    public void RemoveItem()
    {
        InventoryManager.Instance.Remove(item);

        Destroy(gameObject);
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
    }

    public void UseItem()
    {
        if(item.usable == true)
        {
            switch (item.itemType)
            {
                case Item.ItemType.Potion:
                    if (PlayerStats.maxHP != PlayerStats.hp)
                    {
                        RemoveItem();
                    }

                    PlayerStats.Instance.Heal(item.value);
                    break;
                case Item.ItemType.Food:
                    if (PlayerStats.maxHP != PlayerStats.hp)
                    {
                        RemoveItem();
                    }

                    PlayerStats.Instance.Heal(item.value);
                    break;
            }
        }
    }
}
