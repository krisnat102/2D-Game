using Krisnat;
using Spells;
using System.Collections;
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
        [SerializeField] private bool chest;
        [SerializeField] private float openTime;
        [SerializeField] private int price;
        [SerializeField] private float offset = 0.2f;
        [SerializeField] private bool note;
        [TextArea]
        [SerializeField] private string noteText;
        [SerializeField] private GameObject noteUIPreset;

        private bool isPickedUp = false;
        private GameObject itemPrice;
        private Animator animator;

        public static List<PopUpUI> itemPopUps = new();

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
            if (chest) animator = GetComponent<Animator>();
        }

        public  void Pickup()
        {
            if(!UIManager.Instance.NoteOpen) PlayerInputHandler.Instance.UseUseInput();
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
                    UIManager.Instance.NoteText.gameObject.SetActive(true);
                    UIManager.Instance.NoteText.text = noteText;
                }

                UIManager.Instance.NoteOpen = true;
            }
            else if(!isPickedUp && chest) {
                ForSale();

                isPickedUp = true;
                StartCoroutine(AddItem(item, true, openTime));
                animator.SetTrigger("open");
            }
            else if (!isPickedUp && item)
            {
                ForSale();

                InventoryManager.Instance.Add(item, true);
                Destroy(gameObject);
            }
        }


        public void ForSale()
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
        }

        IEnumerator AddItem(Item item, bool boolean, float time)
        {
            yield return new WaitForSeconds(time);
            InventoryManager.Instance.Add(item, boolean);

        }
    }
}