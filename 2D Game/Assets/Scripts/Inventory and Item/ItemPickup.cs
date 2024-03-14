using Krisnat;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private Item item;
        [SerializeField] private bool forSale;
        [SerializeField] private int price;
        [SerializeField] private bool chest;
        [SerializeField] private float offset = 0.2f;
        [SerializeField] private bool note;
        [TextArea]
        [SerializeField] private string noteText;
        [SerializeField] private GameObject noteUIPreset;

        private bool isPickedUp = false;
        private GameObject itemPrice;
        public static List<PopUpUI> itemPopUps = new();


        public void Pickup()
        {
            if(!UIManager.Instance.NoteOpen) PlayerInputHandler.Instance.UseUseInput(); // TODO: Fix the bug listen in Monday under fixBugs
            if (note)
            {
                if (noteUIPreset)
                {
                    UIManager.Instance.NoteUI.SetActive(true);
                    noteUIPreset.SetActive(true);
                }
                else
                {
                    UIManager.Instance.NoteUI.SetActive(true);
                    UIManager.Instance.NoteText.text = noteText;
                }

                UIManager.Instance.NoteOpen = true;
            }
            else if (!isPickedUp && item != null)
            {
                if (forSale)
                {
                    InventoryManager.Instance.StartCoinAnimation();
                    if (InventoryManager.Instance.Coins >= price)
                    {
                        AudioManager.Instance.BuySound.Play();
                        InventoryManager.Instance.SetCoins(InventoryManager.Instance.Coins - price, false);
                    }
                    else
                    {
                        return;
                    }
                }
                InventoryManager.Instance.Add(item, true);
                Destroy(gameObject);
            }
        }

        private void Awake()
        {
            if (forSale)
            {
                itemPrice = GetComponentInChildren<Canvas>()?.GetComponent<Transform>()?.Find("ItemPrice")?.gameObject;
                itemPrice.SetActive(true);
                itemPrice.GetComponentInChildren<TMP_Text>().text = price.ToString();
                if (price < 10)
                {
                    itemPrice.GetComponentInChildren<Image>().gameObject.transform.position -= new Vector3(offset, 0);
                }
                else if (price > 99)
                {
                    itemPrice.GetComponentInChildren<Image>().gameObject.transform.position += new Vector3(offset, 0);
                }
            }
        }
    }
}