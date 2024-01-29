using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject impactEffect;

    [Header("Stats")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float bulletDmg = 20f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D (Collider2D hitInfo)
    {
        if (hitInfo.tag != "Player" && hitInfo.tag != "Item" && hitInfo.tag != "Climbable" && hitInfo.tag != "AttackRange" && hitInfo.tag != "BackgroundObject" && hitInfo.tag != "PickupRange")
        {
            Enemy enemy = hitInfo.GetComponent<Enemy>();
            if(enemy == true)
            {
                enemy.TakeDamage(bulletDmg, 0, false);

                Instantiate(enemy.BloodEffect, transform.position, Quaternion.identity);
            }
            Instantiate(impactEffect, transform.position, transform.rotation);
            /*impactEffect.SetActive(true);
            impactEffect.transform.position = transform.position;
            impactEffect.transform.rotation = transform.rotation;*/

            //gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
