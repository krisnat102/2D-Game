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
            if (!enemy) return;
            if (followCharacter)
            {
                transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;
            }
            else
            {
                startingYPosition += moveYSpeed * Time.deltaTime;
                transform.position = new Vector3(startingXPosition, startingYPosition, transform.position.z);
            }

            if (enemy && enemy.FacingDirection != oldFacingDirection)
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
            if (!enemy)
            {
                transform.parent.localScale = new Vector2(Mathf.Abs(transform.parent.localScale.x), transform.parent.localScale.y);
                return;
            }

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
