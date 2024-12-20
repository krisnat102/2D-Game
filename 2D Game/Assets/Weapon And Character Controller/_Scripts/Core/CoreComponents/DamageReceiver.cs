using UnityEngine;

namespace Bardent.CoreSystem
{
    public class DamageReceiver : CoreComponent, IDamageable
    {
        [SerializeField] private GameObject damageParticles;

        private Stats stats;
        private ParticleManager particleManager;
        private AudioSource damageAudio;

        public void Damage(float rawAmount, bool physical)
        {
            if (physical)
            {
                stats.Health.Decrease(stats.CalculatePhysicalDamageReduction(rawAmount));
            }
            else
            {
                stats.Health.Decrease(stats.CalculateMagicalDamageReduction(rawAmount));
            }

            damageAudio.Play();
            particleManager.StartParticlesWithRandomRotation(damageParticles);
        }

        protected override void Awake()
        {
            base.Awake();

            damageAudio = GetComponent<AudioSource>();
            stats = Core.GetCoreComponent<Stats>();
            particleManager = Core.GetCoreComponent<ParticleManager>();
        }
    }
}