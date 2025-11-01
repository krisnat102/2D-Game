using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class SecretRoom : MonoBehaviour
    {
        [SerializeField] private GameObject roomPrefab;
        [SerializeField] private float fadeOutTime;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var player = collision.GetComponent<Player>();

            if (player)
            {
                FadeOutObject.instance.FadeOutObj(roomPrefab, fadeOutTime);
            }
        }
    }
}
