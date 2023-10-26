using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float hp = 100f;

    public GameObject deathEffect;

    public GameObject attack;

    [SerializeField] private float offsetX = 1.2f;
    [SerializeField] private float offsetY = -1f;

    [SerializeField] private Animator animator;

    public static float offsetXStatic;
    public static float offsetYStatic;

    bool immune = false;


    public void TakeDamage(float damage)
    {
        if(immune == false)
        {
            hp -= damage;

            animator.SetTrigger("Hurt");

            immune = true;

            Invoke("StopImmune", 0.2f);
        }
        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public void Attack()
    {
        Vector3 offsetAttack = new Vector3(offsetX, offsetY, 0f);

        Instantiate(attack, transform.position - offsetAttack, Quaternion.identity);

        animator.SetTrigger("Attack");
    }

    private void Start()
    {
        offsetXStatic = offsetX;
        offsetYStatic = offsetY;

        Invoke("Attack", 2f);
    }

    private void StopImmune()
    { 
        immune = false;
    }
}
