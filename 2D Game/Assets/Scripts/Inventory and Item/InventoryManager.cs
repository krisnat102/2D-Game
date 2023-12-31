using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance;

        #region Private Variables
        [SerializeField] private List<Item> items = new();

        [SerializeField] private Transform itemContent;
        [SerializeField] private GameObject inventoryItem;

        [SerializeField] private Toggle enableRemove;

        [SerializeField] private InventoryItemController[] inventoryItems;

        [SerializeField] private GameObject inventory;
        [SerializeField] private GameObject spellInventory;

        [Header("Equipment MiniMenu")]
        [SerializeField] private Animator equipmentMenuAnimator;
        [SerializeField] private GameObject equipmentMenu;
        [SerializeField] private Button helmetBn;
        [SerializeField] private Button chestplateBn;
        [SerializeField] private Button glovesBn;
        [SerializeField] private Button bootsBn;

        [Header("Item Description")]
        [SerializeField] private Button useButton;
        [SerializeField] private Image itemImage;
        [SerializeField] private TMP_Text itemName, itemDescription, itemValue, itemPrice, itemWeight, itemArmor, itemMagicRes;
        [SerializeField] private GameObject description;
        
        private Filter filter = default;

        private float totalArmor;
        private float totalMagicRes;
        private float totalWeight;
        #endregion

        #region Property Variables
        public Button HelmetBn { get => helmetBn; set => helmetBn = value; }
        public Button ChestplateBn { get => chestplateBn; set => chestplateBn = value; }
        public Button GlovesBn { get => glovesBn; set => glovesBn = value; }
        public Button BootsBn { get => bootsBn; set => bootsBn = value; }
        public Button UseButton { get => useButton; set => useButton = value; }
        public Image ItemImage { get => itemImage; set => itemImage = value; }
        public TMP_Text ItemName { get => itemName; set => itemName = value; }
        public TMP_Text ItemDescription { get => itemDescription; set => itemDescription = value; }
        public TMP_Text ItemValue { get => itemValue; set => itemValue = value; }
        public TMP_Text ItemPrice { get => itemPrice; set => itemPrice = value; }
        public TMP_Text ItemWeight { get => itemWeight; set => itemWeight = value; }
        public TMP_Text ItemArmor { get => itemArmor; set => itemArmor = value; }
        public TMP_Text ItemMagicRes { get => itemMagicRes; set => itemMagicRes = value; }
        public GameObject Description { get => description; set => description = value; }
        #endregion

        #region Enums
        private enum Filter
        {
            Default,
            ConsumableInv,
            MaterialInv,
            EquimpentInv,
            QuestInv,
            MisctInv
        };
        #endregion

        #region Unity Methods
        private void Awake()
        {
            Instance = this;

            UseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Use";
        }

        public void Update()
        {
            if (Core.GameManager.gamePaused == false)
            {
                if (PlayerInputHandler.Instance.InventoryInput)
                {
                    PlayerInputHandler.Instance.UseInventoryInput();
                    if (!inventory.activeInHierarchy && !spellInventory.activeInHierarchy)
                    {
                        inventory.SetActive(true);

                        ListItems();

                        //Weapon.canFire = false;
                    }
                    else
                    {
                        inventory.SetActive(false);
                        spellInventory.SetActive(false);

                        //Weapon.canFire = true;
                    }
                }
            }
            if (PlayerInputHandler.Instance.MenuInput)
            {
                if (inventory.activeInHierarchy || spellInventory.activeInHierarchy)
                {
                    PlayerInputHandler.Instance.UseMenuInpit();

                    inventory.SetActive(false);
                    spellInventory.SetActive(false);

                    //Weapon.canFire = true;
                }
            }
        }
        #endregion

        #region Item Management Methods
        public void Add(Item item)
        {
            items.Add(item);
        }

        public void Remove(Item item)
        {
            items.Remove(item);
        }

        public void ListItems()
        {
            ItemController itemController;

            //clears the inventory before opening so that items dont duplicate
            foreach (Transform item in itemContent)
            {
                item.gameObject.SetActive(true);
                Destroy(item.gameObject);
            }

            //adds the items to the inventory
            foreach (var item in items)
            {
                GameObject obj = Instantiate(inventoryItem, itemContent);

                obj.SetActive(true);

                obj.name = item.name;

                itemController = obj.GetComponent<ItemController>();
                itemController.SetItem(item);

                var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
                var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
                var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();

                itemName.text = item.ItemName;
                itemIcon.sprite = item.icon;

                if (filter == Filter.ConsumableInv)
                    if (item.itemClass != Item.ItemClass.Consumable)
                    {
                        obj.SetActive(false);
                    }
                if (filter == Filter.MaterialInv)
                    if (item.itemClass != Item.ItemClass.Material)
                    {
                        obj.SetActive(false);
                    }
                if (filter == Filter.EquimpentInv)
                    if (item.itemClass != Item.ItemClass.Equipment)
                    {
                        obj.SetActive(false);
                    }
                if (filter == Filter.QuestInv)
                    if (item.itemClass != Item.ItemClass.Quest)
                    {
                        obj.SetActive(false);
                    }
                if (filter == Filter.MisctInv)
                    if (item.itemClass != Item.ItemClass.Misc)
                    {
                        obj.SetActive(false);
                    }

                if (enableRemove.isOn)
                {
                    removeButton.gameObject.SetActive(true);
                }
            }

            SetInventoryItems();
        }

        private void EnableItemRemove()
        {
            if (enableRemove.isOn)
            {
                foreach (Transform item in itemContent)
                {
                    item.transform.Find("RemoveButton").GetComponent<Button>().gameObject.SetActive(true);
                }
            }
            else
            {
                foreach (Transform item in itemContent)
                {
                    item.Find("RemoveButton").gameObject.SetActive(false);
                }
            }
            ListItems();
        }

        private void SetInventoryItems()
        {
            inventoryItems = itemContent.GetComponentsInChildren<InventoryItemController>();
            System.Array.Resize(ref inventoryItems, items.Count);

            for (int i = 0; i < items.Count; i++)
            {
                inventoryItems[i].AddItem(items[i]);
            }
        }
        #endregion

        #region Equipment Stat Methods
        public List<float> GetEquipmentStats()
        {
            List<float> totalStats = new();
            totalStats.Add(totalArmor); totalStats.Add(totalMagicRes); totalStats.Add(totalWeight);
            return totalStats;
        }

        public void AddItemStats(Item item)
        {
            totalArmor += item.armor;
            totalWeight += item.weight;
            totalMagicRes += item.magicRes;

            Debug.Log(totalMagicRes + " " + totalArmor + " " + totalWeight);
        }
        public void RemoveItemStats(Item item)
        {
            totalArmor -= item.armor;
            totalWeight -= item.weight;
            totalMagicRes -= item.magicRes;

            Debug.Log(totalMagicRes + " " + totalArmor + " " + totalWeight);
        }
        #endregion

        #region Buttons
        public void EquipedEquipmentBn()
        {
            UseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unequip";
        }


        public void ConsumableInvBn()
        {
            filter = Filter.ConsumableInv;

            ListItems();
        }
        public void MaterialInvBn()
        {
            filter = Filter.MaterialInv;

            ListItems();
        }
        public void EquipmentInvBn()
        {
            filter = Filter.EquimpentInv;

            ListItems();

        }
        public void QuestInvBn()
        {
            filter = Filter.QuestInv;

            ListItems();
        }
        public void MiscInvBn()
        {
            filter = Filter.MisctInv;

            ListItems();
        }

        public Animator GetEquipmentMenuAnimator()
        {
            return equipmentMenuAnimator;
        }
        public GameObject GetEquipmentMenu()
        {
            return equipmentMenu;
        }
        #endregion
    }
}