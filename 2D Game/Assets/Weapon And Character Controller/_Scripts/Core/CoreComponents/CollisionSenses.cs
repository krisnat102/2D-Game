using UnityEngine;

namespace Bardent.CoreSystem
{
    public class CollisionSenses : CoreComponent
    {

        private Movement Movement { get => movement ?? Core.GetCoreComponent(ref movement); }

        private Movement movement;

        #region Check Transforms

        public Transform GroundCheck
        {
            get => GenericNotImplementedError<Transform>.TryGet(groundCheck, Core.transform.parent.name);
            private set => groundCheck = value;
        }
        public Transform WallCheck
        {
            get => GenericNotImplementedError<Transform>.TryGet(wallCheck, Core.transform.parent.name);
            private set => wallCheck = value;
        }
        public Transform LedgeCheckHorizontal
        {
            get => GenericNotImplementedError<Transform>.TryGet(ledgeCheckHorizontal, Core.transform.parent.name);
            private set => ledgeCheckHorizontal = value;
        }
        public Transform LedgeCheckVertical
        {
            get => GenericNotImplementedError<Transform>.TryGet(ledgeCheckVertical, Core.transform.parent.name);
            private set => ledgeCheckVertical = value;
        }
        public Transform CeilingCheck
        {
            get => GenericNotImplementedError<Transform>.TryGet(ceilingCheck, Core.transform.parent.name);
            private set => ceilingCheck = value;
        }
        public float GroundCheckRadius { get => groundCheckRadius; set => groundCheckRadius = value; }
        public float WallCheckDistance { get => wallCheckDistance; set => wallCheckDistance = value; }
        public LayerMask WhatIsGround { get => whatIsGround; set => whatIsGround = value; }
        public LayerMask WhatIsPlatform { get => whatIsPlatform; set => whatIsPlatform = value; }


        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform wallCheck;
        [SerializeField] private Transform ledgeCheckHorizontal;
        [SerializeField] private Transform ledgeCheckVertical;
        [SerializeField] private Transform ceilingCheck;

        [SerializeField] private float groundCheckRadius;
        [SerializeField] private float wallCheckDistance;

        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private LayerMask whatIsPlatform;

        #endregion

        public bool Ceiling
        {
            get => Physics2D.OverlapCircle(CeilingCheck.position, groundCheckRadius, whatIsGround);
        }

        public bool Ground
        {
            get => Physics2D.OverlapCircle(GroundCheck.position, groundCheckRadius, whatIsGround) || Physics2D.OverlapCircle(GroundCheck.position, groundCheckRadius, whatIsPlatform);
        }

        public bool WallFront
        {
            get => Physics2D.Raycast(WallCheck.position, Vector2.right * Movement.FacingDirection, wallCheckDistance, whatIsGround);
        }

        public bool LedgeHorizontal
        {
            get => Physics2D.Raycast(LedgeCheckHorizontal.position, Vector2.right * Movement.FacingDirection, wallCheckDistance, whatIsGround);
        }

        public bool LedgeVertical
        {
            get => Physics2D.Raycast(LedgeCheckVertical.position, Vector2.down, wallCheckDistance, whatIsGround);
        }

        public bool WallBack
        {
            get => Physics2D.Raycast(WallCheck.position, Vector2.right * -Movement.FacingDirection, wallCheckDistance, whatIsGround);
        }
        
        private void OnDrawGizmos()
        {
            if (LedgeCheckHorizontal == null || Movement == null)
                return;

            Vector3 origin = LedgeCheckHorizontal.position;
            Vector3 direction = Vector2.right * Movement.FacingDirection;
            float distance = wallCheckDistance;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(origin, origin + direction.normalized * distance);
        }

    }
}
