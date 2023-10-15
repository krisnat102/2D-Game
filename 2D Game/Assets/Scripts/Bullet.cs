using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] private float speed = 20f;

    public Rigidbody2D rb;

    [SerializeField] private int bulletDmg = 20;

    public GameObject impactEffect;

    void Start()
    {
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D (Collider2D hitInfo)
    {
        if (hitInfo.tag != "Player" && hitInfo.tag != "Item")
        {
            Enemy enemy = hitInfo.GetComponent<Enemy>();
            if(enemy == true)
            {
                enemy.TakeDamage(bulletDmg);
            }

            Instantiate(impactEffect, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}
