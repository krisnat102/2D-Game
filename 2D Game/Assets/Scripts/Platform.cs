namespace Interactables
{
    using UnityEngine;

    public class Platform : MonoBehaviour
    {
        private Collider2D playerCollider;
        [SerializeField] private float dropTime = 0.3f;

        void Start()
        {
            playerCollider = GetComponent<Collider2D>();
        }

        void Update()
        {
            if (PlayerInputHandler.Instance.NormInputY == -1)
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