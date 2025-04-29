using UnityEngine;
// ReSharper disable Unity.PerformanceCriticalCodeInvocation

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

            stats.health.Decrease(physical
                ? stats.CalculatePhysicalDamageReduction(rawAmount)
                : stats.CalculateMagicalDamageReduction(rawAmount));

            damageAudio?.Play();
            Krisnat.CameraShake.instance.ShakeCamera(0.15f, rawAmount / 10);
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