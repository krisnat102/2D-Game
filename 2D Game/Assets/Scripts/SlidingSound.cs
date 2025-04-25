using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class SlidingSound : MonoBehaviour
    {
        private Rigidbody2D rb; 
        [SerializeField] private AudioSource audioSource;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            audioSource.Pause();
        }

        void Update()
        {
            bool isMoving = rb.velocity.magnitude > 0.1f;

            if (isMoving && !audioSource.isPlaying)
            {
                audioSource.UnPause();
            }
            else if (!isMoving && audioSource.isPlaying)
            {
                audioSource.Pause(); 
            }
        }

    }
}
