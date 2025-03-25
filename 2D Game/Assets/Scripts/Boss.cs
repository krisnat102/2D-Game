using System.Collections.Generic;
using UnityEngine;

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
            //Debug.Log("melee");
            enemy.Attack(true);
        }

        public void RangedAttack()
        {
            //Debug.Log("ranged");
            enemy.Attack(false);
        }

        public void Dash()
        {
            enemy.Dash();
        }

        public void SpecialRangedAttack()
        {
            //Debug.Log("specRanged");
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
            if (enemy.ActionCooldown) return;
            
            foreach (var action in actions)
            {
                //Special Ranged Attack
                if (action == Actions.SpecialRangedAttack)
                {
                    if (enemy.DetectAIRange.InSight && !enemy.AttackAIRange.InRange && !enemy.SpecialRangedAttackCooldown) SpecialRangedAttack();
                }

                // Melee Attack
                if (action == Actions.MeleeAttack)
                {
                    if (enemy.AttackAIRange.InRange && !enemy.AttackCooldown)
                    {
                        MeleeAttack();
                    }
                }

                //Ranged Attack
                if (action == Actions.RangedAttack)
                {
                    if (enemy.DetectAIRange.InSight && !enemy.AttackAIRange.InRange && !enemy.AttackCooldown) RangedAttack();
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
        SpecialAttack,
        SpecialRangedAttack,
        Dash
    }
}
