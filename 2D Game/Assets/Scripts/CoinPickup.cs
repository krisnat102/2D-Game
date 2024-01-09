using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

namespace Krisnat
{
    public class CoinPickup : MonoBehaviour
    {
        private new ParticleSystem particleSystem;
        private AudioSource coinsPickupAudio;

        private List<ParticleSystem.Particle> pickedUp = new List<ParticleSystem.Particle>();

        private void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
            coinsPickupAudio = GetComponent<AudioSource>();
        }

        private void OnParticleTrigger()
        {
            int numEnter = particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, pickedUp);

            for (int i = 0; i < numEnter; i++)
            {
                ParticleSystem.Particle particle = pickedUp[i];

                InventoryManager.Instance.IncreaseCoins();

                particle.remainingLifetime = 0.1f;
                pickedUp[i] = particle;
            }
            coinsPickupAudio.Play();
            Debug.Log(InventoryManager.Instance.Coins);

            particleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, pickedUp);
        }
    }
}
