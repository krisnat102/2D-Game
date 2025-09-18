using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class LandingDamage : MonoBehaviour
    {
        [SerializeField] private float damageMult;
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private LayerMask entitiesLayers;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & groundLayers) != 0)
            {

            }

            if (((1 << collision.gameObject.layer) & entitiesLayers) != 0)
            {

            }
        }
    }
}
