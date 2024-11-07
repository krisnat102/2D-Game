using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Krisnat;
using UnityEditor;
using Spells;
using UnityEngine.UI;
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

        [Header("Animations")]
        [SerializeField] private float inventoryOpeningSpeed = 2f;
        [SerializeField] private float inventoryClosingSpeed = 2f;

        [SerializeField] private float characterTabOpeningSpeed = 2f;
        [SerializeField] private float characterTabClosingSpeed = 2f;

        [Header("Equipment MiniMenu")]
        [SerializeField] private Animator equipmentMenuAnimator;
        [SerializeField] private GameObject equipmentMenu;
        [SerializeField] private Button helmetBn, chestplateBn, glovesBn, bootsBn, swordBn, bowBn;

        [Header("Item Description")]
        [SerializeField] private Button useButton;
        [SerializeField] private Image itemImage;
        [SerializeField] private TMP_Text itemName, itemDescription, itemValue, itemPrice, itemWeight, itemArmor, itemMagicRes, weaponAttack;
        [SerializeField] private GameObject description;

        [Header("Coins")]
        [SerializeField] private GameObject purse;
        [SerializeField] private TMP_Text coinCounter, levelUpCoinCounter, inventoryCoinCounter; 
        [SerializeField] private float purseAnimationDistance, purseAnimationDuration, purseAnimationTimeOnScreen;

        private Filter filter = default;
        private float totalArmor, totalMagicRes, totalWeight;
        private List<Item> distinctItems, duplicates, currentItems, allItems = new List<Item>();
        private float inventoryScale, characterTabScale;
        private Animator purseAnimator;
        //private bool inventoryOpenAnimationTracker;
        //private bool inventoryCloseAnimationTracker;
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
        public Button SwordBn { get => swordBn; private set => swordBn = value; }
        public Button BowBn { get => bowBn; private set => bowBn = value; }
        public Button UseButton { get => useButton; private set => useButton = value; }
        public Image ItemImage { get => itemImage; private set => itemImage = value; }
        public TMP_Text ItemName { get => itemName; private set => itemName = value; }
        public TMP_Text ItemDescription { get => itemDescription; private set => itemDescription = value; }
        public TMP_Text ItemValue { get => itemValue; private set => itemValue = value; }
        public TMP_Text ItemPrice { get => itemPrice; private set => itemPrice = value; }
        public TMP_Text ItemWeight { get => itemWeight; private set => itemWeight = value; }
        public TMP_Text ItemArmor { get => itemArmor; private set => itemArmor = value; }
        public TMP_Text ItemMagicRes { get => itemMagicRes; private set => itemMagicRes = value; }
        public TMP_Text WeaponAttack { get => weaponAttack; private set => weaponAttack = value; }
        public GameObject Description { get => description; private set => description = value; }
        public float TotalArmor { get => totalArmor; private set => totalArmor = value; }
        public float TotalMagicRes { get => totalMagicRes; private set => totalMagicRes = value; }
        public float TotalWeight { get => totalWeight; private set => totalWeight = value; }
        public List<Item> AllItems { get => allItems; private set => allItems = value; }
        public List<Item> DistinctItems { get => distinctItems; private set => distinctItems = value; }
        public List<Item> Duplicates { get => duplicates; private set => duplicates = value; }
        public List<Item> Items { get => items; private set => items = value; }
        public GameObject Inventory { get => inventory; private set => inventory = value; }
        public GameObject SpellInventory { get => spellInventory; private set => spellInventory = value; }
        public GameObject CharacterTab { get => characterTab; private set => characterTab = value; }
        #endregion

        #region Enums
        private enum Filter
        {
            Default,
            ConsumableInv,
            EquipmentInv,
            WeaponInv,
            QuestInv,
            MiscInv
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

            inventoryScale = inventory.transform.localScale.x;
            characterTabScale = characterTab.transform.localScale.x;

            purseAnimator = purse.GetComponent<Animator>();
        }

        public void Update()
        {
            InventoryActiveInHierarchy = Inventory.activeInHierarchy;
            SpellInventoryActiveInHierarchy = SpellInventory.activeInHierarchy;
            CharacterTabActiveInHierarchy = CharacterTab.activeInHierarchy;

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
                else if (CharacterTabActiveInHierarchy)
                {
                    OpenCloseCharacterTab(false);
                }
                else if (SpellInventoryActiveInHierarchy)
                {
                    SpellManager.Instance.OpenCloseSpellInventory(false);
                }
                else
                {
                    OpenCloseInventory(false);
                }
            }
            if (PlayerInputHandler.Instance.CharacterTabInput)
            {
                PlayerInputHandler.Instance.UseCharacterTabInput();
                if (!InventoryActiveInHierarchy && !SpellInventoryActiveInHierarchy && !CharacterTabActiveInHierarchy)
                {
                    OpenCloseCharacterTab(true);
                }
                else if(InventoryActiveInHierarchy)
                {
                    OpenCloseInventory(false);
                }
                else if (SpellInventoryActiveInHierarchy)
                {
                    SpellManager.Instance.OpenCloseSpellInventory(false);
                }
                else
                {
                    OpenCloseCharacterTab(false);
                }
            }

            if (PlayerInputHandler.Instance.MenuInput)
            {
                if (InventoryActiveInHierarchy)
                {
                    OpenCloseInventory(false);
                }
                else if (SpellInventoryActiveInHierarchy)
                {
                    SpellManager.Instance.OpenCloseSpellInventory(false);
                }
                else if (CharacterTabActiveInHierarchy)
                {
                    OpenCloseCharacterTab(false);
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
                UIManager.Instance.OpenCloseUI(Inventory, inventoryScale, inventoryOpeningSpeed, true, false, true);

                //TODO: Optimize this awful code out of the game
                //(done 2 times because of a bug with the item count upon first opening on inv)
                ListItems();
                //ListItems();
                //Core.GameManager.Instance.ChangeBool(0.25f, newValue => Core.GameManager.Instance.GamePaused = newValue[0], true);
            }
            else
            {
                GameObject[] uiToClose = new GameObject[2];
                uiToClose[0] = SpellInventory; uiToClose[1] = CharacterTab;
                UIManager.Instance.OpenCloseUI(Inventory, inventoryScale, inventoryClosingSpeed, true, false, false, uiToClose);
                //Core.GameManager.Instance.GamePaused = false;
            }
        }
        

        public void OpenCloseCharacterTab(bool openOrClose)
        {
            if (openOrClose)
            {
                UIManager.Instance.OpenCloseUI(CharacterTab, characterTabScale, characterTabOpeningSpeed, true, false, true);
                Core.GameManager.Instance.ChangeBool(0.25f, newValue => Core.GameManager.Instance.GamePaused = newValue[0], true);
            }
            else
            {
                GameObject[] uiToClose = new GameObject[2];
                uiToClose[0] = SpellInventory; uiToClose[1] = Inventory;
                UIManager.Instance.OpenCloseUI(CharacterTab, characterTabScale, characterTabClosingSpeed, true, false, false, uiToClose);
                Core.GameManager.Instance.GamePaused = false;
                description.SetActive(false);
            }
        }
        #endregion

        #region Item Management Methods
        public void Add(Item item, bool uiSpawner)
        {
            Items.Add(item);

            if (uiSpawner)
            {
                int resolutionHeight = Screen.currentResolution.height;

                var itemPopUp = Instantiate(UIManager.Instance.ItemPickupPopUp, UIManager.Instance.Canvas.transform).GetComponent<PopUpUI>();

                foreach (var ui in ItemPickup.itemPopUps)
                {
                    ui.GoUp();
                }

                ItemPickup.itemPopUps.Add(itemPopUp);

                switch (resolutionHeight)
                {
                    case <= 720:
                        itemPopUp.transform.position = itemPopUp.transform.position = UIManager.Instance.Canvas.transform.position + new Vector3(0, -350, 0);
                        break;
                    case <= 1080:
                        itemPopUp.transform.position = itemPopUp.transform.position = UIManager.Instance.Canvas.transform.position + new Vector3(0, -400, 0);
                        break;
                    case <= 1440:
                        itemPopUp.transform.position = UIManager.Instance.Canvas.transform.position + new Vector3(0, -450, 0);
                        break;
                    case <= 2160:
                        itemPopUp.transform.position = UIManager.Instance.Canvas.transform.position + new Vector3(0, -500, 0);
                        break;
                    case > 2160:
                        itemPopUp.transform.position = UIManager.Instance.Canvas.transform.position + new Vector3(0, -550, 0);
                        break;
                }
                itemPopUp.GetComponentsInChildren<Image>()[1].sprite = item.icon;
                itemPopUp.GetComponentInChildren<TMP_Text>().text = item.itemName;
            }
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

            if (currentItems != null)
            {
                currentItems.Clear();
            }

            //clears the inventory before opening so that items dont duplicate
            currentItems = new();

            foreach (Transform item in itemContent)
            {
                currentItems.Add(item.GetComponent<ItemController>().GetItem());
            }

            if (currentItems != null)
            {
                foreach (var item in DistinctItems.Except(currentItems))
                {
                    GameObject obj = CreateItem(item);
                }

                foreach (var item in currentItems.Except(DistinctItems))
                {
                    GameObject obj = CreateItem(item);
                }
            }
            else
            {
                foreach (var item in DistinctItems)
                {
                    GameObject obj = CreateItem(item);
                }
            }

            //adds the items to the inventory
            foreach (var item in DistinctItems)
            {
                foreach (var duplicateItem in Duplicates)
                {
                    if (item.name == duplicateItem.name)
                        item.IncreaseItemCount();
                }
            }

            foreach (Transform trans in itemContent)
            {
                var item = trans.GetComponent<ItemController>().GetItem();
                var itemCount = trans.GetComponentInChildren<SearchAssist>().GetComponent<TextMeshProUGUI>();
                itemCount.text = (item.ItemCount > 1 ? "x" + item.ItemCount.ToString() : "");

                if (enableRemove.isOn)
                {
                    var removeButton = trans.GetComponentInChildren<Button>();
                    removeButton.gameObject.SetActive(true);
                }
            }

            SetInventoryItems();
        }

        private GameObject CreateItem(Item item)
        {
            GameObject obj = Instantiate(inventoryItem, itemContent);

            obj.SetActive(true);

            obj.name = item.name;

            ItemController itemController = obj.GetComponent<ItemController>();
            itemController.SetItem(item);

            var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            //var itemCount = obj.transform.Find("ItemCount").GetComponent<TextMeshProUGUI>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();

            itemName.text = item.itemName;
            //itemCount.text = (item.ItemCount > 1 ? "x" + item.ItemCount.ToString() : "");
            itemIcon.sprite = item.icon;

            if (filter == Filter.ConsumableInv)
                if (item.itemClass != Item.ItemClass.Consumable)
                {
                    obj.SetActive(false);
                }
            if (filter == Filter.WeaponInv)
                if (item.itemClass != Item.ItemClass.Weapon)
                {
                    obj.SetActive(false);
                }
            if (filter == Filter.EquipmentInv)
                if (item.itemClass != Item.ItemClass.Equipment)
                {
                    obj.SetActive(false);
                }
            if (filter == Filter.QuestInv)
                if (item.itemClass != Item.ItemClass.Quest)
                {
                    obj.SetActive(false);
                }
            if (filter == Filter.MiscInv)
                if (item.itemClass != Item.ItemClass.Misc)
                {
                    obj.SetActive(false);
                }

            return obj;
        }

        private void EnableItemRemove()
        {
            if (enableRemove.isOn)
            {
                foreach (Transform item in itemContent)
                {
                    item.transform.Find("RemoveButton").GetComponent<UnityEngine.UI.Button>().gameObject.SetActive(true);
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

        public void DisableSelectedIndicators()
        {
            foreach(Transform item in itemContent)
            {
                item.GetComponent<InventoryItemController>().SelectedItemIndicator.gameObject.SetActive(false);
            }
        }

        public int[] EquippedItemsIds()
        {
            int[] ids = new int[6];

            ids[0] = HelmetBn.GetComponent<ItemController>()?.GetItem()?.id ?? 0;
            ids[1] = ChestplateBn.GetComponent<ItemController>()?.GetItem()?.id ?? 0;
            ids[2] = GlovesBn.GetComponent<ItemController>()?.GetItem()?.id ?? 0;
            ids[3] = BootsBn.GetComponent<ItemController>()?.GetItem()?.id ?? 0;
            ids[4] = SwordBn.GetComponent<ItemController>()?.GetItem()?.id ?? 0;
            ids[5] = BowBn.GetComponent<ItemController>()?.GetItem()?.id ?? 0;

            return ids;
        }

        public void EquipItem(Item item)
        {
            if (!Items.Contains(item)) return;

            item.SetEquipped(true);

            AddItemStats(item);

            switch (item.equipmentType)
            {
                case Item.EquipmentType.Helmet:
                    foreach (Transform transform in HelmetBn.transform)
                    {
                        transform.GetComponent<Image>().gameObject.SetActive(true);
                        transform.GetComponent<Image>().sprite = item.icon;
                    }

                    if (HelmetBn.GetComponent<ItemController>().GetItem() != null)
                    {
                        HelmetBn.GetComponent<ItemController>().GetItem().SetEquipped(false);
                    }

                    HelmetBn.GetComponent<ItemController>().SetItem(item);
                    break;

                case Item.EquipmentType.Chestplate:
                    foreach (Transform transform in ChestplateBn.transform)
                    {
                        transform.GetComponent<Image>().gameObject.SetActive(true);
                        transform.GetComponent<Image>().sprite = item.icon;
                    }

                    if (ChestplateBn.GetComponent<ItemController>().GetItem() != null)
                    {
                        ChestplateBn.GetComponent<ItemController>().GetItem().SetEquipped(false);
                    }

                    ChestplateBn.GetComponent<ItemController>().SetItem(item);
                    break;

                case Item.EquipmentType.Leggings:
                    foreach (Transform transform in BootsBn.transform)
                    {
                        transform.GetComponent<Image>().gameObject.SetActive(true);
                        transform.GetComponent<Image>().sprite = item.icon;
                    }

                    if (BootsBn.GetComponent<ItemController>().GetItem() != null)
                    {
                        BootsBn.GetComponent<ItemController>().GetItem().SetEquipped(false);
                    }

                    BootsBn.GetComponent<ItemController>().SetItem(item);
                    break;

                case Item.EquipmentType.Gloves:
                    foreach (Transform transform in GlovesBn.transform)
                    {
                        transform.GetComponent<Image>().gameObject.SetActive(true);
                        transform.GetComponent<Image>().sprite = item.icon;
                    }

                    if (GlovesBn.GetComponent<ItemController>().GetItem() != null)
                    {
                        GlovesBn.GetComponent<ItemController>().GetItem().SetEquipped(false);
                    }

                    GlovesBn.GetComponent<ItemController>().SetItem(item);
                    break;
                case Item.EquipmentType.Weapon:
                    switch (item.weaponType)
                    {
                        case Item.WeaponType.Sword:
                            foreach (Transform transform in SwordBn.transform)
                            {
                                transform.GetComponent<Image>().gameObject.SetActive(true);
                                transform.GetComponent<Image>().sprite = item.icon;
                            }

                            if (SwordBn.GetComponent<ItemController>().GetItem() != null)
                            {
                                SwordBn.GetComponent<ItemController>().GetItem().SetEquipped(false);
                            }

                            SwordBn.GetComponent<ItemController>().SetItem(item);
                            break;
                        case Item.WeaponType.Bow:
                            foreach (Transform transform in BowBn.transform)
                            {
                                transform.GetComponent<Image>().gameObject.SetActive(true);
                                transform.GetComponent<Image>().sprite = item.icon;
                            }

                            if(BowBn.GetComponent<ItemController>().GetItem() != null)
                            {
                                BowBn.GetComponent<ItemController>().GetItem().SetEquipped(false);
                            }

                            BowBn.GetComponent<ItemController>().SetItem(item);
                            break;
                    }
                    break;
            }
        }
        public void UnequipItem(Item item)
        {
            item.SetEquipped(false);

            RemoveItemStats(item);

            switch (item.equipmentType)
            {
                case Item.EquipmentType.Helmet:
                    foreach (Transform transform in HelmetBn.transform)
                    {
                        transform.GetComponent<Image>().gameObject.SetActive(false);
                        transform.GetComponent<Image>().sprite = null;
                    }
                    HelmetBn.GetComponent<ItemController>().SetItem(null);
                    description.SetActive(false);
                    break;

                case Item.EquipmentType.Chestplate:
                    foreach (Transform transform in ChestplateBn.transform)
                    {
                        transform.GetComponent<Image>().gameObject.SetActive(false);
                        transform.GetComponent<Image>().sprite = null;
                    }
                    ChestplateBn.GetComponent<ItemController>().SetItem(null);
                    description.SetActive(false);
                    break;

                case Item.EquipmentType.Leggings:
                    foreach (Transform transform in BootsBn.transform)
                    {
                        transform.GetComponent<Image>().gameObject.SetActive(false);
                        transform.GetComponent<Image>().sprite = null;
                    }
                    BootsBn.GetComponent<ItemController>().SetItem(null);
                    description.SetActive(false);
                    break;

                case Item.EquipmentType.Gloves:
                    foreach (Transform transform in GlovesBn.transform)
                    {
                        transform.GetComponent<Image>().gameObject.SetActive(false);
                        transform.GetComponent<Image>().sprite = null;
                    }
                    GlovesBn.GetComponent<ItemController>().SetItem(null);
                    description.SetActive(false);
                    break;
                case Item.EquipmentType.Weapon:
                    if (SwordBn.GetComponent<ItemController>().GetItem() == item)
                    {
                        foreach (Transform transform in SwordBn.transform)
                        {
                            transform.GetComponent<Image>().gameObject.SetActive(false);
                            transform.GetComponent<Image>().sprite = null;
                        }
                        SwordBn.GetComponent<ItemController>().SetItem(null);
                        description.SetActive(false);
                        break;
                    }
                    else
                    {
                        foreach (Transform transform in BowBn.transform)
                        {
                            transform.GetComponent<Image>().gameObject.SetActive(false);
                            transform.GetComponent<Image>().sprite = null;
                        }
                        BowBn.GetComponent<ItemController>().SetItem(null);
                        description.SetActive(false);
                        break;
                    }
            }
        }
        #endregion

        #region Equipment Stat Methods
        public void AddItemStats(Item item)
        {
            TotalArmor += item.armor;
            TotalWeight += item.weight;
            TotalMagicRes += item.magicRes;
        }
        public void RemoveItemStats(Item item)
        {
            TotalArmor -= item.armor;
            TotalWeight -= item.weight;
            TotalMagicRes -= item.magicRes;
        }
        #endregion

        #region Buttons
        public void EquippedEquipmentBn()
        {
            UseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unequip";
        }

        #region Button Filters
        public void ConsumableInvBn()
        {
            if (filter == Filter.ConsumableInv)
            {
                filter = Filter.Default;
            }
            else
            {
                filter = Filter.ConsumableInv;
            }

            ListItems();
        }
        public void WeaponInvBn()
        {
            if (filter == Filter.WeaponInv)
            {
                filter = Filter.Default;
            }
            else
            {
                filter = Filter.WeaponInv;
            }

            ListItems();
        }
        public void EquipmentInvBn()
        {
            if (filter == Filter.EquipmentInv)
            {
                filter = Filter.Default;
            }
            else
            {
                filter = Filter.EquipmentInv;
            }

            ListItems();

        }
        public void QuestInvBn()
        {
            if (filter == Filter.QuestInv)
            {
                filter = Filter.Default;
            }
            else
            {
                filter = Filter.QuestInv;
            }

            ListItems();
        }
        public void MiscInvBn()
        {
            if (filter == Filter.MiscInv)
            {
                filter = Filter.Default;
            }
            else
            {
                filter = Filter.MiscInv;
            }

            ListItems();
        }
        #endregion

        #endregion

        #region Getters
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

            if (animation)
            {
                StartCoinAnimation();
            }
        }
        public void IncreaseCoins(bool animation)
        {
            Coins++;

            if (animation)
            {
                StartCoinAnimation();
            }
        }

        public void StartCoinAnimation()
        {
            purseAnimator.SetTrigger("open");
        }
        #endregion
    }
}