using UnityEngine;
using UnityEngine.Events;

namespace Krisnat
{
    public class Boss : MonoBehaviour
    {
        #region Private Variables
        [SerializeField] private Actions action1;
        [SerializeField] private Actions action2;
        [SerializeField] private Actions action3;

        private Actions[] actions = new Actions[2];
        private Enemy enemy;
        #endregion

        #region Actions
        public void MeleeAttack()
        {
            enemy.Attack();
        }

        public void RangedAttack()
        {
            GameObject attackProjectile = Instantiate(enemy.data.bossProjectile, enemy.FirePoint);
            if (attackProjectile.activeInHierarchy == false)
            {
                attackProjectile.SetActive(true);
            }
        }

        public void DashAttack()
        {
            enemy.Dash();
        }
        #endregion

        private void Start()
        {
            actions[0] = action1;
            actions[1] = action2;
            actions[2] = action3;

            enemy = GetComponent<Enemy>();
        }
    }

    enum Actions
    {
        MeleeAttack,
        RangedAttack,
        DashAttack
    }
}
