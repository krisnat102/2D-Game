using Bardent.CoreSystem;
using Krisnat;
using UnityEngine;

namespace Bardent.Weapons.Components
{
    public class Damage : WeaponComponent<DamageData, AttackDamage>
    {
        private ActionHitBox hitBox;
        private LevelHandler levelHandler;


        private void HandleDetectCollider2D(Collider2D[] colliders)
        {
            foreach (var item in colliders)
            {
                if (item.TryGetComponent(out Enemy enemy))
                {
                    enemy.TakeDamage(currentAttackData.Amount * levelHandler.StrengthDamage, 0, false);
                }
                else if (item.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Damage(currentAttackData.Amount, true);
                }
            }
        }

        protected override void Start()
        {
            base.Start();

            hitBox = GetComponent<ActionHitBox>();
            levelHandler = Stats.Instance.gameObject.GetComponentInParent<LevelHandler>();

            hitBox.OnDetectedCollider2D += HandleDetectCollider2D;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            hitBox.OnDetectedCollider2D -= HandleDetectCollider2D;
        }
    }
}