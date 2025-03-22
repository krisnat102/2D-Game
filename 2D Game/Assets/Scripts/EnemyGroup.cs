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
            foreach (var enemy in enemyAwareness)
            {
                enemy.Alerted = true;
            }
        }
    }
}
