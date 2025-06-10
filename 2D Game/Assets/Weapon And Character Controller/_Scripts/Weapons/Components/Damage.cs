using Bardent.CoreSystem;
using Krisnat;
using UnityEngine;
using UnityEngine.UI;

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
                float damage = currentAttackData.Amount * levelHandler.StrengthDamage;
                if (item.TryGetComponent(out Enemy enemy))
                {
                    enemy.TakeDamage(damage, 0, false);
                    FrameFreeze.Instance.FreezeFrame(damage / FrameFreeze.Instance.AttackFreezeLength);
                }
                else if (item.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Damage(damage, true);
                }
            }
        }

        protected override void Start()
        {
            base.Start();

            hitBox = GetComponent<ActionHitBox>();
            levelHandler = CoreClass.GameManager.Instance.gameObject.GetComponentInParent<LevelHandler>();

            hitBox.OnDetectedCollider2D += HandleDetectCollider2D;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            hitBox.OnDetectedCollider2D -= HandleDetectCollider2D;
        }
    }
}