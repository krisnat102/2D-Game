using UnityEngine;
using Bardent.CoreSystem;

namespace Interactables
{
    public class Spikes : MonoBehaviour
    {
        [SerializeField] private float damage;
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
                Invoke(nameof(ResetDamageCooldown), 0.3f);
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