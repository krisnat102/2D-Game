using UnityEngine;

namespace Krisnat
{
    public class Checkpoint : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Player>())
            {
                CoreClass.GameManager.checkpoint = transform.position;
            }
        }
    }
}
