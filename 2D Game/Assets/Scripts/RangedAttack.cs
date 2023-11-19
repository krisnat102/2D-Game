using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    [SerializeField] private float damage = 15f;
    [SerializeField] private float speed = 20f;

    [SerializeField] private GameObject impactEffect;

    [SerializeField] private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.tag != "Enemy" && hitInfo.tag != "Item" && hitInfo.tag != "Climbable" && hitInfo.tag != "AttackRange" && hitInfo.tag != "BackgroundObject")
        {
            PlayerStats player = hitInfo.GetComponent<PlayerStats>();
            if (player == true)
            {
                player.TakeDamage(damage);
            }
            Instantiate(impactEffect, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}
