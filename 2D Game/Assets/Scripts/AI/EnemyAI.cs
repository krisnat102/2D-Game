using UnityEngine;
using Pathfinding;
using CoreClass;

public class EnemyAI : MonoBehaviour //https://www.youtube.com/watch?v=sWqRfygpl4I&ab_channel=Etredal
{
    #region Init variables
    [SerializeField] private EnemyDataAI dataAI;
    [Header("Pathfinding")]
    [SerializeField] private Transform target;

    [Header("Grounded Check")]

    [SerializeField] private LayerMask WhatIsGround;
    [SerializeField] private Transform GroundCheck;

    private Path path;
    private int currentWaypoint = 0;
    private bool isGrounded = false;
    private Seeker seeker;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 oldPosition;
    private Enemy enemy;

    const float GroundedRadius = .2f;

    public bool IsGrounded { get => isGrounded; private set => isGrounded = value; }

    #endregion

    public void Start()
    {
        enemy = GetComponent<Enemy>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        InvokeRepeating("UpdatePath", 0f, dataAI.pathUpdateTime);
    }

    private void FixedUpdate()
    {
        if (TargetInDistance() && dataAI.followEnabled)
        {
            PathFollow();
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, GroundedRadius, WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                IsGrounded = true;
            }
            else IsGrounded = false;
        }

        int horizontalMove;

        if (oldPosition.x != transform.position.x)
        {
            horizontalMove = 1;
        }
        else horizontalMove = 0;

        oldPosition = transform.position;

        if (enemy.ContainsParam(animator, "Speed")) animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (horizontalMove == 0)
        {
            if (enemy.ContainsParam(animator, "Standing")) animator.SetBool("Standing", true);
        }
        else if (enemy.ContainsParam(animator, "Standing")) animator.SetBool("Standing", false);
    }

    private void UpdatePath()
    {
        if (dataAI.followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count) //reached end of path
        {
            return;
        }

        //isGrounded = Physics2D.Raycast(transform.position, -Vector3.up, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset); //check if colliding with smth

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized; // calculates direction
        Vector2 force = direction * dataAI.speed * Time.deltaTime;

        if (dataAI.jumpEnabled && IsGrounded) //jump
        {
            if (direction.y > dataAI.jumpNodeHeightRequirement)
            {
                rb.AddForce(Vector2.up * dataAI.speed * dataAI.jumpModifier);
            }
        }

        rb.AddForce(force); //movement

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]); // next waypoint
        if (distance < dataAI.nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (dataAI.directionLookEnabled) //direction graphics
        {
            if (rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private bool TargetInDistance() 
    {
        if (target && dataAI)
            return Vector2.Distance(transform.position, target.transform.position) < dataAI.activateDistance;
        else return false;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}
