using UnityEngine;

namespace Krisnat
{
    public class Checkpoint : MonoBehaviour
    {
        private bool triggered;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Player>() && !triggered)
            {
                CoreClass.GameManager.checkpoint = transform.position;
                triggered = true;
            }
        }
    }
}
