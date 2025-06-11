using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Krisnat;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Spells;
using UnityEngine.UI;
using System.Collections;

namespace Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance;

        #region Private Variables

        [Header("Inventory References")]
        [SerializeField] private Transform itemContent;
        [SerializeField] private GameObject inventoryItem;
        [SerializeField] private Toggle enableRemove;
        [SerializeField] private GameObject inventory, spellInventory, characterTab;

        [Header("Animations")]
        [SerializeField] private float inventoryOpeningSpeed = 2f;
        [SerializeField] private float inventoryClosingSpeed = 2f;

        [SerializeField] private float characterTabOpeningSpeed = 2f;
        [SerializeField] private float characterTabClosingSpeed = 2f;

        [Header("Equipment MiniMenu")]
        [SerializeField] private Animator equipmentMenuAnimator;
        [SerializeField] private GameObject equipmentMenu, equipmentButtons;
        [SerializeField] private Button helmetBn, chestplateBn, glovesBn, bootsBn, swordBn, bowBn;

        [Header("Item Description")]
        [SerializeField] private Button useButton;
        [SerializeField] private Image itemImage;
        [SerializeField] private TMP_Text itemName, itemDescription, itemValue, itemPrice, itemWeight, itemArmor, itemMagicRes, weaponAttack;
        [SerializeField] private GameObject description;

        [Header("Shop")]
        [SerializeField] private GameObject shopInterface;
        [SerializeField] private GameObject inventoryShopItem;
        [SerializeField] private List<Transform> shopItemContents = new();
        
        [Header("Coins")]
        [SerializeField] private GameObject purse;
        [SerializeField] private TMP_Text coinCounter, levelUpCoinCounter, inventoryCoinCounter; 
        [SerializeField] private float purseAnimationDistance, purseAnimationDuration, purseAnimationTimeOnScreen;

        [Header("UI")]
        [SerializeField] private float uiPopUpCooldown = 0.4f;

        private Filter filter = default;
        private float totalArmor, totalMagicRes, totalWeight;
        private List<Item> items = new List<Item>();
        private List<Item> distinctItems, duplicates, currentItems, allItems, equippedItems = new List<Item>();
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
        public bool LevelUpUIActiveInHierarchy { get; private set; }
        public bool Shop { get; set; }
        public float TotalArmor { get => totalArmor; private set => totalArmor = value; }
        public float TotalMagicRes { get => totalMagicRes; private set => totalMagicRes = value; }
        public float TotalWeight { get => totalWeight; private set => totalWeight = value; }
        public float UIPopUpCooldownTracker { get; set; }
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
        public List<Item> AllItems { get => allItems; private set => allItems = value; }
        public List<Item> DistinctItems { get => distinctItems; private set => distinctItems = value; }
        public List<Item> Duplicates { get => duplicates; private set => duplicates = value; }
        public List<Item> Items { get => items; private set => items = value; }
        public List<Item> EquippedItems { get => equippedItems; private set => equippedItems = value; }
        public GameObject Inventory { get => inventory; private set => inventory = value; }
        public GameObject SpellInventory { get => spellInventory; private set => spellInventory = value; }
        public GameObject CharacterTab { get => characterTab; private set => characterTab = value; }
        public float UIPopUpCooldown { get => uiPopUpCooldown; private set => uiPopUpCooldown = value; }
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
            AllItems = CoreClass.GameManager.Instance.GetCustomAssets<Item>("Item", "Items");

            inventoryScale = inventory.transform.localScale.x;
            characterTabScale = characterTab.transform.localScale.x;

            purseAnimator = purse.GetComponent<Animator>();

            StartCoinAnimation();
        }

        public void Update()
        {
            InventoryActiveInHierarchy = Inventory.activeInHierarchy;
            SpellInventoryActiveInHierarchy = SpellInventory.activeInHierarchy;
            CharacterTabActiveInHierarchy = CharacterTab.activeInHierarchy;
            LevelUpUIActiveInHierarchy = UIManager.Instance.LevelUpInterface.activeInHierarchy;

            coinCounter.text = Coins.ToString();
            inventoryCoinCounter.text = Coins.ToString();
            levelUpCoinCounter.text = Coins.ToString();

            if (PlayerInputHandler.Instance.InventoryInput)
            {
                PlayerInputHandler.Instance.UseInventoryInput();
                if (!InventoryActiveInHierarchy && !SpellInventoryActiveInHierarchy && !CharacterTabActiveInHierarchy && !LevelUpUIActiveInHierarchy)
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
                if (!InventoryActiveInHierarchy && !SpellInventoryActiveInHierarchy && !CharacterTabActiveInHierarchy && !LevelUpUIActiveInHierarchy)
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

                if (Shop)
                {
                    shopInterface.SetActive(true);
                    equipmentButtons.SetActive(false);
                } 

                //TODO: Optimize this awful code out of the game
                //(done 2 times because of a bug with the item count upon first opening on inv)
                ListItems();
                ListItems();
            }
            else
            {
                if (shopItemContents.Count > 0 && shopItemContents[0])
                {
                    foreach (Transform shopItemContent in shopItemContents)
                    {
                        shopItemContent?.gameObject.SetActive(false);
                    }
                }

                GameObject[] uiToClose = new GameObject[2];
                uiToClose[0] = SpellInventory; uiToClose[1] = CharacterTab;
                UIManager.Instance.OpenCloseUI(Inventory, inventoryScale, inventoryClosingSpeed, true, false, false, uiToClose);

                if (shopInterface) shopInterface.SetActive(false);
                if (equipmentButtons) equipmentButtons.SetActive(true);
            }
        }
        
        public void OpenCloseCharacterTab(bool openOrClose)
        {
            if (openOrClose)
            {
                UIManager.Instance.OpenCloseUI(CharacterTab, characterTabScale, characterTabOpeningSpeed, true, false, true);
            }
            else
            {
                GameObject[] uiToClose = new GameObject[2];
                uiToClose[0] = SpellInventory; uiToClose[1] = Inventory;
                UIManager.Instance.OpenCloseUI(CharacterTab, characterTabScale, characterTabClosingSpeed, true, false, false, uiToClose);
                description.SetActive(false);
            }
        }
        #endregion

        #region Item Management Methods
        #region General 
        public void Add(Item item, bool uiSpawner)
        {
            Items.Add(item);

            if (uiSpawner)
            {
                if (UIPopUpCooldownTracker == 0) ItemTakenPopUp(item);
                else StartCoroutine(ItemPopupRoutine(item, UIPopUpCooldownTracker));
            }
        }

        public void Add(List<Item> itemsToAdd)
        {
            Items.AddRange(itemsToAdd);
        }

        public void Remove(Item item)
        {
            Items.Remove(item);
            item.DecreaseItemCount();
        }

        public void DeleteItem(Item item)
        {
            Items.Remove(item);
            item.SetItemCount(0);
        }

        public void ClearInventory()
        {
            if (!itemContent) return;
            
            Items.Clear();
            foreach (Transform trans in itemContent)
            {
                Destroy(trans.gameObject);
            }
            ListItems();
        }

        public void ListItems()
        {
            DistinctItems = Items.GroupBy(x => x.name).Select(y => y.First()).ToList();
            Duplicates = Items.GroupBy(p => new { p.name })
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1))
                .ToList();
            //clears the inventory before opening so that items don't duplicate
            if (currentItems != null)
            {
                currentItems.Clear();
            }
            currentItems = new();

            foreach (Transform trans in itemContent)
            {
                Item item = trans.GetComponent<ItemController>().GetItem();
                if (item.ItemCount != 0)
                {
                    item.SetItemCount(1);
                    currentItems.Add(item);
                }
                else
                {
                    Destroy(trans.gameObject);
                    if(items.Contains(item)) items.Remove(item);
                }
            }

            if (currentItems != null)
            {
                foreach (Item item in DistinctItems.Except(currentItems))
                {
                    GameObject obj = CreateItem(item);
                    item.SetItemCount(1);
                }

                foreach (Item item in currentItems.Except(DistinctItems))
                {
                    GameObject obj = CreateItem(item);
                    item.SetItemCount(1);
                }
            }
            else
            {
                foreach (Item item in DistinctItems)
                {
                    GameObject obj = CreateItem(item);
                    item.SetItemCount(1);
                }
            }

            //adds the items to the inventory
            foreach (Item item in DistinctItems)
            {
                foreach (Item duplicateItem in Duplicates)
                {
                    if (item.name == duplicateItem.name)
                        item.IncreaseItemCount();
                }
            }

            foreach (Transform trans in itemContent)
            {
                Item item = trans.GetComponent<ItemController>().GetItem();
                var itemCount = trans.GetComponentInChildren<SearchAssist>().GetComponent<TextMeshProUGUI>();
                itemCount.text = (item.ItemCount > 1 ? "x" + item.ItemCount.ToString() : "");

                var removeButton = CoreClass.GameManager.Instance.GetComponentOnlyInChildren<Button>(trans);
                
                if (enableRemove.isOn && item.itemClass != Item.ItemClass.Quest && !item.Equipped)
                {
                    removeButton.gameObject.SetActive(true);
                }
                else
                {
                    removeButton.gameObject.SetActive(false);
                }
                
                if (Shop)
                {
                    if (item.itemClass == Item.ItemClass.Quest || item.Equipped)
                    {
                        trans.gameObject.SetActive(false);
                    }
                }
                else if (filter == Filter.Default)
                {
                    trans.gameObject.SetActive(true);  
                }
                else if (filter == Filter.QuestInv)
                {
                    if (item.itemClass == Item.ItemClass.Quest) trans.gameObject.SetActive(true);
                    else trans.gameObject.SetActive(false);
                }
                else if (filter == Filter.EquipmentInv)
                {
                    if (item.itemClass == Item.ItemClass.Equipment) trans.gameObject.SetActive(true);
                    else trans.gameObject.SetActive(false);
                }
                else if (filter == Filter.MiscInv)
                {
                    if (item.itemClass == Item.ItemClass.Misc) trans.gameObject.SetActive(true);
                    else trans.gameObject.SetActive(false);
                }
                else if (filter == Filter.WeaponInv)
                {
                    if (item.itemClass == Item.ItemClass.Weapon) trans.gameObject.SetActive(true);
                    else trans.gameObject.SetActive(false);
                }
                else if (filter == Filter.ConsumableInv)
                {
                    if (item.itemClass == Item.ItemClass.Consumable) trans.gameObject.SetActive(true);
                    else trans.gameObject.SetActive(false);
                }
            }

            for (int i = equippedItems.Count - 1; i >= 0; i--) 
            {
                if (!DistinctItems.Contains(equippedItems[i]))
                {
                    UnequipItem(equippedItems[i]);
                }
            }
        }

        private GameObject CreateItem(Item item)
        {
            GameObject obj = Instantiate(inventoryItem, itemContent);

            obj.SetActive(true);

            obj.name = item.name;

            ItemController itemController = obj.GetComponent<ItemController>();
            itemController.SetItem(item);

            var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();

            itemName.text = item.itemName;
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
                foreach (Transform trans in itemContent)
                {
                    Item item = trans.GetComponent<ItemController>().GetItem();
                    if (item.itemClass == Item.ItemClass.Quest || item.Equipped)
                    {
                        trans.transform.Find("RemoveButton").GetComponent<Button>().gameObject.SetActive(false);
                    }
                    else trans.transform.Find("RemoveButton").GetComponent<Button>().gameObject.SetActive(true);
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
        #endregion

        public GameObject CreateShopItem(Item item, Transform transContent)
        {
            GameObject obj = Instantiate(inventoryShopItem, transContent);

            obj.SetActive(true);

            obj.name = item.name;

            ItemController itemController = obj.GetComponent<ItemController>();
            itemController.SetItem(item);

            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();

            itemName.text = item.itemName;
            itemIcon.sprite = item.icon;

            return obj;
        }

        public void DisableSelectedIndicators()
        {
            foreach(Transform item in itemContent)
            {
                item.GetComponent<InventoryItemController>().SelectedItemIndicator.gameObject.SetActive(false);
            }
            if (Shop)
            {
                foreach(Transform shopItemContent in shopItemContents)
                {
                    foreach (Transform item in shopItemContent)
                    {
                        item.GetComponent<InventoryItemController>().SelectedItemIndicator.gameObject.SetActive(false);
                    }
                }
            }
        }
        #endregion

        #region Equipment Methods
        public int[] EquippedItemsIds()
        {
            if (!HelmetBn || !ChestplateBn || !GlovesBn || !BootsBn || SwordBn || BowBn) return Array.Empty<int>();
            
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

            equippedItems.Add(item);

            item.SetEquipped(true);

            AddItemStats(item);

            switch (item.equipmentType)
            {
                case Item.EquipmentType.Helmet:
                    var bnHelmet = HelmetBn.GetComponent<ItemController>();

                    if (bnHelmet.GetItem() != null)
                    {
                        UnequipItem(bnHelmet.GetItem());
                    }

                    foreach (Transform transform in HelmetBn.transform)
                    {
                        transform.GetComponent<Image>().gameObject.SetActive(true);
                        transform.GetComponent<Image>().sprite = item.icon;
                    }

                    bnHelmet.SetItem(item);
                    break;

                case Item.EquipmentType.Chestplate:
                    var bnChestplate = ChestplateBn.GetComponent<ItemController>();

                    if (bnChestplate.GetItem() != null)
                    {
                        UnequipItem(bnChestplate.GetItem());
                    }

                    foreach (Transform transform in ChestplateBn.transform)
                    {
                        transform.GetComponent<Image>().gameObject.SetActive(true);
                        transform.GetComponent<Image>().sprite = item.icon;
                    }

                    bnChestplate.SetItem(item);
                    break;

                case Item.EquipmentType.Leggings:
                    var bnLeggings = BootsBn.GetComponent<ItemController>();

                    if (bnLeggings.GetItem() != null)
                    {
                        UnequipItem(bnLeggings.GetItem());
                    }

                    foreach (Transform transform in BootsBn.transform)
                    {
                        transform.GetComponent<Image>().gameObject.SetActive(true);
                        transform.GetComponent<Image>().sprite = item.icon;
                    }

                    bnLeggings.SetItem(item);
                    break;

                case Item.EquipmentType.Gloves:
                    var bnGloves = GlovesBn.GetComponent<ItemController>();

                    if (bnGloves.GetItem() != null)
                    {
                        UnequipItem(bnGloves.GetItem());
                    }

                    foreach (Transform transform in GlovesBn.transform)
                    {
                        transform.GetComponent<Image>().gameObject.SetActive(true);
                        transform.GetComponent<Image>().sprite = item.icon;
                    }

                    bnGloves.SetItem(item);
                    break;
                case Item.EquipmentType.Weapon:
                    switch (item.GetWeaponType())
                    {
                        case Item.WeaponType.Sword:
                            var bnSword = SwordBn.GetComponent<ItemController>();

                            if (bnSword.GetItem() != null)
                            {
                                UnequipItem(bnSword.GetItem());
                            }

                            foreach (Transform transform in SwordBn.transform)
                            {
                                transform.GetComponent<Image>().gameObject.SetActive(true);
                                transform.GetComponent<Image>().sprite = item.icon;
                            }

                            bnSword.SetItem(item);
                            break;
                        case Item.WeaponType.Bow:
                            var bnBow = BowBn.GetComponent<ItemController>();

                            if (bnBow.GetItem() != null)
                            {
                                UnequipItem(bnBow.GetItem());
                            }

                            foreach (Transform transform in BowBn.transform)
                            {
                                transform.GetComponent<Image>().gameObject.SetActive(true);
                                transform.GetComponent<Image>().sprite = item.icon;
                            }

                            bnBow.SetItem(item);
                            break;
                    }
                    break;
            }
        }
        public void UnequipItem(Item item)
        {
            item.SetEquipped(false);

            if(equippedItems.Contains(item)) equippedItems.Remove(item);

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

        #region UI
        public void ItemTakenPopUp(Item item)
        {
            UIPopUpCooldownTracker = UIPopUpCooldown;

            StartCoroutine(ReduceCooldownOverTime());

            var canvasTransform = UIManager.Instance.Canvas.transform;
            var itemPopUp = Instantiate(UIManager.Instance.ItemPickupPopUp, canvasTransform).GetComponent<PopUpUI>();

            foreach (var ui in ItemPickup.itemPopUps)
            {
                ui.GoUp();
            }

            ItemPickup.itemPopUps.Add(itemPopUp);

            float yOffset = Mathf.Lerp(-350, -550, (Screen.currentResolution.height - 720f) / (2160f - 720f));
            yOffset = Mathf.Clamp(yOffset, -550, -350);

            itemPopUp.transform.position = canvasTransform.position + new Vector3(0, yOffset, 0);

            var images = itemPopUp.GetComponentsInChildren<Image>();
            if (images.Length > 1) images[1].sprite = item.icon;

            var textComponent = itemPopUp.GetComponentInChildren<TMP_Text>();
            if (textComponent != null) textComponent.text = item.itemName;
        }

        private IEnumerator ItemPopupRoutine(Item item, float delay)
        {
            yield return new WaitForSeconds(delay);
            ItemTakenPopUp(item);
        }

        public IEnumerator ReduceCooldownOverTime()
        {
            while (UIPopUpCooldownTracker > 0)
            {
                yield return null;
                UIPopUpCooldownTracker -= Time.deltaTime;
            }

            UIPopUpCooldownTracker = 0;
        }

        #endregion
    }
}