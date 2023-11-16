using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float hp = 100f;
    [SerializeField] private float attackSpeed = 1f;

    [SerializeField] private GameObject deathEffect;

    [SerializeField] private GameObject attack;

    [SerializeField] private float offsetX = 1.2f;
    [SerializeField] private float offsetX2 = 1.2f;
    [SerializeField] private float offsetY = -1f;

    [SerializeField] private float attackAnimLength = 0.3f;

    [SerializeField] private Animator animator;

    private bool immune = false;
    private bool attackCooldown = false;

    [SerializeField] private EnemyAttackAI enemyAttackAI;

    [SerializeField] private AudioSource attackSound;

    [SerializeField] private Transform playerTrans;
    [SerializeField] private Transform enemyTrans;

    private float offsetXSave;

    public void TakeDamage(float damage)
    {
        if (immune == false)
        {
            hp -= damage;

            animator.SetTrigger("Hurt");

            immune = true;

            Invoke("StopImmune", 0.1f);
        }
        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public void Attack()
    {
        Debug.Log(attackCooldown);
        if (attackCooldown == false)
        {
            animator.SetTrigger("Attack");

            attackCooldown = true;
            Invoke("AttackCooldown", attackSpeed);
            Invoke("AttackSpawn", attackAnimLength);
        }
        else Debug.Log("cooldown");
    }

    private void StopImmune()
    {
        immune = false;
    }
    private void AttackCooldown()
    {
        attackCooldown = false;
    }

    private void Update()
    {
        if (enemyAttackAI.PlayerInRange()) Invoke("Attack", 0.2f);

        if (enemyTrans.position.x < playerTrans.position.x)
        {
            offsetX = -offsetX2;
        }
        else offsetX = offsetXSave;
    }

    private void AttackSpawn()
    {
        Vector3 offsetAttack = new Vector3(offsetX, offsetY, 0f);

        Instantiate(attack, transform.position - offsetAttack, Quaternion.identity);

        attackSound.Play();
    }

    private void Start()
    {
        offsetXSave = offsetX;
    }
}
