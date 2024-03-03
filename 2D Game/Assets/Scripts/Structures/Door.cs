using Inventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Krisnat
{
    public class Door : MonoBehaviour, IStructurable
    {
        [SerializeField] private bool open;
        [SerializeField] private Item key;
        [SerializeField] private AudioSource openAudio;
        [SerializeField] private AudioSource lockedAudio;
        [SerializeField] private GameObject uiPopUp;

        private BoxCollider2D collider;
        private Animator animator;
        private bool cooldown = false;

        private void Start()
        {
            animator = GetComponent<Animator>();
            collider = GetComponent<BoxCollider2D>();

            if (open)
            {
                animator.SetBool("open", true);
                collider.enabled = false;
            }
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            if (PlayerInputHandler.Instance.UseInput && !cooldown)
            {
                cooldown = true;
                Invoke(nameof(StopCooldown), 1.5f);

                if (key)
                {
                    uiPopUp.SetActive(true);

                    if (!InventoryManager.Instance.Items.Contains(key))
                    {
                        uiPopUp.GetComponentInChildren<TMP_Text>().text = key.itemName + " Needed";
                        lockedAudio.Play();
                        return;
                    }

                    InventoryManager.Instance.Remove(key);
                    uiPopUp.GetComponentInChildren<TMP_Text>().text = key.itemName + " Used";
                }

                PlayerInputHandler.Instance.UseUseInput();
                collider.enabled = false;
                animator.SetBool("open", true);
                openAudio.Play();
            }
        }
        private void StopCooldown() => cooldown = false;
    }
}
