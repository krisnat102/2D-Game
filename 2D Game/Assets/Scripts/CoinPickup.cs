using System.Collections.Generic;
using UnityEngine;
using Inventory;

namespace Krisnat
{
    public class CoinPickup : MonoBehaviour
    {
        private new ParticleSystem particleSystem;
        private List<ParticleSystem.Particle> pickedUp = new List<ParticleSystem.Particle>();

        private void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnParticleTrigger()
        {
            int numEnter = particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, pickedUp);

            for (int i = 0; i < numEnter; i++)
            {
                ParticleSystem.Particle particle = pickedUp[i];

                InventoryManager.Instance.IncreaseCoins(true);

                particle.remainingLifetime = 0.001f;
                pickedUp[i] = particle;
            }
            AudioManager.Instance.CoinPickupSound.Play();
            Debug.Log(InventoryManager.Instance.Coins);

            particleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, pickedUp);
        }
    }
}
