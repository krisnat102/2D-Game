using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    [SerializeField] private List<Item> Items = new List<Item>();

    [SerializeField] private Transform ItemContent;
    [SerializeField] private GameObject InventoryItem;

    [SerializeField] private Toggle EnableRemove;

    [SerializeField] private InventoryItemController[] InventoryItems;

    [SerializeField] private GameObject Inventory;
    [SerializeField] private GameObject SpellInventory;

    [Header("Item Description")]
    [SerializeField] private Button useButton;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemName, itemDescription, itemValue, itemPrice;
    [SerializeField] private GameObject description;

    public static Button useButton1;
    public static Image itemImage1;
    public static TMP_Text itemName1, itemDescription1, itemValue1, itemPrice1;
    public static GameObject description1;
    
    private Filter filter = default;

    private enum Filter
    {
        ConsumableInv,
        MaterialInv,
        EquimpentInv,
        QuestInv,
        MisctInv
    };

    private void Awake()
    {
        Instance = this;

        useButton1 = useButton;
        itemImage1 = itemImage;
        itemName1 = itemName;
        itemDescription1 = itemDescription;
        itemValue1 = itemValue;
        itemPrice1 = itemPrice;
        description1 = description;
    }

    public void Add(Item item)
    {
        Items.Add(item);
    }

    public void Remove(Item item)
    {
        Items.Remove(item);
    }

    private void ListItems()
    {
        ItemController itemController;

        //clears the inventory before opening so that items dont duplicate
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }

        //adds the items to the inventory
        foreach (var item in Items)
        {
            /*
            if (filter == Filter.ConsumableInv)
                if (item.itemClass == Item.ItemClass.Consumable)
                {
                    GameObject obj = Instantiate(InventoryItem, ItemContent);

                    obj.SetActive(true);
                    obj.name = item.name;

                    itemController = obj.GetComponent<ItemController>();
                    itemController.SetItem(item);

                    var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
                    var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
                    var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();

                    itemName.text = item.ItemName;
                    itemIcon.sprite = item.icon;

                    if (EnableRemove.isOn)
                    {
                        removeButton.gameObject.SetActive(true);
                    }
                }
            else if (filter == Filter.MaterialInv)
                    if (item.itemClass == Item.ItemClass.Material)
                    {
                        GameObject obj = Instantiate(InventoryItem, ItemContent);

                        obj.SetActive(true);
                        obj.name = item.name;

                        itemController = obj.GetComponent<ItemController>();
                        itemController.SetItem(item);

                        var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
                        var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
                        var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();

                        itemName.text = item.ItemName;
                        itemIcon.sprite = item.icon;

                        if (EnableRemove.isOn)
                        {
                            removeButton.gameObject.SetActive(true);
                        }
                    }
            else if (filter == Filter.EquimpentInv)
                        if (item.itemClass == Item.ItemClass.Equipment)
                        {
                            GameObject obj = Instantiate(InventoryItem, ItemContent);

                            obj.SetActive(true);
                            obj.name = item.name;

                            itemController = obj.GetComponent<ItemController>();
                            itemController.SetItem(item);

                            var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
                            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
                            var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();

                            itemName.text = item.ItemName;
                            itemIcon.sprite = item.icon;

                            if (EnableRemove.isOn)
                            {
                                removeButton.gameObject.SetActive(true);
                            }
                        }
            else if (filter == Filter.QuestInv)
                            if (item.itemClass == Item.ItemClass.Quest)
                            {
                                GameObject obj = Instantiate(InventoryItem, ItemContent);

                                obj.SetActive(true);
                                obj.name = item.name;

                                itemController = obj.GetComponent<ItemController>();
                                itemController.SetItem(item);

                                var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
                                var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
                                var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();

                                itemName.text = item.ItemName;
                                itemIcon.sprite = item.icon;

                                if (EnableRemove.isOn)
                                {
                                    removeButton.gameObject.SetActive(true);
                                }
                            }
            else if (filter == Filter.MisctInv)
                                if (item.itemClass == Item.ItemClass.Misc)
                                {
                                    GameObject obj = Instantiate(InventoryItem, ItemContent);

                                    obj.SetActive(true);
                                    obj.name = item.name;

                                    itemController = obj.GetComponent<ItemController>();
                                    itemController.SetItem(item);

                                    var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
                                    var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
                                    var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();

                                    itemName.text = item.ItemName;
                                    itemIcon.sprite = item.icon;

                                    if (EnableRemove.isOn)
                                    {
                                        removeButton.gameObject.SetActive(true);
                                    }
                                }
            else if (filter == default)
                                {
                                    GameObject obj = Instantiate(InventoryItem, ItemContent);

                                    obj.SetActive(true);
                                    obj.name = item.name;

                                    itemController = obj.GetComponent<ItemController>();
                                    itemController.SetItem(item);

                                    var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
                                    var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
                                    var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();

                                    itemName.text = item.ItemName;
                                    itemIcon.sprite = item.icon;

                                    if (EnableRemove.isOn)
                                    {
                                        removeButton.gameObject.SetActive(true);
                                    }
                                }
            */
            GameObject obj = Instantiate(InventoryItem, ItemContent);

            obj.SetActive(true);
            obj.name = item.name;

            itemController = obj.GetComponent<ItemController>();
            itemController.SetItem(item);

            var itemName = obj.transform.Find("ItemName").GetComponent<Text>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();

            itemName.text = item.ItemName;
            itemIcon.sprite = item.icon;

            if (EnableRemove.isOn)
            {
                removeButton.gameObject.SetActive(true);
            }
        }

        SetInventoryItems();
    }

    private void EnableItemRemove()
    {
        if (EnableRemove.isOn)
        {
            foreach (Transform item in ItemContent)
            {
                item.transform.Find("RemoveButton").GetComponent<Button>().gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform item in ItemContent)
            {
                item.Find("RemoveButton").gameObject.SetActive(false);
            }
        }
        ListItems();
        SetInventoryItems();
    }

    private void SetInventoryItems()
    {
        InventoryItems = ItemContent.GetComponentsInChildren<InventoryItemController>();
        System.Array.Resize(ref InventoryItems, Items.Count);

        for (int i = 0; i < Items.Count; i++)
        {
            InventoryItems[i].AddItem(Items[i]);
        }
    }

    public void Update()
    {
        if (GameManager.gamePaused == false)
        {
            if (Input.GetButtonDown("Inventory"))
            {
                if (!Inventory.activeInHierarchy && !SpellInventory.activeInHierarchy)
                {
                    Inventory.SetActive(true);

                    ListItems();

                    Weapon.canFire = false;
                }
                else
                {
                    Inventory.SetActive(false);
                    SpellInventory.SetActive(false);

                    Weapon.canFire = true;
                }
            }
        }
        if (Input.GetButtonDown("Menu"))
        {
            if (Inventory.activeInHierarchy || SpellInventory.activeInHierarchy)
            {
                Inventory.SetActive(false);
                SpellInventory.SetActive(false);

                Weapon.canFire = true;
            }
        }
    }

    private void ConsumableInv()
    {
        filter = Filter.ConsumableInv;

        ListItems();
    }
    private void MaterialInv()
    {
        filter = Filter.MaterialInv;

        ListItems();
    }
    private void EquipmentInv()
    {
        filter = Filter.EquimpentInv;

        ListItems();
        
    }
    private void QuestInv()
    {
        filter = Filter.QuestInv;

        ListItems();
    }
    private void MiscInv()
    {
        filter = Filter.MisctInv;

        ListItems();
    }
}
