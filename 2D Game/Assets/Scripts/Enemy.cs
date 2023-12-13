using UnityEngine;
using UnityEngine.UI;
using Core;

public class Enemy : MonoBehaviour
{
    [Header("Other")]
    [SerializeField] private EnemyData data;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private Transform playerTrans;
    [SerializeField] private EnemyAttackAI enemyAttackAI;
    
    [Header("Hp Bar")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fill;

    private float offsetXSave;
    private Rigidbody2D rb;
    private Animator animator;
    private bool immune = false;
    private bool attackCooldown = false;
    private bool flip;
    private bool facingSide;
    private float hp;
    private float lvlIndex;

    public GameObject BloodEffect { get => Data.bloodEffect; private set => Data.bloodEffect = value; }
    public Transform PlayerTrans { get => playerTrans; private set => playerTrans = value; }
    public EnemyData Data { get => data; private set => data = value; }
    public float LevelIndex { get => lvlIndex; private set => lvlIndex = value; }

    public void TakeDamage(float damage)
    {
        if (immune == false)
        {
            hp -= damage;
            hpBar.value = hp;
            fill.color = gradient.Evaluate(hpBar.normalizedValue);

            if (damage > 0)
            {
                animator.SetTrigger("Hurt");

                immune = true;

                Invoke("StopImmune", 0.1f);

                TakeKnockback(damage);
            }
        }
        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Instantiate(Data.deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void TakeKnockback(float damage)
    {
        float knockback = damage * Data.knockbackModifier;

        if (playerTrans.position.x < transform.position.x + 0.5f)
        {
            rb.AddForce(transform.up * knockback / 2, ForceMode2D.Force);
            rb.AddForce(transform.right * knockback, ForceMode2D.Force);
        }
        else
        {
            rb.AddForce(transform.up * knockback, ForceMode2D.Force);
            rb.AddForce(transform.right * -knockback, ForceMode2D.Force);
        }
    }

    public void Attack()
    {
        if (attackCooldown == false && Data.ranged == false && Data.attack != null)
        {
            animator.SetTrigger("Attack");

            attackCooldown = true;
            Invoke("AttackCooldown", Data.attackSpeed);
            Invoke("AttackSpawn", Data.attackAnimLength);
        }
        else if (attackCooldown == false && Data.ranged && enemyAttackAI.PlayerInSight())
        {
            animator.SetTrigger("Attack");
            Invoke("AttackSpawn", Data.attackAnimLength);

            attackCooldown = true;
            Invoke("AttackCooldown", Data.attackSpeed);
        }
    }

    private void StopImmune() => immune = false;
    private void AttackCooldown() => attackCooldown = false;

    private void Update()
    {
        if (enemyAttackAI.PlayerInRange()) Invoke("Attack", 0.2f);

        if (transform != null && flip && PlayerStats.death == false)
        {
            if (transform.position.x < playerTrans.position.x)
            {
                Data.offsetX = -Data.offsetX2;
                animator.SetBool("Flip", true);
            }
            else
            {
                Data.offsetX = offsetXSave;
                animator.SetBool("Flip", false);
            }
        }

        if (Data.lookAtPlayer && enemyAttackAI.PlayerInSight() && PlayerStats.death == false)
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
        if (Data.ranged == false)
        {
            Vector3 offsetAttack = new Vector3(Data.offsetX, Data.offsetY, 0f);

            GameObject attack = Instantiate(Data.attack , transform.position - offsetAttack, Quaternion.identity);
            attack.transform.parent = gameObject.transform;

            attackSound.Play();
        }
        else
        {
            GameObject attackProjectile = Instantiate(Data.projectile, Data.firePoint);
            if (attackProjectile.activeInHierarchy == false)
            {
                attackProjectile.SetActive(true);
            }
        }
    }

    private void Start()
    {
        lvlIndex = Data.level * 0.1f + 0.9f;

        rb = GetComponent<Rigidbody2D>();

        offsetXSave = Data.offsetX;

        flip = ContainsParam(animator, "Flip");

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        hp = Data.maxHP * lvlIndex;
        hpBar.maxValue = Data.maxHP * lvlIndex;
        TakeDamage(0);

        if (transform.rotation.y == 0)
        {
            facingSide = true;
        }
        else facingSide = false;
    }

    //checks if a parameter exists in the animator, found here https://discussions.unity.com/t/is-there-a-way-to-check-if-an-animatorcontroller-has-a-parameter/86194
    private bool ContainsParam(Animator _Anim, string _ParamName)
    {
        if (_Anim != null)
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
        return Data.ranged;
    }
}
