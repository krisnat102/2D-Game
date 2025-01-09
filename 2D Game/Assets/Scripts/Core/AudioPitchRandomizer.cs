using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class AudioPitchRandomizer : MonoBehaviour
    {
        [Range(0f, 2f)]
        [SerializeField] private float lowPitchBoundary = 1f;
        [Range(0.5f, 5f)]
        [SerializeField] private float highPitchBoundary = 1f;

        private AudioSource audioSource;

        private void OnEnable()
        {
            audioSource = GetComponent<AudioSource>();

            if(audioSource) audioSource.pitch = Random.Range(lowPitchBoundary, highPitchBoundary);
            Debug.Log(audioSource.pitch);
        }
    }
}
