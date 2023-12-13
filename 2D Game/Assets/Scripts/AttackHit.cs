using UnityEngine;
using Core;

public class AttackHit : MonoBehaviour
{
    private Enemy enemy;
    private float damage;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        Invoke("FinishAttack", 0.5f);
        damage = enemy.Data.damage * enemy.LevelIndex;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.tag == "Player")
        {
            PlayerStats player = hitInfo.GetComponent<PlayerStats>();

            player.TakeDamage(damage);

            Destroy(gameObject);
        }
    }

    private void FinishAttack()
    {
        Destroy(gameObject);
    }
}
