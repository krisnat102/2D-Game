using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemController : MonoBehaviour
{
    Item item;

    [SerializeField] private Button removeButton;
    
    private Button useButton;
    private Image itemImage;
    private TMP_Text itemName, itemPrice, itemValue, itemDescription;
    private GameObject description;

    public void RemoveItem()
    {
        InventoryManager.Instance.Remove(item);

        Destroy(gameObject);
    }
    public void RemoveItem2(Item item)
    {
        InventoryManager.Instance.Remove(item);

        GameObject itemToDelete = GameObject.Find(item.name);
        if (itemToDelete != null) Destroy(itemToDelete);
        else Debug.Log("not found item to delete");

        InventoryManager.description1.SetActive(false);
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
    }

    public void UseItem()
    {
        GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        ItemController itemController = button.GetComponent<ItemController>();

        if (itemController.item.usable)
        {
            switch (itemController.item.itemType)
            {
                case Item.ItemType.Potion:
                    if (PlayerStats.maxHP != PlayerStats.hp)
                    {
                        RemoveItem2(itemController.item);
                    }

                    PlayerStats.Instance.Heal(itemController.item.value);
                    break;
                case Item.ItemType.Food:
                    if (PlayerStats.maxHP != PlayerStats.hp)
                    {
                        RemoveItem2(itemController.item);
                    }

                    PlayerStats.Instance.Heal(itemController.item.value);
                    break;
            }
        }
    }

    public void Description()
    {
        GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        ItemController itemController = button.GetComponent<ItemController>();

        description = InventoryManager.description1;
        description.SetActive(true);

        itemImage = InventoryManager.itemImage1;
        itemName = InventoryManager.itemName1;
        itemDescription = InventoryManager.itemDescription1;
        itemValue = InventoryManager.itemValue1;
        itemPrice = InventoryManager.itemPrice1;

        itemImage.sprite = itemController.item.icon;
        itemName.text = itemController.item.ItemName.ToUpper();
        itemDescription.text = itemController.item.itemDescription;
        if(itemController.item.value != 0)
        {
            itemValue.text = "VALUE - " + itemController.item.value.ToString();
        }
        else itemValue.text = null;
        itemPrice.text = "PRICE - "  + itemController.item.cost.ToString();

        useButton = InventoryManager.useButton1;
        ItemController useButtonItemController = useButton.GetComponent<ItemController>();
        useButtonItemController.item = itemController.item;

        if (itemController.item.usable)
        {
            useButton.gameObject.SetActive(true);
        }
        else useButton.gameObject.SetActive(false);
    }
}
