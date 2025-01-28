using UnityEngine;
using CoreClass;
using Bardent.CoreSystem;
using Krisnat;

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
    private DamageReceiver dmgReceiver;
    private Rigidbody2D arrowRB;
    private Transform impactEffectLocation;
    private Vector2 direction;

    void Start()
    {
        dmgReceiver = PlayerInputHandler.Instance.GetComponentInChildren<DamageReceiver>();
        enemy = GetComponentInParent<Enemy>();
        arrowRB = GetComponent<Rigidbody2D>();
        impactEffectLocation = GetComponentInChildren<SearchAssist>(true).transform;

        System.Random random = new System.Random();

        Vector2 firePoint = enemy.FirePoint.position;

        if (directedParabola)
        {
            direction = CalculateParabola() * enemy.Data.rangedSpeed;
        }
        else if (directed)
        {
            direction = new Vector2(enemy.OldPlayerPosition.x - firePoint.x, enemy.OldPlayerPosition.y - firePoint.y).normalized;
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
        if (hitInfo.tag != "Enemy" && hitInfo.tag != "Item" && hitInfo.tag != "AttackRange" && hitInfo.tag != "BackgroundObject" && hitInfo.tag != "PickupRange" && hitInfo.tag != "Event" && hitInfo.tag != "Arrow")
        {
            Player player = hitInfo.GetComponent<Player>();
            if (player)
            {
                dmgReceiver.Damage(enemy.Data.rangedDamage * enemy.EnemyLevelScale, enemy.Data.damageType);
            }

            if (enemy.Data.impactEffect && !dmgReceiver.Invincible)
            {
                Instantiate(enemy.Data.impactEffect, impactEffectLocation.position, Quaternion.identity).transform.parent = null;
            }

            if (!dmgReceiver.Invincible) gameObject.SetActive(false);
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

    private Vector2 CalculateParabola()
    {
        Vector2 firePoint = enemy.FirePoint.position;

        float g = Mathf.Abs(Physics2D.gravity.y); // Gravity magnitude
        float distance = enemy.OldPlayerPosition.x - firePoint.x; // Horizontal distance
        float heightDifference = enemy.OldPlayerPosition.y - firePoint.y; // Vertical distance

        // Adjust initial speed to control the trajectory
        float initialSpeed = 10f;

        // Calculate the time to reach the target horizontally
        float time = Mathf.Abs(distance) / initialSpeed;

        // Recalculate vertical velocity using the correct height difference
        float Vy = (heightDifference / time) + (0.5f * g * time);
        Vy *= enemy.Data.distanceOffset;

        // Create the velocity vector
        Vector2 velocity = new Vector2(initialSpeed * Mathf.Sign(distance), Vy);

        // Normalize or use directly
        return velocity.normalized;
    }

}
