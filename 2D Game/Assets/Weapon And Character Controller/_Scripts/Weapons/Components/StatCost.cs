using Bardent.CoreSystem;
using System.Collections;
using UnityEngine;

namespace Bardent.Weapons.Components
{
    public class StatCost : WeaponComponent<StatCostData, AttackStatCost>
    {
        private Stats stats;

        private Stats Stats =>
            stats ? stats : Core.GetCoreComponent(ref stats);

        private void HandleAttackCost()
        {
            if (currentAttackData.StamCost)
            {
                Stats.Stam.Decrease(currentAttackData.AttackCost);
                Stats.Stam.StopRegen(currentAttackData.AttackRecoveryTime);
            }
        }

        protected override void Start()
        {
            base.Start();

            eventHandler.OnAttackAction += HandleAttackCost;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            eventHandler.OnAttackAction -= HandleAttackCost;
        }
    }
}