using UnityEngine;
using CoreClass;
using Bardent.CoreSystem;

public class RangedAttack : MonoBehaviour
{
    [Header("Behaviour")]
    [SerializeField] private bool directedParabola;
    [SerializeField] private bool directed;
    [SerializeField] private bool randomized;
    [SerializeField] private bool flip;
    [Header("Values")]
    [Range(1, 100)]
    [SerializeField] private int accuracy;
    private Enemy enemy;
    private Rigidbody2D arrowRB;
    Vector2 direction;

    void Start()
    {
        System.Random random = new System.Random();

        enemy = GetComponentInParent<Enemy>();

        arrowRB = GetComponent<Rigidbody2D>();
        
        float offset = Mathf.Abs(enemy.OldPlayerPosition.x - transform.position.x) / enemy.Data.distanceOffset;

        if (directedParabola)
        {
            direction = new Vector2(enemy.OldPlayerPosition.x - transform.position.x, enemy.OldPlayerPosition.y - transform.position.y + offset).normalized;
        }
        else if (directed)
        {
            direction = new Vector2(enemy.OldPlayerPosition.x - transform.position.x, enemy.OldPlayerPosition.y - transform.position.y).normalized;
        }
        else
        {
            arrowRB.velocity = Vector2.right * enemy.Data.rangedSpeed;
            return;
        }

        if (randomized)
        {
            int next;
            do next = random.Next(-1, 2);
            while (next == 0);
            var randomIndex = ((float)random.NextDouble() / accuracy) * next;

            arrowRB.velocity = new Vector2(direction.x + randomIndex, direction.y + randomIndex).normalized * enemy.Data.rangedSpeed;
        }
        else arrowRB.velocity = direction * enemy.Data.rangedSpeed;

        transform.parent = null;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.tag != "Enemy" && hitInfo.tag != "Item" && hitInfo.tag != "Climbable" && hitInfo.tag != "AttackRange" && hitInfo.tag != "BackgroundObject" && hitInfo.tag != "PickupRange" && hitInfo.tag != "Event")
        {
            Player player = hitInfo.GetComponent<Player>();
            if (player)
            {
                player.Core.GetCoreComponent<DamageReceiver>().Damage(enemy.Data.rangedDamage * enemy.EnemyLevelScale, enemy.Data.damageType);
            }
            if (enemy.Data.impactEffect) Instantiate(enemy.Data.impactEffect, transform.position, Quaternion.identity);

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
        
        if (flip)
        {
            if (direction.x < 0)
            {
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else
            {
                transform.rotation = Quaternion.AngleAxis(angle + 180f, Vector3.forward);
            }
        }
        else transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
