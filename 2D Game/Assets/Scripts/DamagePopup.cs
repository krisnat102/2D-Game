using UnityEngine;

namespace Krisnat
{
    public class DamagePopup : MonoBehaviour
    {
        [SerializeField] private float moveYSpeed;
        [SerializeField] private bool followCharacter;

        private Enemy enemy;
        private int oldFacingDirection;
        private float startingXPosition;
        private float startingYPosition;

        void Update()
        {
            if (followCharacter)
            {
                transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;
            }
            else
            {
                startingYPosition += moveYSpeed * Time.deltaTime;
                transform.position = new Vector3(startingXPosition, startingYPosition, transform.position.z);
            }

            if (enemy.FacingDirection != oldFacingDirection)
            {
                Flip();
            }
            oldFacingDirection = enemy.FacingDirection;
        }

        private void Start()
        {
            startingXPosition = transform.position.x;
            startingYPosition = transform.position.y;
            enemy = GetComponentInParent<Enemy>();

            oldFacingDirection = enemy.FacingDirection;

            if (enemy.FacingDirection == -1)
            {
                Flip();
            }
        }

        private void Flip()
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }
}
