using UnityEngine;
using Core;
using Bardent.CoreSystem;

namespace Interactables
{
    public class Spikes : MonoBehaviour
    {
        [SerializeField] private float spikeDmg;
        [SerializeField] private bool damageType;

        private void OnTriggerEnter2D(Collider2D hitinfo)
        {
            DamageReceiver player = hitinfo.GetComponentInChildren<DamageReceiver>();
            Enemy enemy = hitinfo.GetComponent<Enemy>();
            if (player)
            {
                player.Damage(spikeDmg, damageType);
            }
            else if (enemy)
            {
                enemy.TakeDamage(spikeDmg, 0, false);
            }
        }
    }
}