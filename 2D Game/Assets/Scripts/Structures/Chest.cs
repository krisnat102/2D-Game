using Inventory;
using Spells;
using UnityEngine;
using CoreClass;

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
            }
        }

        private void OpenChest()
        {
            if (opened == false && item != null)
            {
                InventoryManager.Instance.Add(item, true);

                animator.SetTrigger("Open");
            }
            else if (opened == false && spell != null)
            {
                SpellManager.Instance.Add(spell);

                animator.SetTrigger("Open");
            }
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
        }
    }
}