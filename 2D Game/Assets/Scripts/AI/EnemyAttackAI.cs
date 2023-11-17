using UnityEngine;

public class EnemyAttackAI : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    private bool inRange;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inRange = true;
            Debug.Log("3");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inRange = false;
            Debug.Log("4");
        }
    }

    public bool PlayerInRange()
    {
        return inRange;
    }
}
