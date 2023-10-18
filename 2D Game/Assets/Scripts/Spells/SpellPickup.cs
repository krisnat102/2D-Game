using UnityEngine;

public class SpellPickup : MonoBehaviour
{
    public Spell Spell;

    private bool isPickedUp = false;

    void Pickup()
    {
        if (!isPickedUp)
        {
            SpellManager.Instance.Add(Spell);

            Destroy(gameObject);

            isPickedUp = true;
        }
    }

    private void OnTriggerStay2D(Collider2D hitInfo)
    {
        if (hitInfo.tag == "Player")
        {
            if (Input.GetButtonDown("Interact"))
            {
                Pickup();
            }
        }
    }
}
