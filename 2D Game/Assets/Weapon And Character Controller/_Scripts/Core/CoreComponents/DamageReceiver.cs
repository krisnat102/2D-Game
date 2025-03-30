using UnityEngine;

namespace Bardent.CoreSystem
{
    public class DamageReceiver : CoreComponent
    {
        [SerializeField] private GameObject damageParticles;

        private Stats stats;
        private ParticleManager particleManager;
        private AudioSource damageAudio;
        private bool invincible;

        public bool Invincible { get => invincible; set => invincible = value; }

        public void Damage(float rawAmount, bool physical)
        {
            if (Invincible) return;

            if (physical)
            {
                stats.health.Decrease(stats.CalculatePhysicalDamageReduction(rawAmount));
            }
            else
            {
                stats.health.Decrease(stats.CalculateMagicalDamageReduction(rawAmount));
            }

            damageAudio?.Play();
            Krisnat.CameraShake.Instance.ShakeCamera(0.15f, rawAmount / 10);
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