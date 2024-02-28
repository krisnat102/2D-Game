using UnityEngine;
using UnityEngine.UI;
using Bardent.CoreSystem;
using Pathfinding;
using TMPro;
using Inventory;
using Spells;
using System.Collections;
using UnityEngine.Rendering.UI;
using Krisnat;

public class Enemy : MonoBehaviour
{
    #region Private Variables
    [Header("Hp Bar")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private Gradient hpBarGradient;
    [SerializeField] private Image hpBarFill;
    [SerializeField] private GameObject damagePopup;
    [SerializeField] private Gradient damagePopupGradient;
    [SerializeField] private Vector3 damagePopupOffset;
    [SerializeField] private int fontSizeToDamageScaler;

    [Header("Ranges")]
    [SerializeField] private EnemyAttackAI detectAIRange;
    [SerializeField] private EnemyAttackAI attackAIRange, dashAIRange;
    [SerializeField] private Transform leftPatrolBarrier, rightPatrolBarrier;

    [Header("Sound")]
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource rangedAttackSound;
    [SerializeField] private AudioSource bossMusicPlayer;

    [Header("Other")]
    public EnemyData data;
    [SerializeField] private GameObject bossSpecialProjectile;
    [SerializeField] private Transform playerTrans;

    private int coinsDropped;
    private int facingDirection = 1;
    private float offsetXSave;
    private float hp;
    private float lvlIndex;
    private bool immune = false;
    private bool attackCooldown = false;
    private bool specialRangedAttackCooldown = true;
    private bool dashCooldown = false;
    private bool isDashing = false;
    private bool isPatrolling;
    private bool stopPatrol;
    private bool patrollingDirection = true;
    private bool flip;
    private bool childFlip;
    private bool rooted;
    private bool fixRotation;
    private bool matchPlayerY = false;
    private bool bossMusicTracker = true;
    private float leftPatrolBarrierPositionX;
    private float rightPatrolBarrierPositionX;
    private float previousRotationX;
    private Vector2 offset;
    private Vector2 previousPosition;
    private EnemyAI enemyAI;
    private AIPath aiPath;
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D[] detected;
    private Player player;
    private Transform firePoint;
    private GameObject arrow;
    private ParticleSystem coinBurstParticleEffect;
    private Transform particleContainer;
    private CameraShake cameraShake;
    #endregion

    #region Properties
    public GameObject BloodEffect { get => Data.bloodEffect; private set => Data.bloodEffect = value; }
    public Transform PlayerTrans { get => playerTrans; private set => playerTrans = value; }
    public EnemyData Data { get => data; private set => data = value; }
    public float EnemyLevelScale { get => lvlIndex; private set => lvlIndex = value; }
    public int FacingDirection { get => facingDirection; private set => facingDirection = value; }
    public Transform FirePoint { get => firePoint; private set => firePoint = value; }
    public Transform FirePoint2 { get; private set; }
    public EnemyAttackAI DetectAIRange { get => detectAIRange; private set => detectAIRange = value; }
    public EnemyAttackAI AttackAIRange { get => attackAIRange; private set => attackAIRange = value; }
    public EnemyAttackAI DashAIRange { get => dashAIRange; private set => dashAIRange = value; }
    public bool SpecialRangedAttackCooldown { get => specialRangedAttackCooldown; private set => specialRangedAttackCooldown = value; }
    #endregion

    #region Unity Methods
    private void Update()
    {
        if (DashAIRange && DashAIRange.InRange && Data.canDash && !dashCooldown && !attackCooldown && !Data.boss) Dash();
        else if (AttackAIRange.InRange && !Data.ranged && !Data.boss) Attack(true);
        else if (AttackAIRange.InSight && Data.ranged && !Data.boss) Attack(false);

        if (Data.lookAtPlayer && DetectAIRange.Alerted && !isDashing)
        {
            if (Data.facingDirection)
            {
                if (playerTrans.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3(1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else if (playerTrans.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
            }
            else
            {
                if (playerTrans.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else if (playerTrans.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
            }
        }

        FacingDirection = transform.localScale.x > 0 ? 1 : -1;

        if (enemyAI != null)
        {
            if (DetectAIRange.Alerted || AttackAIRange.Alerted)
            {
                enemyAI.enabled = true;
            }
            else enemyAI.enabled = false;
        }
        if (aiPath != null)
        {
            if (DetectAIRange.Alerted && !matchPlayerY)
            {
                aiPath.enabled = true;
            }
            else aiPath.enabled = false;
        }

        if (rooted)
        {
            rb.velocity = Vector3.zero;
            transform.position = new Vector2(previousPosition.x, transform.position.y);
        }

        if (DetectAIRange.Alerted)
        {
            isPatrolling = false;

            if (Data.boss && !hpBar.gameObject.activeInHierarchy)
            {
                hpBar.gameObject.SetActive(true);
            }
            if (bossMusicPlayer && bossMusicTracker)
            {
                bossMusicTracker = false;
                AudioManager.Instance.PauseMusic();
                bossMusicPlayer.Play();
            }
        }
        if (isPatrolling)
        {
            if ((transform.position.x <= leftPatrolBarrierPositionX || transform.position.x >= rightPatrolBarrierPositionX) && !stopPatrol)
            {
                rooted = true;
                stopPatrol = true;

                StartCoroutine(StopRootedCoroutine(Data.patrolPauseTime));
                StartCoroutine(StartPatrolCoroutine(Data.patrolPauseTime + 1));

                patrollingDirection = !patrollingDirection;

                if (transform.position.x >= rightPatrolBarrierPositionX && transform.localScale.x < 0) StartCoroutine(FlipCoroutine(Data.patrolPauseTime));
                else if (transform.position.x <= leftPatrolBarrierPositionX && transform.localScale.x > 0) StartCoroutine(FlipCoroutine(Data.patrolPauseTime));
            }
            if (patrollingDirection && !rooted)
            {
                rb.velocity = new Vector2(Data.patrolSpeed, 0);
            }
            else if (!rooted)
            {
                rb.velocity = new Vector2(-Data.patrolSpeed, 0);
            }
        }

        if (matchPlayerY && !aiPath.enabled)
        {
            if (playerTrans.transform.position.y > FirePoint.position.y - 0.5f && playerTrans.transform.position.y < FirePoint.position.y + 0.5f)
            {
                rooted = true;
            }
            else if(playerTrans.transform.position.y > FirePoint.position.y)
            {
                rb.velocity = new Vector2(0, 6);
            }
            else
            {
                rb.velocity = new Vector2(0, -6);
            }
        }

        if (fixRotation) transform.localScale = new Vector2(previousRotationX, transform.localScale.y);

        previousRotationX = transform.localScale.x;
        previousPosition = transform.position;
    }

    private void Start()
    {
        #region Variable Getting and Finding
        enemyAI = GetComponentInChildren<EnemyAI>();
        aiPath = GetComponentInChildren<AIPath>();
        coinBurstParticleEffect = GetComponentInChildren<ParticleSystem>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        particleContainer = GameObject.FindGameObjectWithTag("ParticleContainer").transform;
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        flip = ContainsParam(animator, "Flip");
        FirePoint = gameObject.transform.Find("FirePoint");
        FirePoint2 = gameObject.transform.Find("FirePoint2");
        arrow = gameObject.transform.Find("Arrow")?.gameObject;
        if (rightPatrolBarrier) rightPatrolBarrierPositionX = rightPatrolBarrier.transform.position.x;
        if (leftPatrolBarrier) leftPatrolBarrierPositionX = leftPatrolBarrier.transform.position.x;
        isPatrolling = Data.patrol;
        cameraShake = CameraShake.instance;
        #endregion

        #region Calculations
        lvlIndex = Data.level * 0.1f + 0.9f;

        offsetXSave = Data.offsetX;

        hp = Data.maxHP * lvlIndex;
        hpBar.maxValue = Data.maxHP * lvlIndex;

        coinsDropped = Random.Range(Data.minCoinsDropped, Data.maxCoinsDropped);
        #endregion

        TakeDamage(0, 0, false);

        StartCoroutine(SpecialRangedAttackCooldownCoroutine(Data.specialRangedAttackCooldown));
    }
    #endregion

    #region Combat

    #region Takers
    public void TakeDamage(float rawDamage, float knockback, bool multipleDamageSources)
    {
        float damage = Mathf.Round(rawDamage);

        if (immune == false)
        {
            hp -= damage;
            hpBar.value = hp;
            hpBarFill.color = hpBarGradient.Evaluate(hpBar.normalizedValue);

            if (damage > 0)
            {
                if (GetComponentInChildren<Canvas>() != null && damagePopup != null)
                {
                    var dmgNumber = Instantiate(damagePopup, transform.position + damagePopupOffset, Quaternion.identity, GetComponentInChildren<Canvas>().transform).GetComponent<TextMeshProUGUI>();
                    dmgNumber.text = damage.ToString();
                    dmgNumber.color = damagePopupGradient.Evaluate(Mathf.Clamp01(damage / 100));
                    dmgNumber.fontSize += Mathf.Round(damage / fontSizeToDamageScaler);
                }

                if(ContainsParam(animator, "Hurt")) animator.SetTrigger("Hurt");

                if(!multipleDamageSources)
                {
                    immune = true;

                    StartCoroutine(StopImmuneCoroutine(0.1f));
                }

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
    #endregion

    #region Actions
    private void Die()
    {
        var coins = Instantiate(coinBurstParticleEffect.gameObject, transform.position, Quaternion.identity, particleContainer);
        coins.GetComponent<ParticleSystem>().Emit(coinsDropped);
        Instantiate(Data.deathEffect, transform.position, Quaternion.identity);
        if (Data.itemDrop)
        {
            var random = new System.Random();
            if(random.NextDouble() <= Data.itemDropChance)
            {
                InventoryManager.Instance.Add(Data.itemDrop, true);
            }
            if (random.NextDouble() <= Data.spellDropChance)
            {
                SpellManager.Instance.Add(Data.spellDrop);
            }
        }

        Destroy(gameObject);
    }

    public void Attack(bool meleeRanged)
    {
        if (attackCooldown) return;

        if (meleeRanged)
        {
            if (ContainsParam(animator, "Attack")) animator.SetTrigger("Attack");

            attackCooldown = true;
            StartCoroutine(AttackCooldownCoroutine(Data.attackSpeed));
            StartCoroutine(AttackSpawnCoroutine(Data.attackAnimationLength));

            if (ContainsParam(animator, "idle")) animator.SetBool("idle", false);
            if (ContainsParam(animator, "attack")) animator.SetBool("attack", true);
        }
        else if (!meleeRanged && AttackAIRange.InSight)
        {
            if (ContainsParam(animator, "Attack")) animator.SetTrigger("Attack");
            StartCoroutine(AttackSpawnBossRangedCoroutine(Data.attackAnimationLength));

            attackCooldown = true;
            StartCoroutine(AttackCooldownCoroutine(Data.attackSpeed));

            if (ContainsParam(animator, "idle")) animator.SetBool("idle", false);
            if (ContainsParam(animator, "ranged")) animator.SetBool("ranged", true);

            if (rangedAttackSound) StartCoroutine(PlayRangedAttackSoundCoroutine(Data.rangedAttackSoundDelay));
        }
        if (Data.rootWhenAttacking)
        {
            StartCoroutine(StartRootedCoroutine(0.1f));
            StartCoroutine(StopRootedCoroutine(Data.attackAnimationLength + 0.1f));
        }
    }

    public void SpecialRangedAttack()
    {
        if (attackCooldown || SpecialRangedAttackCooldown) return;

        SpecialRangedAttackCooldown = true;
        attackCooldown = true;

        StartCoroutine(SpecialRangedAttackCooldownCoroutine(Data.specialRangedAttackCooldown));
        StartCoroutine(AttackCooldownCoroutine(Data.attackSpeed + Data.specialRangedAttackChargeTime));
        StartCoroutine(SpecialRangedAttackSpawnCoroutine(Data.specialRangedAttackChargeTime));

        StartCoroutine(StopRootedCoroutine(Data.specialRangedAttackChargeTime + Data.specialRangedAttackChargeExecutionTime));

        if (ContainsParam(animator, "idle")) animator.SetBool("idle", false);
        if (ContainsParam(animator, "specialRanged")) animator.SetBool("specialRanged", true);

        if (aiPath)
        {
            aiPath.enabled = false;
            StartCoroutine(EnableAIPathCoroutine(Data.specialRangedAttackChargeTime + Data.specialRangedAttackChargeExecutionTime));
            matchPlayerY = true;
            StartCoroutine(StopMatchPlayerYCoroutine(Data.specialRangedAttackChargeTime));
        }
    }

    public void Dash()
    {
        animator.SetTrigger("Dash");

        dashCooldown = true;
        StartCoroutine(DashCooldownCoroutine(Data.dashCooldown));
        attackCooldown = true;
        StartCoroutine(AttackCooldownCoroutine(Data.attackSpeed));

        isDashing = true;
        StartCoroutine(StopDashCoroutine(Data.dashDuration));

        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        if (playerTrans.position.x > transform.position.x) rb.AddForce(new Vector2(-Data.dashStrength, 0), ForceMode2D.Impulse);
        else rb.AddForce(new Vector2(Data.dashStrength, 0), ForceMode2D.Impulse);
    }

    #endregion

    #region Spawners
    private void AttackSpawn(bool bossRanged)
    {
        if (arrow && Data.ranged && FirePoint)
        {
            GameObject attackProjectile = Instantiate(arrow, FirePoint);

            if (attackProjectile.activeInHierarchy == false)
            {
                StartCoroutine(SetObjectActiveCoroutine(0.2f, attackProjectile));
            }

            return;
        }
        else if (Data.bossProjectile && bossRanged && FirePoint2)
        {
            GameObject attackProjectile = Instantiate(Data.bossProjectile, FirePoint2);

            if (attackProjectile.activeInHierarchy == false)
            {
                attackProjectile.SetActive(true);
            }

            if (ContainsParam(animator, "idle")) animator.SetBool("idle", true);
            if (ContainsParam(animator, "ranged")) animator.SetBool("ranged", false);

            return;
        }
        
        if(data.boss) cameraShake.ShakeCamera(0.5f, 1.2f);

        offset.Set(
            transform.position.x + (Data.HitBox.center.x * FacingDirection * -1),
            transform.position.y + Data.HitBox.center.y
        );

        attackSound?.Play();

        detected = Physics2D.OverlapBoxAll(offset, Data.HitBox.size, 0f, Data.DetectableLayers);

        if (ContainsParam(animator, "idle")) animator.SetBool("idle", true);
        if (ContainsParam(animator, "attack")) animator.SetBool("attack", false);

        if (detected.Length == 0) return;

        foreach (Collider2D obj in detected)
        {
            player = obj.gameObject.GetComponent<Player>();

            if (player)
            {
                player.Core.GetCoreComponent<DamageReceiver>().Damage(Data.damage * EnemyLevelScale, Data.damageType);
            }
        }
    }

    private void SpecialRangedAttackSpawn()
    {
        if (!Data.bossSpecialProjectile)
        {
            bossSpecialProjectile.SetActive(true);
        }
        else
        {
            GameObject attackProjectile = Instantiate(Data.bossSpecialProjectile, FirePoint);
            if (attackProjectile.activeInHierarchy == false)
            {
                attackProjectile.SetActive(true);
            }
        }

        fixRotation = true;

        StartCoroutine(StopFixRotationCoroutine(Data.specialRangedAttackChargeExecutionTime));

        cameraShake.ShakeCamera(Data.specialRangedAttackChargeExecutionTime, 2f);

        if (ContainsParam(animator, "idle")) animator.SetBool("idle", true);
        if (ContainsParam(animator, "specialRanged")) animator.SetBool("specialRanged", false);

        return;
    }
    #endregion

    #endregion

    #region Coroutines
    IEnumerator AttackCooldownCoroutine(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        attackCooldown = false;
    }

    IEnumerator SpecialRangedAttackCooldownCoroutine(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        SpecialRangedAttackCooldown = false;
    }

    IEnumerator DashCooldownCoroutine(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        dashCooldown = false;
    }

    IEnumerator StopDashCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        isDashing = false;
    }

    IEnumerator StopImmuneCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        immune = false;
    }

    IEnumerator StopRootedCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        rooted = false;
    }
    IEnumerator StartRootedCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        rooted = true;
    }

    IEnumerator StopFixRotationCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        fixRotation = false;
    }

    IEnumerator StartPatrolCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        isPatrolling = true;
    }

    IEnumerator StopMatchPlayerYCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        matchPlayerY = false;
    }

    IEnumerator EnableAIPathCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        aiPath.enabled = true;
    }

    IEnumerator AttackSpawnBossRangedCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        AttackSpawn(true);
    }
    IEnumerator AttackSpawnCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        AttackSpawn(false);
    }

    IEnumerator SpecialRangedAttackSpawnCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpecialRangedAttackSpawn();
    }

    IEnumerator FlipCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Flip();
    }

    IEnumerator PlayRangedAttackSoundCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        rangedAttackSound.Play();
    }

    IEnumerator SetObjectActiveCoroutine(float delay, GameObject objectToSetActive)
    {
        yield return new WaitForSeconds(delay);
        objectToSetActive.SetActive(true);
    }
    #endregion

    #region OtherMethods
    //checks if a parameter exists in the animator, found here https://discussions.unity.com/t/is-there-a-way-to-check-if-an-animatorcontroller-has-a-parameter/86194
    private void Flip() => transform.localScale = new Vector3(-1f * transform.localScale.x, transform.localScale.y, transform.localScale.z);
    public bool ContainsParam(Animator _Anim, string _ParamName)
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
        Vector3 adjustedCenter = transform.position + new Vector3(Data.HitBox.center.x * FacingDirection * -1, Data.HitBox.center.y, 0f);
        Gizmos.DrawWireCube(adjustedCenter, Data.HitBox.size);
    }
    #endregion
}
