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

    public bool InRange { get => inRange; private set => inRange = value; }
    public bool InSight { get => inSight; private set => inSight = value; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();

        if (player && attackOrDetectRange)
        {
            InRange = true;
        }
        if (player && !attackOrDetectRange)
        {
            InSight = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player && attackOrDetectRange)
        {
            InRange = false;
        }
        if (player && !attackOrDetectRange)
        {
            InSight = false;
        }
    }

    public bool PlayerInRange()
    {
        return InRange;
    }
    public bool PlayerInSight()
    {
        return InSight;
    }

    private void Update()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, playerTrans.position - firePoint.position, Mathf.Infinity, ~IgnoreMe); //shoots a ray from the firepoint to the player

        if (hitInfo)
        {
            Player player = hitInfo.transform.GetComponent<Player>(); //checks if it hit the player
            if (player != null)
            {
                InSight = true;
            }
            else InSight = false;
        }
    }
}
