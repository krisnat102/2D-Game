using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Krisnat;
using UnityEditor;
using System.Collections;

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

        [SerializeField] private GameObject inventory, spellInventory, characterTab;

        [Header("Equipment MiniMenu")]
        [SerializeField] private Animator equipmentMenuAnimator;
        [SerializeField] private GameObject equipmentMenu;
        [SerializeField] private Button helmetBn, chestplateBn, glovesBn, bootsBn;

        [Header("Item Description")]
        [SerializeField] private Button useButton;
        [SerializeField] private Image itemImage;
        [SerializeField] private TMP_Text itemName, itemDescription, itemValue, itemPrice, itemWeight, itemArmor, itemMagicRes;
        [SerializeField] private GameObject description;

        [Header("Coins")]
        [SerializeField] private TMP_Text coinCounter, levelUpCoinCounter, inventoryCoinCounter;
        [SerializeField] private float purseAnimationDistance, purseAnimationDuration, purseAnimationTimeOnScreen;

        private Filter filter = default;
        private float totalArmor, totalMagicRes, totalWeight;
        private List<Item> distinctItems, duplicates = new();
        private List<Item> allItems = new();
        private bool coinAnimationTracker;
        #endregion

        #region Property Variables
        public int Coins { get; private set; }
        public bool InventoryActiveInHierarchy { get; private set; }
        public bool SpellInventoryActiveInHierarchy { get; private set; }
        public bool CharacterTabActiveInHierarchy { get; private set; }
        public bool Shop { get; set; }
        public Button HelmetBn { get => helmetBn; private set => helmetBn = value; }
        public Button ChestplateBn { get => chestplateBn; private set => chestplateBn = value; }
        public Button GlovesBn { get => glovesBn; private set => glovesBn = value; }
        public Button BootsBn { get => bootsBn; private set => bootsBn = value; }
        public Button UseButton { get => useButton; private set => useButton = value; }
        public Image ItemImage { get => itemImage; private set => itemImage = value; }
        public TMP_Text ItemName { get => itemName; private set => itemName = value; }
        public TMP_Text ItemDescription { get => itemDescription; private set => itemDescription = value; }
        public TMP_Text ItemValue { get => itemValue; private set => itemValue = value; }
        public TMP_Text ItemPrice { get => itemPrice; private set => itemPrice = value; }
        public TMP_Text ItemWeight { get => itemWeight; private set => itemWeight = value; }
        public TMP_Text ItemArmor { get => itemArmor; private set => itemArmor = value; }
        public TMP_Text ItemMagicRes { get => itemMagicRes; private set => itemMagicRes = value; }
        public GameObject Description { get => description; private set => description = value; }
        public float TotalArmor { get => totalArmor; private set => totalArmor = value; }
        public float TotalMagicRes { get => totalMagicRes; private set => totalMagicRes = value; }
        public float TotalWeight { get => totalWeight; private set => totalWeight = value; }
        public List<Item> AllItems { get => allItems; private set => allItems = value; }
        public List<Item> DistinctItems { get => distinctItems; private set => distinctItems = value; }
        public List<Item> Duplicates { get => duplicates; private set => duplicates = value; }
        public List<Item> Items { get => items; private set => items = value; }
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
        private void Start()
        {
            AllItems = Core.GameManager.Instance.GetCustomAssets<Item>("Item", "CreatedAssets");
        }

        public void Update()
        {
            InventoryActiveInHierarchy = inventory.activeInHierarchy;
            SpellInventoryActiveInHierarchy = spellInventory.activeInHierarchy;
            CharacterTabActiveInHierarchy = characterTab.activeInHierarchy;

            coinCounter.text = Coins.ToString();
            inventoryCoinCounter.text = Coins.ToString();
            levelUpCoinCounter.text = Coins.ToString();

            if (PlayerInputHandler.Instance.InventoryInput)
            {
                PlayerInputHandler.Instance.UseInventoryInput();
                if (!InventoryActiveInHierarchy && !SpellInventoryActiveInHierarchy && !CharacterTabActiveInHierarchy)
                {
                    OpenCloseInventory(true);
                }
                else
                {
                    OpenCloseInventory(false);

                    //Weapon.canFire = true;
                }
            }

            if (PlayerInputHandler.Instance.MenuInput)
            {
                if (InventoryActiveInHierarchy || SpellInventoryActiveInHierarchy || CharacterTabActiveInHierarchy)
                {
                    PlayerInputHandler.Instance.UseMenuInpit();

                    inventory.SetActive(false);
                    spellInventory.SetActive(false);

                    //Weapon.canFire = true;
                }
            }
        }
        private void OnDestroy()
        {
            foreach (Item item in Items)
            {
                item.SetEquipped(false);
                item.SetItemCount(1);
            }
        }
        #endregion

        #region Inventory Methods
        public void OpenCloseInventory(bool openOrClose)
        {
            if (openOrClose)
            {
                inventory.SetActive(true);

                //TODO: Optimize this awful code out of the game
                //(done 2 times because of a bug with the item count upon first openning on inv)
                ListItems();
                ListItems();
            }
            else
            {
                inventory.SetActive(false);
                spellInventory.SetActive(false);
                characterTab.SetActive(false);

                if (Shop)
                {
                    Shop = false;
                }
            }
        }
        #endregion

        #region Item Management Methods
        public void Add(Item item)
        {
            Items.Add(item);
        }
        public void Add(List<Item> itemsToAdd)
        {
            Items.AddRange(itemsToAdd);
        }
        public void Remove(Item item)
        {
            Items.Remove(item);
        }
        public void ClearInventory()
        {
            Items.Clear();
            ListItems();
        }

        public void ListItems()
        {
            DistinctItems = Items.GroupBy(x => x.name).Select(y => y.First()).ToList();
            Duplicates = Items.GroupBy(p => new { p.name })
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1))
                .ToList();

            //clears the inventory before opening so that items dont duplicate
            foreach (Transform item in itemContent)
            {
                item.gameObject.SetActive(true);
                item.GetComponent<ItemController>().GetItem().SetItemCount(1);
                Destroy(item.gameObject);
            }

            //adds the items to the inventory
            foreach (var item in DistinctItems)
            {
                foreach (var duplicateItem in Duplicates)
                {
                    if (item.name == duplicateItem.name)
                        item.IncreaseItemCount();
                }

                GameObject obj = Instantiate(inventoryItem, itemContent);

                obj.SetActive(true);

                obj.name = item.name;

                ItemController itemController = obj.GetComponent<ItemController>();
                itemController.SetItem(item);

                var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
                var itemCount = obj.transform.Find("ItemCount").GetComponent<TextMeshProUGUI>();
                var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
                var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();

                itemName.text = item.itemName;
                itemCount.text = (item.ItemCount > 1 ? "x" + item.ItemCount.ToString() : "");
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
            System.Array.Resize(ref inventoryItems, DistinctItems.Count);

            for (int i = 0; i < DistinctItems.Count; i++)
            {
                inventoryItems[i].AddItem(DistinctItems[i]);
            }
        }
        #endregion

        #region Equipment Stat Methods
        public void AddItemStats(Item item)
        {
            TotalArmor += item.armor;
            TotalWeight += item.weight;
            TotalMagicRes += item.magicRes;

            Debug.Log(TotalMagicRes + " " + TotalArmor + " " + TotalWeight);
        }
        public void RemoveItemStats(Item item)
        {
            TotalArmor -= item.armor;
            TotalWeight -= item.weight;
            TotalMagicRes -= item.magicRes;

            Debug.Log(TotalMagicRes + " " + TotalArmor + " " + TotalWeight);
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

        #region Coin Methods

        public void SetCoins(int i, bool animation)
        {
            Coins = i;

            if (coinAnimationTracker == false && animation)
            {
                StartCoinAnimation();
            }
        }
        public void IncreaseCoins(bool animation)
        {
            Coins++;

            if (coinAnimationTracker == false && animation)
            {
                StartCoinAnimation();
            }
        }

        public void StartCoinAnimation()
        {
            coinAnimationTracker = true;
            UIManager.Instance.MovePurseAnimation(true, purseAnimationDistance, purseAnimationDuration);
            Invoke("EndCoinAnimation", purseAnimationTimeOnScreen);
        }
        private void EndCoinAnimation()
        {
            coinAnimationTracker = false;
            UIManager.Instance.MovePurseAnimation(false, purseAnimationDistance, purseAnimationDuration);
        }

        #endregion
    }
}