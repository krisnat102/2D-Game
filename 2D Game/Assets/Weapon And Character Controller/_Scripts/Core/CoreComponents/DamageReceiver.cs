using UnityEngine;

namespace Bardent.CoreSystem
{
    public class DamageReceiver : CoreComponent, IDamageable
    {
        [SerializeField] private GameObject damageParticles;

        private Stats stats;
        private ParticleManager particleManager;

        public void Damage(float amount) {
            stats.Health.Decrease(amount);
            particleManager.StartParticlesWithRandomRotation(damageParticles);
        }

        protected override void Awake()
        {
            base.Awake();

            stats = Core.GetCoreComponent<Stats>();
            particleManager = Core.GetCoreComponent<ParticleManager>();
        }
    }
}