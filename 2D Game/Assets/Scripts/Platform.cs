using UnityEngine;

namespace Interactables
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] private Collider2D playerCollider;
        [SerializeField] private float dropTime = 0.3f;

        void Start()
        {
            playerCollider = GetComponent<Collider2D>();
        }

        void Update()
        {
            if (PlayerInputHandler.Instance.NormInputY < 0)
            {
                StartCoroutine(DropThroughPlatform());
            }
        }

        private System.Collections.IEnumerator DropThroughPlatform()
        {
            playerCollider.enabled = false;
            yield return new WaitForSeconds(dropTime);
            playerCollider.enabled = true;
        }
    }
}
