using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float hp = 100f;
    [SerializeField] private float attackSpeed = 1f;

    [Header("Offsets")]
    [SerializeField] private float offsetX = 1.2f;
    [SerializeField] private float offsetX2 = 1.2f;
    [SerializeField] private float offsetY = -1f;
    private float offsetXSave;

    [Header("Ranged")]
    [SerializeField] private bool ranged = false;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectile;

    [Header("Attack")]
    [SerializeField] private EnemyAttackAI enemyAttackAI;
    [SerializeField] private GameObject attack;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private float attackAnimLength = 0.3f;

    [Header("Other")]
    [SerializeField] private Transform playerTrans;
    [SerializeField] private GameObject deathEffect;

    [Header("Customiseable")]
    [SerializeField] private bool lookAtPlayer;

    private Animator animator;
    private bool immune = false;
    private bool attackCooldown = false;
    private bool flip;
    private bool facingSide;

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
        if (attackCooldown == false && ranged == false && attack != null)
        {
            animator.SetTrigger("Attack");

            attackCooldown = true;
            Invoke("AttackCooldown", attackSpeed);
            Invoke("AttackSpawn", attackAnimLength);
        }
        else if(attackCooldown == false && ranged && enemyAttackAI.PlayerInSight())
        {
            animator.SetTrigger("Attack");
            Invoke("AttackSpawn", attackAnimLength);

            attackCooldown = true;
            Invoke("AttackCooldown", attackSpeed);
        }
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

        if (transform != null && flip && PlayerStats.death == false)
        {
            if (transform.position.x < playerTrans.position.x)
            {
                offsetX = -offsetX2;
                animator.SetBool("Flip", true);
            }
            else
            {
                offsetX = offsetXSave;
                animator.SetBool("Flip", false);
            }
        }

        if (lookAtPlayer && enemyAttackAI.PlayerInSight() && PlayerStats.death == false)
        {
            if (facingSide)
            {
                if (playerTrans.position.x < transform.position.x + 0.5f)
                {
                    transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else if (playerTrans.position.x > transform.position.x - 0.5f)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
            }
            else
            {
                if (playerTrans.position.x < transform.position.x + 0.5f)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else if (playerTrans.position.x > transform.position.x - 0.5f)
                {
                    transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

                }
            }
        }
    }
    

    private void AttackSpawn()
    {
        if(ranged == false)
        {
            Vector3 offsetAttack = new Vector3(offsetX, offsetY, 0f);

            Instantiate(attack, transform.position - offsetAttack, Quaternion.identity);

            attackSound.Play();
        }
        else
        {
            GameObject attackProjectile = Instantiate(projectile, firePoint);
            if (attackProjectile.activeInHierarchy == false)
            {
                attackProjectile.SetActive(true);
            }
        }
    }

    private void Start()
    {
        offsetXSave = offsetX;

        flip = ContainsParam(animator, "Flip");

        animator = GetComponent<Animator>();

        if (transform.rotation.y == 0)
        {
            facingSide = true;
        }
        else facingSide = false;
    }

    private bool ContainsParam(Animator _Anim, string _ParamName)
    {
        if(_Anim != null)
        {
            foreach (AnimatorControllerParameter param in _Anim.parameters)
            {
                if (param.name == _ParamName) return true;
            }
        }
        return false;
    }

    public bool Ranged()
    {
        return ranged;
    }
}
