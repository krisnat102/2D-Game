using Krisnat.Assets.Scripts;
using UnityEngine;

namespace Krisnat
{
    public class Checkpoint : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var player = collision.GetComponent<Player>();

            if (player)
            {
                CoreClass.GameManager.checkpoint = transform.position;
                SaveSystem.SavePlayer(player);
            }
        }
    }
}
