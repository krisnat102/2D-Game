using Krisnat;
using Krisnat.Assets.Scripts;
using Spells;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Inventory
{
    public class ItemPickup : MonoBehaviour
    {
        #region Variables
        #region Private Variables
        [FormerlySerializedAs("portal")] [SerializeField] private bool arrowOnly;
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
        [SerializeField] private string itemId;
        [SerializeField] private Animator deathAnim;
        [SerializeField] private float animTime;
        [SerializeField] private GameObject pickUpEffect;
        [SerializeField] private AudioSource pickUpAudio;
        [SerializeField] private float lowerPitchRange = 0.75f;
        [SerializeField] private float higherPitchRange = 1.25f;

        private bool isPickedUp = false;
        private GameObject itemPrice;
        private Animator animator;
        private Canvas canvas;
        #endregion

        #region Static Variables
        public static List<PopUpUI> itemPopUps = new();
        public static List<string> itemsTaken = new();
        #endregion

        #region Properties
        public bool Note { get => note; private set => note = value; }
        public string ItemId { get => itemId; private set => itemId = value; }
        #endregion
        #endregion

        #region Unity Methods
        private void Start()
        {
            if (arrowOnly || note) return;

            if (string.IsNullOrEmpty(ItemId)) ItemId = gameObject.name;

            PlayerSaveData data = SaveSystem.LoadPlayer();
            if (data != null && data.itemsTakenId != null && data.itemsTakenId.Contains(ItemId))
            {
                gameObject.SetActive(false);
                return;
            }

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
            if (chest)
            {
                canvas = GetComponentInChildren<Canvas>();
            }
        }
        #endregion

        public void Pickup()
        {
            if (arrowOnly) return;
            if (!UIManager.Instance.NoteOpen) PlayerInputHandler.Instance.UseUseInput();

            if (pickUpAudio && !UIManager.Instance.NoteOpen)
            {
                if (!chest && !Note) pickUpAudio.gameObject.transform.parent = CoreClass.GameManager.Instance.Audios;
                pickUpAudio.pitch = Random.Range(lowerPitchRange, higherPitchRange);
                pickUpAudio.Play();
            }

            if (Note)
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
                return;
            }
            else if (item)
            {
                if (deathAnim)
                {
                    deathAnim.SetTrigger("take");
                    Invoke("PickUpItem", animTime);

                    return;
                }

                PickUpItem();

                if (chest)
                {
                    animator?.SetTrigger("open");
                    canvas?.gameObject.SetActive(false);
                }
            }
        }

        private void PickUpItem()
        {
            if (!isPickedUp && chest)
            {
                if (ForSale())
                {
                    canvas.gameObject.SetActive(false);
                    isPickedUp = true;
                    StartCoroutine(AddItem(item, true, openTime));
                    animator.SetTrigger("open");
                    CoreClass.GameManager.Instance.ItemsTaken.Add(ItemId);
                }
            }
            else if (!isPickedUp && item)
            {
                if (ForSale())
                {
                    InventoryManager.Instance.Add(item, true);
                    Disable();
                    CoreClass.GameManager.Instance.ItemsTaken.Add(ItemId);
                }
            }
        }

        private void Disable()
        {
            if (pickUpEffect)
            {
                pickUpEffect.SetActive(true);
                pickUpEffect.transform.parent = CoreClass.GameManager.Instance.Particles;
            }

            gameObject.SetActive(false);
        }

        public bool ForSale()
        {
            if (forSale)
            {
                InventoryManager.Instance.StartCoinAnimation();
                if (InventoryManager.Instance.Coins >= price)
                {
                    AudioManager.instance.PlayBuySound(0.8f, 1.2f);
                    InventoryManager.Instance.SetCoins(InventoryManager.Instance.Coins - price, false);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        IEnumerator AddItem(Item item, bool boolean, float time)
        {
            yield return new WaitForSeconds(time);
            InventoryManager.Instance.Add(item, boolean);
        }
    }
}