using UnityEngine;
using Core;
using System.Runtime.InteropServices;
public class EnemyAttackAI : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform playerTrans;
    [SerializeField] private LayerMask IgnoreMe;
    [Tooltip("If true it's an attack range, if false it's a detect range")]
    [SerializeField] private bool attackOrDetectRange;

    private bool inRange = false;
    private bool inSight = false;
    private bool inRangeOfSight = false;
    private bool flipTracker;
    private Enemy enemy;

    public bool InRange { get => inRange; private set => inRange = value; }
    public bool InSight { get => inSight; private set => inSight = value; }
    public bool InRangeOfSight { get => inRangeOfSight; private set => inRangeOfSight = value; }
    public bool Alerted { get; private set; } 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();

        if (player && attackOrDetectRange)
        {
            InRange = true;
            flipTracker = true;
        }
        else if (player && !attackOrDetectRange)
        {
            InRangeOfSight = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player && attackOrDetectRange)
        {
            InRange = false;
            if (flipTracker == false)
            {
                Flip();
                flipTracker = true;
            }
        }
        else if (player && !attackOrDetectRange)
        {
            InRangeOfSight = false;
        }
    }

    private void Update()
    {
        if (InRangeOfSight)
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, playerTrans.position - firePoint.position, Mathf.Infinity, ~IgnoreMe); //shoots a ray from the fire point to the player

            if (hitInfo)
            {
                Player player = hitInfo.transform.GetComponent<Player>(); //checks if it hit the player
                if (player != null)
                {
                    InSight = true;
                    Alerted = true;
                }
                else InSight = false;
            }
        }

        if (InRange && enemy.data.flipWhenPlayerIsRight && playerTrans.position.x > enemy.transform.position.x && flipTracker)
        {
            Flip();
            flipTracker = false;
        }
        if (InRange && enemy.data.flipWhenPlayerIsRight && playerTrans.position.x < enemy.transform.position.x && !flipTracker)
        {
            Flip();
            flipTracker = true;
        }
    }
    private void Start()
    {
        enemy = transform.parent.GetComponent<Enemy>();
    }

    private void Flip()
    {
        enemy.transform.localScale = new Vector3(-1 * enemy.transform.localScale.x, enemy.transform.localScale.y, enemy.transform.localScale.z);
    }
}
