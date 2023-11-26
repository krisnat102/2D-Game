using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float damage = 15f;
    [SerializeField] private float speed = 20f;
    [SerializeField] private int distanceOffset = 5;

    [Header("Other")]
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private Transform player;

    private Rigidbody2D rb;
    Vector2 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        float offset = Mathf.Abs(player.position.x - transform.position.x) / distanceOffset;

        Vector2 direction = new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y + offset).normalized;
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.tag != "Enemy" && hitInfo.tag != "Item" && hitInfo.tag != "Climbable" && hitInfo.tag != "AttackRange" && hitInfo.tag != "BackgroundObject" && hitInfo.tag != "PickupRange")
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

    private void Update()
    {
        TrackMovement();
    }

    private void TrackMovement()
    {
        Vector2 direction = rb.velocity;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
