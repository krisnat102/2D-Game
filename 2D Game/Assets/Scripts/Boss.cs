using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Krisnat
{
    public class Boss : MonoBehaviour
    {
        #region Private Variables
        [SerializeField] private List<Actions> actions;

        private Enemy enemy;
        #endregion

        #region Actions
        public void MeleeAttack()
        {
            enemy.Attack(true);
        }

        public void RangedAttack()
        {
            enemy.Attack(false);
        }

        public void Dash()
        {
            enemy.Dash();
        }

        public void SpecialRangedAttack()
        {
            enemy.SpecialRangedAttack();
        }
        #endregion

        #region Unity Methods
        private void Start()
        {
            enemy = GetComponent<Enemy>();
        }
        private void Update()
        {
            // Melee Attack
            foreach (var action in actions)
            {
                if(action == Actions.MeleeAttack)
                {
                    if (enemy.AttackAIRange.InRange)
                    {
                        MeleeAttack();
                    }
                }
            }

            // Special Ranged Attack
            foreach (var action in actions)
            {
                if (action == Actions.SpecialRangedAttack)
                {
                    if (enemy.DetectAIRange.InSight && !enemy.AttackAIRange.InRange && !enemy.SpecialRangedAttackCooldown) SpecialRangedAttack();
                }
            }

            // Ranged attack
            foreach (var action in actions)
            {
                if (action == Actions.RangedAttack)
                {
                    if (enemy.DetectAIRange.InSight && !enemy.AttackAIRange.InRange && enemy.SpecialRangedAttackCooldown) RangedAttack();
                }
            }
        }
        #endregion
    }

    enum Actions
    {
        Empty,
        MeleeAttack,
        RangedAttack,
        SpecialRangedAttack,
        Dash
    }
}
