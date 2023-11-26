using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemController : MonoBehaviour
{
    Item item;

    [SerializeField] private Button removeButton;
    
    private Button useButton;
    private Image itemImage;
    private TMP_Text itemName, itemPrice, itemValue, itemWeight, itemArmor, itemMagicRes, itemDescription;
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

        if (itemController.GetItem().consumable)
        {
            switch (itemController.GetItem().consumableType)
            {
                case Item.ConsumableType.Heal:
                    if (PlayerStats.maxHP != PlayerStats.hp)
                    {
                        RemoveItem2(itemController.GetItem());
                    }

                    PlayerStats.Instance.Heal(itemController.GetItem().value);
                    break;
            }
        }
        if (itemController.GetItem().equipment)
        {
            switch (itemController.GetItem().equipmentType)
            {
                case Item.EquipmentType.Helmet:
                    Debug.Log("Helmet");
                    break;
                case Item.EquipmentType.Chestplate:
                    Debug.Log("Chestplate");
                    break;
                case Item.EquipmentType.Leggings:
                    Debug.Log("Leggings");
                    break;
                case Item.EquipmentType.Gloves:
                    Debug.Log("Gloves");
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
        itemWeight = InventoryManager.itemWeight1;
        itemArmor = InventoryManager.itemArmor1;
        itemMagicRes = InventoryManager.itemMagicRes1;

        itemImage.sprite = itemController.GetItem().icon;
        itemName.text = itemController.GetItem().ItemName.ToUpper();
        itemDescription.text = itemController.GetItem().itemDescription;
        if(itemController.GetItem().value != 0)
        {
            itemValue.gameObject.SetActive(true);
            itemValue.text = "VALUE - " + itemController.GetItem().value.ToString();
        }
        else itemValue.gameObject.SetActive(false);
        itemPrice.text = "PRICE - "  + itemController.GetItem().cost.ToString();

        useButton = InventoryManager.useButton1;
        ItemController useButtonItemController = useButton.GetComponent<ItemController>();
        useButtonItemController.SetItem(itemController.GetItem());

        if (itemController.GetItem().usable)
            useButton.gameObject.SetActive(true);
        else useButton.gameObject.SetActive(false);

        if (itemController.GetItem().consumable)
        {
            useButton.GetComponentInChildren<Text>().text = "Use";
        }

        if (itemController.GetItem().equipment)
        {
            itemWeight.gameObject.SetActive(true);
            itemArmor.gameObject.SetActive(true);
            itemMagicRes.gameObject.SetActive(true);

            itemWeight.text = "WEIGHT - " + itemController.GetItem().weight.ToString();
            itemArmor.text = "ARMOR - " + itemController.GetItem().armor.ToString();
            itemMagicRes.text = "MAGIC RES - " + itemController.GetItem().magicRes.ToString();

            useButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
        }
    }
}
