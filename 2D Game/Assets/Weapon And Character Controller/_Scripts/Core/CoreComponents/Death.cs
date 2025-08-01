using Inventory;
using UnityEngine;

namespace Bardent.CoreSystem
{
    public class Death : CoreComponent
    {
        public bool IsDead { get; set; }

        [SerializeField] private GameObject[] deathParticles;
        [SerializeField] private GameObject deathScreen;
        [SerializeField] private float deathDepth;

        private AudioSource deathSFX;

        private ParticleManager ParticleManager =>
            particleManager ? particleManager : Core.GetCoreComponent(ref particleManager);

        private ParticleManager particleManager;

        private Stats Stats => stats ? stats : Core.GetCoreComponent(ref stats);
        private Stats stats;

        private void Start()
        {
            deathSFX = GetComponentInChildren<AudioSource>();
        }

        public void Die()
        {
            foreach (var particle in deathParticles)
            {
                ParticleManager.StartParticles(particle, transform.position, transform.rotation);
            }

            if (deathScreen)
            {
                deathScreen.SetActive(true);
            }
            
            Time.timeScale = 1f;
            InventoryManager.Instance.SetCoins(InventoryManager.Instance.Coins / 3, false);
            CoreClass.GameManager.instance.SavePlayer();
            deathSFX.gameObject.transform.parent = CoreClass.GameManager.instance.Audios;
            deathSFX.Play();
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

        private void OnEnable() => Stats.health.OnCurrentValueZero += Die;
        private void OnDisable() => Stats.health.OnCurrentValueZero -= Die;
    }
}