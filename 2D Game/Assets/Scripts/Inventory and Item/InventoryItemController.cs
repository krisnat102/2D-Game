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

        Destroy(this.gameObject);
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
    }

    public void UseItem()
    {
        GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        ItemController itemController = button.GetComponent<ItemController>();

        if (itemController.Item.usable)
        {
            switch (itemController.Item.itemType)
            {
                case Item.ItemType.Potion:
                    if (PlayerStats.maxHP != PlayerStats.hp)
                    {
                        RemoveItem();
                    }

                    PlayerStats.Instance.Heal(itemController.Item.value);
                    break;
                case Item.ItemType.Food:
                    if (PlayerStats.maxHP != PlayerStats.hp)
                    {
                        RemoveItem();
                    }

                    PlayerStats.Instance.Heal(itemController.Item.value);
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

        itemImage.sprite = itemController.Item.icon;
        itemName.text = itemController.Item.ItemName.ToUpper();
        itemDescription.text = itemController.Item.itemDescription;
        if(itemController.Item.value != 0)
        {
            itemValue.text = "VALUE - " + itemController.Item.value.ToString();
        }
        else itemValue.text = null;
        itemPrice.text = "PRICE - "  + itemController.Item.cost.ToString();

        useButton = InventoryManager.useButton1;
        ItemController useButtonItemController = useButton.GetComponent<ItemController>();
        useButtonItemController.Item = itemController.Item;

        if (itemController.Item.usable)
        {
            useButton.gameObject.SetActive(true);
        }
        else useButton.gameObject.SetActive(false);
    }
}
