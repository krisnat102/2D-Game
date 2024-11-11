using Inventory;
using Spells;
using UnityEngine;

namespace Interactables
{
    public class Chest : MonoBehaviour
    {
        [SerializeField] private Item item;
        [SerializeField] private Spell spell;

        private Animator animator;

        private bool opened = false;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == "PickupRange" && PlayerInputHandler.Instance.UseInput)
            {
                OpenChest();

                opened = true;

                PlayerInputHandler.Instance.UseUseInput();

                transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        private void OpenChest()
        {
            if (opened == false && item != null)
            {
                InventoryManager.Instance.Add(item, true);

                animator.SetTrigger("open");
            }
            else if (opened == false && spell != null)
            {
                SpellManager.Instance.Add(spell);

                animator.SetTrigger("open");
            }
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
        }
    }
}