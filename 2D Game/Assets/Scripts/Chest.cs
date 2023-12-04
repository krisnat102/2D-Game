using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private Spell spell;

    private Animator animator;

    private bool openned = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "PickupRange" && Input.GetButton("Interact"))
        {
            OpenChest();

            openned = true;
        }
    }

    private void OpenChest()
    {
        if(openned == false && item != null)
        {
            InventoryManager.Instance.Add(item);

            animator.SetTrigger("Open");
        }
        else if (openned == false && spell != null)
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
