using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;
using Bardent.CoreSystem;
using Bardent.CoreSystem.StatsSystem;

namespace Inventory
{
    public class InventoryItemController : MonoBehaviour
    {
        Item item;

        [SerializeField] private Button removeButton;
        [SerializeField] private float equipmentCloseTime;

        private Button useButton;
        private Image itemImage;
        private TMP_Text itemName, itemPrice, itemValue, itemWeight, itemArmor, itemMagicRes, itemDescription;
        private GameObject description;

        private bool equipmentMenuActive = false;

        public void RemoveItem()
        {
            GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            ItemController itemController = button.GetComponentInParent<ItemController>();
            Item item = itemController.GetItem();

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
                        if (Stats.Instance.Health.CurrentValue != Stats.Instance.Health.MaxValue)
                        {
                            RemoveItem2(itemController.GetItem());

                            Stats.Instance.Health.Increase(itemController.GetItem().value);
                        }
                        break;
                }
            }
            else if (itemController.GetItem().equipment)
            {
                GameObject equipmentMenu = InventoryManager.Instance.GetEquipmentMenu();

                PlayerStats.Instance.RefreshStats();

                switch (itemController.GetItem().equipmentType)
                {
                    case Item.EquipmentType.Helmet:
                        foreach (Transform transform in InventoryManager.Instance.HelmetBn.transform)
                        {
                            transform.GetComponent<Image>().gameObject.SetActive(true);
                            transform.GetComponent<Image>().sprite = itemController.GetItem().icon;
                        }
                        InventoryManager.Instance.HelmetBn.GetComponent<ItemController>().SetItem(itemController.GetItem());
                        break;

                    case Item.EquipmentType.Chestplate:
                        foreach (Transform transform in InventoryManager.Instance.ChestplateBn.transform)
                        {
                            transform.GetComponent<Image>().gameObject.SetActive(true);
                            transform.GetComponent<Image>().sprite = itemController.GetItem().icon;
                        }
                        InventoryManager.Instance.ChestplateBn.GetComponent<ItemController>().SetItem(itemController.GetItem());
                        break;

                    case Item.EquipmentType.Leggings:
                        foreach (Transform transform in InventoryManager.Instance.BootsBn.transform)
                        {
                            transform.GetComponent<Image>().gameObject.SetActive(true);
                            transform.GetComponent<Image>().sprite = itemController.GetItem().icon;
                        }
                        InventoryManager.Instance.BootsBn.GetComponent<ItemController>().SetItem(itemController.GetItem());
                        break;

                    case Item.EquipmentType.Gloves:
                        foreach (Transform transform in InventoryManager.Instance.GlovesBn.transform)
                        {
                            transform.GetComponent<Image>().gameObject.SetActive(true);
                            transform.GetComponent<Image>().sprite = itemController.GetItem().icon;
                        }
                        InventoryManager.Instance.GlovesBn.GetComponent<ItemController>().SetItem(itemController.GetItem());
                        break;
                }
                if (button.GetComponentInChildren<TextMeshProUGUI>().text == "Unequip")
                {
                    description = InventoryManager.description1;

                    switch (button.GetComponent<ItemController>().GetItem().equipmentType)
                    {
                        case Item.EquipmentType.Helmet:
                            foreach (Transform transform in InventoryManager.Instance.HelmetBn.transform)
                            {
                                transform.GetComponent<Image>().gameObject.SetActive(false);
                                transform.GetComponent<Image>().sprite = null;
                            }
                            InventoryManager.Instance.HelmetBn.GetComponent<ItemController>().SetItem(null);
                            description.SetActive(false);
                            break;

                        case Item.EquipmentType.Chestplate:
                            foreach (Transform transform in InventoryManager.Instance.ChestplateBn.transform)
                            {
                                transform.GetComponent<Image>().gameObject.SetActive(false);
                                transform.GetComponent<Image>().sprite = null;
                            }
                            InventoryManager.Instance.ChestplateBn.GetComponent<ItemController>().SetItem(null);
                            description.SetActive(false);
                            break;

                        case Item.EquipmentType.Leggings:
                            foreach (Transform transform in InventoryManager.Instance.BootsBn.transform)
                            {
                                transform.GetComponent<Image>().gameObject.SetActive(false);
                                transform.GetComponent<Image>().sprite = null;
                            }
                            InventoryManager.Instance.BootsBn.GetComponent<ItemController>().SetItem(null);
                            description.SetActive(false);
                            break;

                        case Item.EquipmentType.Gloves:
                            foreach (Transform transform in InventoryManager.Instance.GlovesBn.transform)
                            {
                                transform.GetComponent<Image>().gameObject.SetActive(false);
                                transform.GetComponent<Image>().sprite = null;
                            }
                            InventoryManager.Instance.GlovesBn.GetComponent<ItemController>().SetItem(null);
                            description.SetActive(false);
                            break;
                    }
                }
            }
        }

        public void Description()
        {
            GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            ItemController itemController = button.GetComponent<ItemController>();

            GameObject descriptionExtension = InventoryManager.Instance.GetEquipmentMenu();
            Animator descriptionAnimator = InventoryManager.Instance.GetEquipmentMenuAnimator();

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
            if (itemController.GetItem().value != 0)
            {
                itemValue.gameObject.SetActive(true);
                itemValue.text = "VALUE - " + itemController.GetItem().value.ToString();
            }
            else itemValue.gameObject.SetActive(false);
            itemPrice.text = "PRICE - " + itemController.GetItem().cost.ToString();

            useButton = InventoryManager.useButton1;
            ItemController useButtonItemController = useButton.GetComponent<ItemController>();
            useButtonItemController.SetItem(itemController.GetItem());

            if (itemController.GetItem().usable)
                useButton.gameObject.SetActive(true);
            else useButton.gameObject.SetActive(false);

            if (itemController.GetItem().consumable)
            {
                useButton.GetComponentInChildren<TextMeshProUGUI>().text = "Use";
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

                descriptionExtension.SetActive(true);
                if (equipmentMenuActive == false)
                {
                    descriptionAnimator.SetTrigger("Open");
                    descriptionAnimator.SetBool("OpenClose", true);
                    Debug.Log("open");
                    equipmentMenuActive = true;
                }
            }
            else
            {
                itemWeight.gameObject.SetActive(false);
                itemArmor.gameObject.SetActive(false);
                itemMagicRes.gameObject.SetActive(false);

                descriptionAnimator.SetTrigger("Close");
                descriptionAnimator.SetBool("OpenClose", false);
                Debug.Log("close");
                Invoke("CloseEquipmentMenu", equipmentCloseTime);
                equipmentMenuActive = false;
            }
        }
        private void CloseEquipmentMenu()
        {
            GameObject descriptionExtension = InventoryManager.Instance.GetEquipmentMenu();
            descriptionExtension.SetActive(false);
        }
    }
}