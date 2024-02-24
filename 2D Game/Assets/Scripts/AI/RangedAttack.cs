using UnityEngine;
using Core;
using Bardent.CoreSystem;

public class RangedAttack : MonoBehaviour
{
    [SerializeField] private bool directedParabola;
    [SerializeField] private bool directed;
    private Enemy enemy;
    private Rigidbody2D arrowRB;
    Vector2 direction;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();

        arrowRB = GetComponent<Rigidbody2D>();

        float offset = Mathf.Abs(enemy.PlayerTrans.position.x - transform.position.x) / enemy.Data.distanceOffset;

        if (directedParabola)
        {
            direction = new Vector2(enemy.PlayerTrans.position.x - transform.position.x, enemy.PlayerTrans.position.y - transform.position.y + offset).normalized;
            arrowRB.velocity = direction * enemy.Data.rangedSpeed;
        }
        else if (directed)
        {
            direction = new Vector2(enemy.PlayerTrans.position.x - transform.position.x, enemy.PlayerTrans.position.y - transform.position.y).normalized;
            arrowRB.velocity = direction * enemy.Data.rangedSpeed;
        }
        else
        {
            arrowRB.velocity = Vector2.right * enemy.Data.rangedSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.tag != "Enemy" && hitInfo.tag != "Item" && hitInfo.tag != "Climbable" && hitInfo.tag != "AttackRange" && hitInfo.tag != "BackgroundObject" && hitInfo.tag != "PickupRange")
        {
            Player player = hitInfo.GetComponent<Player>();
            if (player)
            {
                player.Core.GetCoreComponent<DamageReceiver>().Damage(enemy.data.rangedDamage * enemy.EnemyLevelScale, enemy.data.damageType);
            }
            if (enemy.Data.impactEffect) Instantiate(enemy.Data.impactEffect, transform.position, transform.rotation);

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
