using UnityEngine;
using CoreClass;
using Bardent.CoreSystem;

public class AttackHit : MonoBehaviour
{
    private Enemy enemy;
    private float damage;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        Invoke("FinishAttack", 0.5f);
        damage = enemy.Data.damage * enemy.EnemyLevelScale;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.tag == "Player")
        {
            Player player = hitInfo.transform.GetComponent<Player>(); //checks if it hit the player

            player.Core.GetCoreComponent<Stats>().Health.Decrease(damage);

            Destroy(gameObject);
        }
    }

    private void FinishAttack()
    {
        Destroy(gameObject);
    }
}
