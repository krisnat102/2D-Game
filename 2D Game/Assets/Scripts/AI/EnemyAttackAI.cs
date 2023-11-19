using UnityEngine;

public class EnemyAttackAI : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform playerTrans;

    [SerializeField] private LayerMask IgnoreMe;

    private bool inRange = false;
    private bool inSight = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inRange = false;
        }
    }

    public bool PlayerInRange()
    {
        return inRange;
    }
    public bool PlayerInSight()
    {
        return inSight;
    }

    private void Update()
    {
        if(inRange && enemy.Ranged())
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, playerTrans.position - firePoint.position, Mathf.Infinity, ~IgnoreMe); //shoots a ray from the firepoint to the player

            if (hitInfo)
            {
                PlayerStats player = hitInfo.transform.GetComponent<PlayerStats>(); //checks if it hit the player
                if (player != null)
                {
                    inSight = true;
                }
                else inSight = false;
                //Debug.Log("player not seen");
            }
        }
    }
}
