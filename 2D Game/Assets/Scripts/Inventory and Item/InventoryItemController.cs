using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Bardent.CoreSystem;
using Krisnat;
using Interactables;

namespace Inventory
{
    public class InventoryItemController : MonoBehaviour
    {
        #region Private Variables
        [SerializeField] private Button removeButton;
        [SerializeField] private Image selectedItemIndicator;
        [SerializeField] private TMP_Text itemCount;
        [SerializeField] private float equipmentCloseTime;
        [Header("Shop")]
        [SerializeField] private bool selling;
        [SerializeField] private GameObject priceText;
        [SerializeField] private float profitMargin;

        private Button useButton;
        private Image itemImage;
        private TMP_Text itemName, itemPrice, itemValue, itemWeight, itemArmor, itemMagicRes, itemDescription, weaponAttack;
        private GameObject description;
        //private bool equipmentMenuActive = false;
        private Item item;
        private Item oldDescriptionItem;
        private int cost;

        public Image SelectedItemIndicator { get => selectedItemIndicator; private set => selectedItemIndicator = value; }
        #endregion

        #region Unity Methods
        private void Update()
        {
            if (GetComponent<ItemController>().GetItem().ItemCount <= 0 && !selling)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            item = GetComponent<ItemController>().GetItem();
            cost = item.GetSaleCost();

            if (selling)
            {
                var text = priceText.GetComponentInChildren<TextMeshProUGUI>();
                text.text = cost + "";
            }
        }
        #endregion

        #region Item Methods
        public void RemoveItem()
        {
            GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            ItemController itemController = button.GetComponentInParent<ItemController>();
            item = itemController.GetItem();
            //item = itemController.GetItem();

            if (item.Equipped == true) return;

            InventoryManager.Instance.Remove(item);

            InventoryManager.Instance.ListItems();
        }
        public void RemoveItem2(Item item)
        {
            InventoryManager.Instance.Remove(item);
            Debug.Log(item.name);
            GameObject itemToDelete = GameObject.Find(item.name);
            if (itemToDelete != null) Destroy(itemToDelete);
            else Debug.Log("not found item to delete");

            InventoryManager.Instance.Description.SetActive(false);
        }

        public void AddItem(Item newItem)
        {
            item = newItem;
        }

        private void DecreaseItem(Item item)
        {
            InventoryManager.Instance.Remove(item);

            if (item.ItemCount == 0)
            {
                description = InventoryManager.Instance.Description;
                description.SetActive(false);
            }

            itemCount.text = (item.ItemCount > 1 ? "x" + item.ItemCount.ToString() : "");
            itemCount.ForceMeshUpdate();
            InventoryManager.Instance.ListItems();
        }

        public void UseItem()
        {
            GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            try
            {
                ItemController itemController = button.GetComponent<ItemController>();
                Item item = itemController.GetItem();

                if (button.GetComponentInChildren<TextMeshProUGUI>().text == "Sell")
                {
                    if (item.equipment && item.Equipped) return;
                    DecreaseItem(item);

                    InventoryManager.Instance.SetCoins(InventoryManager.Instance.Coins + item.cost, false);
                    AudioManager.Instance.PlayCoinPickupSound(0.8f, 1.2f);
                    return;
                }

                if (button.GetComponentInChildren<TextMeshProUGUI>().text == "Buy")
                {
                    cost = item.GetSaleCost();

                    if (InventoryManager.Instance.Coins < cost)
                    {
                        CameraShake.Instance.ShakeCamera(0.3f, 1.7f);
                        AudioManager.Instance.PlayTradeRefusedSound(0.5f, 0.6f);

                        return;
                    }
                    InventoryManager.Instance.Add(item, false);

                    InventoryManager.Instance.SetCoins(InventoryManager.Instance.Coins - cost, false);
                    InventoryManager.Instance.ListItems();
                    AudioManager.Instance.PlayBuySound(0.8f, 1.2f);
                    return;
                }

                if (item.consumable && item.ItemCount > 0)
                {
                    switch (item.consumableType)
                    {
                        case Item.ConsumableType.Heal:
                            if (Stats.Instance.Health.CurrentValue != Stats.Instance.Health.MaxValue)
                            {
                                DecreaseItem(item);

                                Stats.Instance.Health.Increase(item.value);

                                switch (item.consumableSoundEffect)
                                {
                                    case Item.ConsumableSoundEffect.Food:
                                        AudioManager.Instance.PlayEatSound(0.8f, 1.2f);
                                        break;
                                    case Item.ConsumableSoundEffect.Potion:
                                        AudioManager.Instance.PlayPotionDrinkSound(0.8f, 1.2f);
                                        break;
                                }
                            }
                            break;
                        case Item.ConsumableType.ManaHeal:
                            if (Stats.Instance.Mana.CurrentValue != Stats.Instance.Mana.MaxValue)
                            {
                                DecreaseItem(item);

                                Stats.Instance.Mana.Increase(item.value);

                                switch (item.consumableSoundEffect)
                                {
                                    case Item.ConsumableSoundEffect.Food:
                                        AudioManager.Instance.PlayEatSound(0.8f, 1.2f);
                                        break;
                                    case Item.ConsumableSoundEffect.Potion:
                                        AudioManager.Instance.PlayPotionDrinkSound(0.8f, 1.2f);
                                        break;
                                }
                            }
                            break;
                    }
                }
                else if (item.equipment && item.ItemCount > 0)
                {
                    AudioManager.Instance.PlayEquipmentSound(0.8f, 1.2f);
                    switch (button.GetComponentInChildren<TextMeshProUGUI>().text)
                    {
                        case "Equip":
                            InventoryManager.Instance.EquipItem(item);

                            Description();
                            break;

                        case "Unequip":
                            //description = InventoryManager.Instance.Description;
                            InventoryManager.Instance.UnequipItem(item);

                            Description();
                            break;
                    }
                }
            }
            catch
            {
                Debug.LogWarning("Equip failed");
            }
        }

        public void DisableSelectedIndicators() => InventoryManager.Instance.DisableSelectedIndicators();
        #endregion

        #region UI Methods
        public void Description()
        {
            GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            ItemController itemController = button?.GetComponent<ItemController>();
            try
            {
                item = itemController.GetItem();
                cost = item.GetSaleCost();

                description = InventoryManager.Instance.Description;
                description.SetActive(true);

                itemImage = InventoryManager.Instance.ItemImage;
                itemName = InventoryManager.Instance.ItemName;
                itemDescription = InventoryManager.Instance.ItemDescription;
                itemValue = InventoryManager.Instance.ItemValue;
                itemPrice = InventoryManager.Instance.ItemPrice;
                itemWeight = InventoryManager.Instance.ItemWeight;
                itemArmor = InventoryManager.Instance.ItemArmor;
                itemMagicRes = InventoryManager.Instance.ItemMagicRes;
                weaponAttack = InventoryManager.Instance.WeaponAttack;

                itemImage.sprite = item.icon;
                itemName.text = item.itemName.ToUpper();
                itemDescription.text = item.itemDescription;
                useButton = InventoryManager.Instance.UseButton;
                ItemController useButtonItemController = useButton.GetComponent<ItemController>();
                useButtonItemController.SetItem(item);

                itemPrice.text = "PRICE - " + cost.ToString();

                if (item.value != 0 && item.equipmentType != Item.EquipmentType.Weapon)
                {
                    itemValue.gameObject.SetActive(true);
                    itemValue.text = "VALUE - " + item.value.ToString();
                }
                else itemValue.gameObject.SetActive(false);

                if (item.usable)
                    useButton.gameObject.SetActive(true);
                else useButton.gameObject.SetActive(false);

                if (item.consumable)
                {
                    useButton.GetComponentInChildren<TextMeshProUGUI>().text = "Use";
                    itemValue.gameObject.SetActive(true);

                    switch (item.consumableType)
                    {
                        case Item.ConsumableType.Heal:
                            itemValue.text = "HEAL - " + item.value.ToString();
                            break;
                        case Item.ConsumableType.ManaHeal:
                            itemValue.text = "HEAL - " + item.value.ToString();
                            break;
                    }
                }

                if (item.equipment)
                {
                    itemWeight.gameObject.SetActive(true);

                    itemWeight.text = "WEIGHT - " + item.weight.ToString();

                    if (item.equipmentType != Item.EquipmentType.Weapon)
                    {
                        weaponAttack.gameObject.SetActive(false);

                        itemArmor.gameObject.SetActive(true);
                        itemMagicRes.gameObject.SetActive(true);

                        itemArmor.text = "ARMOR - " + item.armor.ToString();
                        itemMagicRes.text = "MAGIC RES - " + item.magicRes.ToString();
                    }
                    else
                    {
                        itemArmor.gameObject.SetActive(false);
                        itemMagicRes.gameObject.SetActive(false);

                        weaponAttack.gameObject.SetActive(true);

                        weaponAttack.text = "ATTACK - " + item.value.ToString();
                    }

                    if (item.Equipped)
                    {
                        useButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unequip";
                    }
                    else
                    {
                        useButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
                    }

                    /*descriptionExtension.SetActive(true);
                    if (equipmentMenuActive == false)
                    {
                        descriptionAnimator.SetTrigger("Open");
                        descriptionAnimator.SetBool("OpenClose", true);

                        equipmentMenuActive = true;
                    }*/
                }
                else
                {
                    itemWeight.gameObject.SetActive(false);
                    itemArmor.gameObject.SetActive(false);
                    itemMagicRes.gameObject.SetActive(false);
                    weaponAttack.gameObject.SetActive(false);

                    /*descriptionAnimator.SetTrigger("Close");
                    descriptionAnimator.SetBool("OpenClose", false);

                    Invoke("CloseEquipmentMenu", equipmentCloseTime);
                    equipmentMenuActive = false;*/
                }
                if (InventoryManager.Instance.Shop && ! selling)
                {
                    useButton.gameObject.SetActive(true);
                    useButton.GetComponentInChildren<TextMeshProUGUI>().text = "Sell";
                }

                if (selling)
                {
                    useButton.gameObject.SetActive(true);
                    useButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
                }

                if (!oldDescriptionItem) oldDescriptionItem = item;
                else if (oldDescriptionItem != item)
                {
                    DisableSelectedIndicators();
                }
                oldDescriptionItem = item;
            }
            catch
            {
                Debug.LogWarning("Item not found");
                Description();
            }
            
        }

        /*private void CloseEquipmentMenu()
        {
            GameObject descriptionExtension = InventoryManager.Instance.GetEquipmentMenu();
            descriptionExtension.SetActive(false);
        }*/
                #endregion
            }
        }