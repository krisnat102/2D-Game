using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class RandomizeAnimationSpeed : MonoBehaviour
    {
        [SerializeField] private Animator animRandomSpeed;
        [SerializeField] private float upperAnimLimit;
        [SerializeField] private float lowerAnimLimit;

        private void Start()
        {
            if (animRandomSpeed)
            {
                animRandomSpeed.speed = Random.Range(lowerAnimLimit, upperAnimLimit);
            }
        }
    }
}
