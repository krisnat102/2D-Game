using UnityEngine;
using Bardent.CoreSystem;
using Krisnat;

namespace Interactables
{
    public class Spikes : MonoBehaviour
    {
        [SerializeField] private float damage;
        [SerializeField] private float damageFrequency = 1f;
        [SerializeField] private bool damageType;
        [SerializeField] private bool damageEnemies;

        private bool canDamagePlayer = true;

        private void OnTriggerStay2D(Collider2D collision)
        {
            Player player = collision.GetComponent<Player>();
            Enemy enemy = collision.GetComponent<Enemy>();

            if (player && canDamagePlayer)
            {
                player.Core.GetCoreComponent<DamageReceiver>().Damage(damage, damageType);
                canDamagePlayer = false;
                Invoke(nameof(ResetDamageCooldown), damageFrequency);
            }
            else if (enemy && damageEnemies)
            {
                enemy.TakeDamage(damage, 0, false);
            }
        }

        private void ResetDamageCooldown()
        {
            canDamagePlayer = true;
        }
    }
}