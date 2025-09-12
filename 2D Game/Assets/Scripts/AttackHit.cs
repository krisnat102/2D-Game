using UnityEngine;
using CoreClass;
using Bardent.CoreSystem;
using Krisnat;

public class AttackHit : MonoBehaviour
{
    private Enemy enemy;
    private float damage;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        Invoke(nameof(FinishAttack), 0.5f);
        damage = Random.Range(enemy.Data.minDamage, enemy.Data.maxDamage) * enemy.EnemyLevelScale;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player"))
        {
            Player player = hitInfo.transform.GetComponent<Player>(); //checks if it hit the player

            player.Core.GetCoreComponent<Stats>().health.Decrease(damage);

            Destroy(gameObject);
        }
    }

    private void FinishAttack()
    {
        Destroy(gameObject);
    }
}
