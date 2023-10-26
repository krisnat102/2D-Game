using UnityEngine;

public class Slash : MonoBehaviour
{
    [SerializeField] private float slashDmg = 30f;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.tag != "Player")
        {
            Enemy enemy = hitInfo.GetComponent<Enemy>();
            if (enemy == true)
            {
                enemy.TakeDamage(slashDmg);
            }

        }
    }
}
