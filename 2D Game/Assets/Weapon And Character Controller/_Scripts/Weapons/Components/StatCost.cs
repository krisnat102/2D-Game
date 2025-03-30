using Bardent.CoreSystem;

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
                Stats.stam.Decrease(currentAttackData.AttackCost);
                Stats.stam.StopRegen(currentAttackData.AttackRecoveryTime);
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