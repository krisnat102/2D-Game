using UnityEngine;

public class Slash : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float slashDmg = 30f;
    [SerializeField] private float slashSpeed = 2f;

    private Rigidbody2D rb;
    private MeleeWeaponSprite meleeWeaponSprite;
    private Vector3 scale;

    private void Start()
    {
        meleeWeaponSprite = FindObjectOfType<MeleeWeaponSprite>().GetComponent<MeleeWeaponSprite>();
        rb = GetComponent<Rigidbody2D>();

        if(meleeWeaponSprite.Side)
        {
            rb.velocity = transform.right * slashSpeed;
        }
        else
        {
            rb.velocity = -transform.right * slashSpeed;

            Vector3 scale = transform.localScale;
            scale.x *= -1;

            transform.localScale = scale;
        }
    }

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
