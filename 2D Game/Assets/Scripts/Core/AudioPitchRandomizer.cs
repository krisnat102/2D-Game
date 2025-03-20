using UnityEngine;

namespace Krisnat
{
    public class AudioPitchRandomizer : MonoBehaviour
    {
        [Range(0f, 2f)]
        [SerializeField] private float lowPitchBoundary = 1f;
        [Range(0.5f, 5f)]
        [SerializeField] private float highPitchBoundary = 1f;

        private AudioSource[] audioSources;

        private void OnEnable()
        {
            audioSources = GetComponents<AudioSource>();

            if (audioSources.Length > 0)
            {
                foreach (AudioSource source in audioSources)
                {
                    source.pitch = Random.Range(lowPitchBoundary, highPitchBoundary);
                }
            }

            int randomSFX = Mathf.RoundToInt(Random.Range(0, audioSources.Length));
            audioSources[randomSFX].Play();
        }
    }
}
