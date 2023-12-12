using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    private Enemy enemy;
    private Rigidbody2D arrowRB;
    Vector2 direction;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();

        arrowRB = GetComponent<Rigidbody2D>();

        float offset = Mathf.Abs(enemy.PlayerTrans.position.x - transform.position.x) / enemy.Data.distanceOffset;

        direction = new Vector2(enemy.PlayerTrans.position.x - transform.position.x, enemy.PlayerTrans.position.y - transform.position.y + offset).normalized;
        arrowRB.velocity = direction * enemy.Data.rangedSpeed;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.tag != "Enemy" && hitInfo.tag != "Item" && hitInfo.tag != "Climbable" && hitInfo.tag != "AttackRange" && hitInfo.tag != "BackgroundObject" && hitInfo.tag != "PickupRange")
        {
            PlayerStats player = hitInfo.GetComponent<PlayerStats>();
            if (player == true)
            {
                player.TakeDamage(enemy.Data.rangedDamage);
            }
            Instantiate(enemy.Data.impactEffect, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }

    private void Update()
    {
        TrackMovement();
    }

    private void TrackMovement()
    {
        direction = arrowRB.velocity;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
