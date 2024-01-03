using UnityEngine;
using UnityEngine.UI;
using Core;
using Bardent.CoreSystem;
using Pathfinding;
using TMPro;

public class Enemy : MonoBehaviour
{
    #region Serialized Variables
    [Header("Hp Bar")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private Gradient hpBarGradient;
    [SerializeField] private Image hpBarFill;
    [SerializeField] private GameObject damagePopup;
    [SerializeField] private Vector3 damagePopupOffset;
    [SerializeField] private Gradient damagePopupGradient;
    [SerializeField] private int fontSizeToDamageScaler;

    [Header("Other")]
    public EnemyData data;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private Transform playerTrans;
    #endregion

    #region Private Variables
    private float offsetXSave;
    private Rigidbody2D rb;
    private Animator animator;
    private bool immune = false;
    private bool attackCooldown = false;
    private bool flip;
    private bool childFlip;
    private bool facingSide;
    private float hp;
    private float lvlIndex;
    private Vector2 offset;
    private Collider2D[] detected;
    private int facingDirection = 1;
    private Player player;
    private AIPath seekerAI;
    private EnemyAI enemyAI;
    private EnemyAttackAI enemyAttackAI;
    #endregion

    #region Properties
    public GameObject BloodEffect { get => Data.bloodEffect; private set => Data.bloodEffect = value; }
    public Transform PlayerTrans { get => playerTrans; private set => playerTrans = value; }
    public EnemyData Data { get => data; private set => data = value; }
    public float EnemyLevelScale { get => lvlIndex; private set => lvlIndex = value; }
    public int FacingDirection { get => facingDirection; private set => facingDirection = value; }
    #endregion

    #region Combat
    public void TakeDamage(float rawDamage, float knockback)
    {
        float damage = Mathf.Round(rawDamage);
        
        if (immune == false)
        {
            hp -= damage;
            hpBar.value = hp;
            hpBarFill.color = hpBarGradient.Evaluate(hpBar.normalizedValue);

            if (damage > 0)
            {
                if(GetComponentInChildren<Canvas>() != null && damagePopup != null)
                {
                    var dmgNumber = Instantiate(damagePopup, transform.position + damagePopupOffset, Quaternion.identity, GetComponentInChildren<Canvas>().transform).GetComponent<TextMeshProUGUI>();
                    dmgNumber.text = damage.ToString();
                    dmgNumber.color = damagePopupGradient.Evaluate(Mathf.Clamp01(damage/100));
                    dmgNumber.fontSize += Mathf.Round(damage / fontSizeToDamageScaler);
                }

                animator.SetTrigger("Hurt");
                immune = true;
                
                Invoke("StopImmune", 0.1f);
                TakeKnockback(damage + knockback);
            }
        }
        if (hp <= 0)
        {
            Die();
        }
    }
    private void TakeKnockback(float damage)
    {
        float knockback = damage * Data.knockbackModifier;

        if (playerTrans.position.x < transform.position.x + 0.5f)
        {
            rb.AddForce(transform.up * knockback, ForceMode2D.Force);
            rb.AddForce(transform.right * knockback, ForceMode2D.Force);
        }
        else
        {
            rb.AddForce(transform.up * knockback, ForceMode2D.Force);
            rb.AddForce(transform.right * -knockback, ForceMode2D.Force);
        }
    }

    private void Die()
    {
        Instantiate(Data.deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public void Attack()
    {
        if (attackCooldown == false && Data.ranged == false)
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

    private void AttackSpawn()
    {
        /*if(Data.ranged == false)
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
        }*/
        if(Data.ranged)
        {
            GameObject attackProjectile = Instantiate(Data.projectile, Data.firePoint);
            if (attackProjectile.activeInHierarchy == false)
            {
                attackProjectile.SetActive(true);
            }
            return;
        }
        offset.Set(
            transform.position.x + (data.HitBox.center.x * FacingDirection * -1),
            transform.position.y + data.HitBox.center.y
        );

        attackSound.Play();

        detected = Physics2D.OverlapBoxAll(offset, data.HitBox.size, 0f, data.DetectableLayers);
        
        if (detected.Length == 0) return;

        foreach (Collider2D obj in detected)
        {
            player = obj.gameObject.GetComponent<Player>();

            if (player)
            {
                player.Core.GetCoreComponent<DamageReceiver>().Damage(data.damage * EnemyLevelScale, data.damageType);
            }
        }
    }

    private void StopImmune() => immune = false;
    private void AttackCooldown() => attackCooldown = false;
    #endregion

    #region Unity Methods
    private void Update()
    {
        if (enemyAttackAI.PlayerInRange()) Invoke("Attack", 0.2f);

        if (transform != null && flip && PlayerStats.death == false)
        {
            FacingDirection *= -1;
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
        else if (transform != null && childFlip && PlayerStats.death == false)
        {
            FacingDirection *= -1;
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

        FacingDirection = transform.localScale.x > 0 ? 1 : -1;

        if (seekerAI != null)
        {
            if (enemyAttackAI.PlayerInRange())
            {
                seekerAI.enabled = true;
            }
            else seekerAI.enabled = false;
        }

        if (enemyAI != null)
        {
            if (enemyAttackAI.PlayerInRange())
            {
                enemyAI.enabled = true;
            }
            else enemyAI.enabled = false;
        }
    }

    private void Start()
    {
        lvlIndex = Data.level * 0.1f + 0.9f;

        offsetXSave = Data.offsetX;

        seekerAI = GetComponent<AIPath>();
        enemyAI = GetComponent<EnemyAI>();
        enemyAttackAI = GetComponentInChildren<EnemyAttackAI>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        flip = ContainsParam(animator, "Flip");

        hp = Data.maxHP * lvlIndex;
        hpBar.maxValue = Data.maxHP * lvlIndex;
        TakeDamage(0, 0);

        if (transform.rotation.y == 0)
        {
            facingSide = true;
        }
        else facingSide = false;
    }
    #endregion

    #region OtherMethods
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
    #endregion

    #region GettersSetters
    public bool GetRanged()
    {
        return Data.ranged;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 adjustedCenter = transform.position + new Vector3(data.HitBox.center.x * FacingDirection * -1, data.HitBox.center.y, 0f);
        Gizmos.DrawWireCube(adjustedCenter, data.HitBox.size);
    }
    #endregion
}
