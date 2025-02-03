using UnityEngine;
using Inventory;
using TMPro;
using UnityEngine.UI;
using Krisnat;
using Krisnat.Assets.Scripts;
using System.Linq;

namespace Spells
{
    public class SpellPickup : MonoBehaviour
    {
        [SerializeField] private Spell spell;
        [SerializeField] private bool forSale;
        [SerializeField] private int price;
        [SerializeField] private bool chest;
        [SerializeField] private float offset = 0.2f;
        [SerializeField] private string itemId;
        [SerializeField] private Animator deathAnim;
        [SerializeField] private float animTime;
        [SerializeField] private GameObject pickUpEffect;
        [SerializeField] private AudioSource pickUpAudio;

        private bool isPickedUp = false;
        private Animator animator;
        private Canvas canvas;
        private GameObject itemPrice;

        public string ItemId { get => itemId; private set => itemId = value; }

        private void Start()
        {
            if (string.IsNullOrEmpty(ItemId)) ItemId = gameObject.name + "z";

            PlayerSaveData data = SaveSystem.LoadPlayer();
            if (data != null && data.itemsTakenId != null && data.itemsTakenId.Contains(ItemId))
            {
                gameObject.SetActive(false);
            }

            animator = GetComponent<Animator>();
            canvas = GetComponentInChildren<Canvas>();

            if (forSale)
            {
                itemPrice = canvas?.GetComponent<Transform>()?.Find("ItemPrice")?.gameObject;
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

        public void Pickup()
        {
            if (!isPickedUp && spell != null)
            {
                if (pickUpAudio)
                {
                    if(!chest) pickUpAudio.gameObject.transform.parent = CoreClass.GameManager.Instance.Audios;
                    pickUpAudio.pitch = Random.Range(0.8f, 1.2f);
                    pickUpAudio.Play();
                }

                if (deathAnim)
                {
                    deathAnim.SetTrigger("take");
                    Invoke("PickupSpell", animTime);

                    return;
                }

                PickupSpell();

                if (chest)
                {
                    animator?.SetTrigger("open");
                    canvas?.gameObject.SetActive(false);
                }
            }
        }

        private void PickupSpell()
        {
            if (forSale)
            {
                InventoryManager.Instance.StartCoinAnimation();
                if (InventoryManager.Instance.Coins >= price)
                {
                    AudioManager.Instance.PlayBuySound(0.8f, 1.2f);
                    SpellManager.Instance.Add(spell);
                    InventoryManager.Instance.SetCoins(InventoryManager.Instance.Coins - price, false);
                }
                else
                {
                    return;
                }
            }
            else
            {
                SpellManager.Instance.Add(spell);
            }

            CoreClass.GameManager.Instance.ItemsTaken.Add(ItemId);

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
            itemPopUp.GetComponentsInChildren<Image>()[1].sprite = spell.icon;
            itemPopUp.GetComponentInChildren<TMP_Text>().text = spell.spellName;

            isPickedUp = true;

            if (!chest) Disable();
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
    }
}