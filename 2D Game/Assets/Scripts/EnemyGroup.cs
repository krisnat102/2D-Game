using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Krisnat
{
    public class EnemyGroup : MonoBehaviour
    {
        public bool Alerted { get; private set; }

        private List<EnemyAttackAI> enemyAwareness;
        private List<Enemy> enemies;

        private void Start()
        {
            enemyAwareness = GetComponentsInChildren<EnemyAttackAI>().ToList();
            enemies = GetComponentsInChildren<Enemy>(true).ToList();

            Player.instance.onRest.AddListener(ResetGroup);
        }

        public void Alert()
        {
            Alerted = true;

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

        private void ResetGroup()
        {
            foreach (var enemy in enemies)
            {
                if (enemy.Data.boss && enemy.Dead) return;

                enemy.gameObject.SetActive(true);
                enemy.ResetEnemy();
            }
        }
    }
}