using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private Item Item;

    private Animator animator;

    private bool openned = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player" && Input.GetButton("Interact"))
        {
            Debug.Log("chest");

            OpenChest();

            openned = true;
        }
    }

    private void OpenChest()
    {
        if(openned == false)
        {
            InventoryManager.Instance.Add(Item);

            animator.SetTrigger("Open");
        }
    }

    private void Start()
    {
        animator = FindObjectOfType<Animator>();
    }
}
