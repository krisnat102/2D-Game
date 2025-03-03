using UnityEngine;


namespace Interactables
{
    public class Platform : MonoBehaviour
    {
        CompositeCollider2D platform;

        [SerializeField] private Transform player;

        [SerializeField] float offset = 1f;

        private void Start()
        {
            platform = GetComponent<CompositeCollider2D>();
        }

        private void Update()
        {
            if (player.position.y < transform.position.y + offset)
            {
                platform.enabled = false;
            }
            else
            {
                platform.enabled = true;
            }
        }
    }
}