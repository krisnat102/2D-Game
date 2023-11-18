using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private Item Item;

    private bool isPickedUp = false;

    void Pickup()
    {
        if (!isPickedUp)
        {
            InventoryManager.Instance.Add(Item);

            Destroy(gameObject);

            isPickedUp = true;
        }
    }

    private void OnTriggerStay2D(Collider2D hitInfo)
    {
        if (hitInfo.tag == "Player" && Input.GetButtonDown("Interact"))
        {
                Pickup();
        }
    }
}
