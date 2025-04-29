using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Krisnat
{
    public class EnemyGroup : MonoBehaviour
    {
        private List<EnemyAttackAI> enemyAwareness;

        private void Start()
        {
            enemyAwareness = GetComponentsInChildren<EnemyAttackAI>().ToList();
        }

        public void Alert()
        {
            foreach (var enemyAI in enemyAwareness)
            {
                enemyAI.Alerted = true;
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                var enemy = enemyAI.GetComponentInParent<Enemy>();

                if (enemy && enemy.Sleeping)
                {
                    enemy.WakeUp();
                }
            }
        }
    }
}
