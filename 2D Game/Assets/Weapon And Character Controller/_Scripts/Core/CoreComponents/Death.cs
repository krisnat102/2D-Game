using UnityEngine;

namespace Bardent.CoreSystem
{
    public class Death : CoreComponent
    {
        public bool IsDead { get; set; }

        [SerializeField] private GameObject[] deathParticles;
        [SerializeField] private GameObject deathScreen;
        [SerializeField] private float deathDepth;

        private ParticleManager ParticleManager =>
            particleManager ? particleManager : Core.GetCoreComponent(ref particleManager);

        private ParticleManager particleManager;

        private Stats Stats => stats ? stats : Core.GetCoreComponent(ref stats);
        private Stats stats;

        public void Die()
        {
            foreach (var particle in deathParticles)
            {
                ParticleManager.StartParticles(particle);
            }

            if (deathScreen != null)
            {
                deathScreen.SetActive(true);
            }

            Core.transform.parent.gameObject.SetActive(false);
            IsDead = true;
        }

        private void Update()
        {
            if(transform.position.y < deathDepth)
            {
                Die();
            }
        }

        private void OnEnable() => Stats.Health.OnCurrentValueZero += Die;
        private void OnDisable() => Stats.Health.OnCurrentValueZero -= Die;
    }
}