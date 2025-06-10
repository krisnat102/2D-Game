using UnityEngine;
using Inventory;
using TMPro;
using UnityEngine.UI;
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
                isPickedUp = true;

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
                    AudioManager.instance.PlayBuySound(0.8f, 1.2f);
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

            if (InventoryManager.Instance.UIPopUpCooldownTracker == 0) SpellManager.Instance.SpellTakenPopUp(spell);
            else StartCoroutine(SpellManager.Instance.SpellPopupRoutine(spell, InventoryManager.Instance.UIPopUpCooldownTracker));

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