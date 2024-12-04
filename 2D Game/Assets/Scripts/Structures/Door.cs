using Inventory;
using TMPro;
using UnityEngine;

namespace Krisnat
{
    public class Door : MonoBehaviour, IStructurable
    {
        [Header("Behaviour")]
        [SerializeField] private bool locked;
        [SerializeField] private bool openOnStart;
        [SerializeField] private Item key;

        [Header("Variable References")]
        [SerializeField] private AudioSource openAudio;
        [SerializeField] private AudioSource lockedAudio;
        [SerializeField] private GameObject uiPopUp;

        private new BoxCollider2D collider;
        private Animator animator;
        private bool cooldown = false;
        private bool opened = false;

        private void Start()
        {
            animator = GetComponent<Animator>();
            collider = GetComponentInChildren<BoxCollider2D>();

            if (openOnStart)
            {
                Open(false);
            }
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            var player = collision.GetComponent<Player>();

            if (PlayerInputHandler.Instance.UseInput && !cooldown && !opened && player)
            {
                cooldown = true;
                Invoke(nameof(StopCooldown), 1.5f);

                if (locked)
                {
                    uiPopUp.SetActive(true);

                    if (key)
                    {
                        if (!InventoryManager.Instance.Items.Contains(key))
                        {
                            uiPopUp.GetComponentInChildren<TMP_Text>().text = key.itemName + " Needed";
                            lockedAudio.Play();
                            return;
                        }

                        InventoryManager.Instance.Remove(key);
                        uiPopUp.GetComponentInChildren<TMP_Text>().text = key.itemName + " Used";
                    }
                    else
                    {
                        uiPopUp.GetComponentInChildren<TMP_Text>().text = "Locked";
                        lockedAudio.Play();
                        return;
                    }
                }

                PlayerInputHandler.Instance.UseUseInput();
                Open(true);

                if (player.transform.position.x + 0.3f > transform.position.x)
                {
                    transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                    transform.position += new Vector3(-1.5f, 0);
                }
            }
        }

        public void Open(bool playAudio)
        {
            collider.enabled = false;
            animator.SetBool("open", true);
            if (playAudio) openAudio.Play();
            opened = true;
        }

        private void StopCooldown() => cooldown = false;
    }
}
