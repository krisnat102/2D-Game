using UnityEngine;
using UnityEngine.UI;
using Bardent.CoreSystem;
using Pathfinding;
using TMPro;
using Inventory;
using Spells;
using System.Collections;
using Krisnat;
using System;

public class Enemy : MonoBehaviour
{
    #region Private Variables
    [Header("Stats")]
    [SerializeField] private int level = 1;

    [Header("Hp Bar")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private Gradient hpBarGradient;
    [SerializeField] private Image hpBarFill;
    [SerializeField] private GameObject damagePopup;
    [SerializeField] private Gradient damagePopupGradient;
    [SerializeField] private Vector3 damagePopupOffset;
    [SerializeField] private float fontSizeToDamageScaler;

    [Header("Ranges")]
    [SerializeField] private EnemyAttackAI detectAIRange;
    [SerializeField] private EnemyAttackAI attackAIRange, cancelRangedAIRange, dashAIRange;
    [SerializeField] private Transform leftPatrolBarrier, rightPatrolBarrier;

    [Header("Sound")]
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource damageSound;
    [SerializeField] private AudioSource rangedAttackSound;
    [SerializeField] private AudioSource bossMusicPlayer;

    [Header("Other")]
    [SerializeField] private EnemyData data;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject bossSpecialProjectile;
    [SerializeField] private GameObject sleepingObj;
    [SerializeField] private BoxCollider2D groundCollider;

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
    private bool sleeping;
    private bool fixRotation;
    private bool matchPlayerY = false;
    private bool bossMusicTracker = true;
    private bool attacking = false;
    private bool alerted = false;
    private float leftPatrolBarrierPositionX;
    private float rightPatrolBarrierPositionX;
    private float previousRotationX;
    private float previousLocalScaleX;
    private Vector2 offset;
    private Vector2 previousPosition;
    private Vector2 oldPlayerPosition;
    private EnemyAI enemyAI;
    private AIPath aiPath;
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D[] detected;
    private Player player;
    private Transform playerTrans;
    private Transform firePoint;
    private GameObject arrow;
    private CameraShake cameraShake;
    private CoreClass.GameManager gameManager;
    #endregion

    #region Properties
    public bool SpecialRangedAttackCooldown { get => specialRangedAttackCooldown; private set => specialRangedAttackCooldown = value; }
    public bool Dead { get; private set; }
    public int FacingDirection { get => facingDirection; private set => facingDirection = value; }
    public float EnemyLevelScale { get => lvlIndex; private set => lvlIndex = value; }
    public string BossId { get; private set; }
    public Transform PlayerTrans { get => playerTrans; private set => playerTrans = value; }
    public EnemyData Data { get => data; private set => data = value; }
    public Transform FirePoint { get => firePoint; private set => firePoint = value; }
    public Transform FirePoint2 { get; private set; }
    public EnemyAttackAI DetectAIRange { get => detectAIRange; private set => detectAIRange = value; }
    public EnemyAttackAI AttackAIRange { get => attackAIRange; private set => attackAIRange = value; }
    public EnemyAttackAI DashAIRange { get => dashAIRange; private set => dashAIRange = value; }
    public Vector2 OldPlayerPosition { get => oldPlayerPosition; private set => oldPlayerPosition = value; }
    #endregion

    #region Unity Methods
    private void Update()
    {
        if (Dead) gameObject.SetActive(false);

        if (Data.dummy) return;

        if (DashAIRange && DashAIRange.InRange && Data.canDash && !dashCooldown && !attackCooldown && !Data.boss) Dash();
        else if (AttackAIRange.InRange && !Data.ranged && !Data.boss) 
        { 
            Attack(true);
        }
        else if (AttackAIRange.InSight && Data.ranged && !Data.boss)
        {
            Attack(false);
        }

        #region Facing Direction

        if (playerTrans && Data.lookAtPlayer && DetectAIRange.Alerted && !isDashing && !(Data.commitDirectionWhenAttacking && attacking))
        {
            if (playerTrans.position.x < transform.position.x)
            {
                transform.localScale = new Vector3((Data.facingDirection ? 1 : -1) * -1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (playerTrans.position.x > transform.position.x)
            {
                transform.localScale = new Vector3((Data.facingDirection ? 1 : -1) * 1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }

        if (!attacking || !Data.commitDirectionWhenAttacking)
        {
            previousLocalScaleX = transform.localScale.x;
        }
        else
        {
            transform.localScale = new Vector3(previousLocalScaleX, transform.localScale.y, transform.localScale.z);
        }

        //FacingDirection = transform.localScale.x > 0 ? 1 : -1;
        FacingDirection = (int) Mathf.Sign(transform.localScale.x);

        #endregion

        #region AI

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
            if (ContainsParam(animator, "combat")) animator.SetBool("combat", true);
            isPatrolling = false;

            if (!hpBar.gameObject.activeInHierarchy)
            {
                hpBar.gameObject.SetActive(true);
            }
            if (bossMusicPlayer && bossMusicTracker)
            {
                bossMusicTracker = false;
                AudioManager.Instance.PauseMusic();
                bossMusicPlayer.Play();
            }
            if(ContainsParam(animator, "sleep") && ContainsParam(animator, "wakeUp") && animator.GetBool("sleep"))
            {
                animator.SetBool("wakeUp", true);
                animator.SetBool("sleep", false);
                rooted = true;
                immune = true;
                //StartCoroutine(StopRootedCoroutine(Data.wakeUpTime));
                StartCoroutine(ChangeBoolCoroutine(Data.wakeUpTime, newValue => rooted = newValue[0], new[] { false }));
                //StartCoroutine(StopImmuneCoroutine(Data.wakeUpTime));
                StartCoroutine(ChangeBoolCoroutine(Data.wakeUpTime, newValue => immune = newValue[0], new[] { false }));
                //StartCoroutine(StopSleepingCoroutine(Data.wakeUpTime));
                StartCoroutine(ChangeBoolCoroutine(Data.wakeUpTime, newValue => sleeping = newValue[0], new[] { false }));
                StartCoroutine(StartIdleCoroutine(Data.wakeUpTime - 0.2f, "wakeUp"));
            }

            if (!alerted)
            {
                //StartCoroutine(SpecialRangedAttackCooldownCoroutine(Data.specialRangedAttackCooldown));
                StartCoroutine(ChangeBoolCoroutine(Data.specialRangedAttackCooldown, newValue => specialRangedAttackCooldown = newValue[0], new[] { false }));

                alerted = true;
            }
        }
        else
        {
            if(ContainsParam(animator, "sleep") && ContainsParam(animator, "idle"))
            {
                sleeping = true;
                animator.SetBool("idle", false);
                animator.SetBool("sleep", true);
            }
        }
        #endregion

        #region Patrol
        if (isPatrolling)
        {
            if (!(enemyAI && !enemyAI.IsGrounded))
            {
                if ((transform.position.x <= leftPatrolBarrierPositionX || transform.position.x >= rightPatrolBarrierPositionX) && !stopPatrol)
                {
                    rooted = true;
                    stopPatrol = true;

                    //StartCoroutine(StopRootedCoroutine(Data.patrolPauseTime));
                    StartCoroutine(ChangeBoolCoroutine(Data.patrolPauseTime, newValue => rooted = newValue[0], new[] { false }));
                    //StartCoroutine(StartPatrolCoroutine(Data.patrolPauseTime + 1));
                    StartCoroutine(ChangeBoolCoroutine(Data.patrolPauseTime + 1, newValue => stopPatrol = newValue[0], new[] { false }));

                    patrollingDirection = !patrollingDirection;

                    if (transform.position.x >= rightPatrolBarrierPositionX && transform.localScale.x < 0)
                    {
                        StartCoroutine(FlipCoroutine(Data.patrolPauseTime));
                    }
                    else if (transform.position.x <= leftPatrolBarrierPositionX && transform.localScale.x > 0)
                    {
                        StartCoroutine(FlipCoroutine(Data.patrolPauseTime));
                    }
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
        }
        #endregion

        #region Follow Player Y Position

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

        #endregion

        #region Animations

        if (!Data.boss)
        {
            if (!attacking && ContainsParam(animator, "idle"))
            {
                if (ContainsParam(animator, "run"))
                {
                    if (rb.velocity.x != 0)
                    {
                        animator.SetBool("run", true);
                        animator.SetBool("idle", false);
                    }
                    else
                    {
                        animator.SetBool("run", false);
                        animator.SetBool("idle", true);
                    }
                }
                
                if (ContainsParam(animator, "jump"))
                {
                    if (rb.velocity.y != 0)
                    {
                        animator.SetBool("jump", true);
                        animator.SetBool("idle", false);
                    }
                    else
                    {
                        animator.SetBool("jump", false);
                        animator.SetBool("idle", true);
                    }
                }
            }

            if (animator.GetBool("attack")) {
                if (ContainsParam(animator, "jump")) animator.SetBool("jump", false);
                if (ContainsParam(animator, "run")) animator.SetBool("run", false);
                if (ContainsParam(animator, "idle")) animator.SetBool("idle", false);
                if (ContainsParam(animator, "attack")) animator.SetBool("attack", true);
            }

        }

        #endregion

        if (fixRotation) transform.localScale = new Vector2(previousRotationX, transform.localScale.y);

        previousRotationX = transform.localScale.x;
        previousPosition = transform.position;
    }

    private void Awake()
    {
        if (Data.boss) BossId = gameObject.name;
    }

    private void Start()
    {
        #region Variable Getting and Finding
        enemyAI = GetComponentInChildren<EnemyAI>();
        aiPath = GetComponentInChildren<AIPath>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        flip = ContainsParam(animator, "Flip");
        FirePoint = gameObject.transform.Find("FirePoint");
        FirePoint2 = gameObject.transform.Find("FirePoint2");
        arrow = GetComponentInChildren<RangedAttack>(true)?.gameObject;
        if (rightPatrolBarrier) rightPatrolBarrierPositionX = rightPatrolBarrier.transform.position.x;
        if (leftPatrolBarrier) leftPatrolBarrierPositionX = leftPatrolBarrier.transform.position.x;
        isPatrolling = Data.patrol;
        cameraShake = CameraShake.Instance;
        previousLocalScaleX = transform.localScale.x;
        gameManager = CoreClass.GameManager.Instance;
        playerTrans = PlayerInputHandler.Instance.gameObject.transform;
        #endregion

        if (Data.dummy) return;
        
        #region Calculations
        lvlIndex = level * 0.1f + 0.9f;

        offsetXSave = Data.offsetX;

        hp = Data.maxHP * lvlIndex;
        if(hpBar) hpBar.maxValue = Data.maxHP * lvlIndex;

        coinsDropped = UnityEngine.Random.Range(Data.minCoinsDropped, Data.maxCoinsDropped);
        #endregion
        
        TakeDamage(0, 0, false);
    }
    #endregion

    #region Combat

    #region Takers
    public void TakeDamage(float rawDamage, float knockback, bool multipleDamageSources)
    {
        float damage = Mathf.Round(rawDamage);

        if (immune == false)
        {
            if (!Data.dummy) hp -= damage;

            if (hpBar)
            {
                hpBar.value = hp;
                hpBarFill.color = hpBarGradient.Evaluate(hpBar.normalizedValue);
            }

            if (damage > 0)
            {
                if (GetComponentInChildren<Canvas>() && damagePopup && MenuManager.Instance.DamagePopUps)
                {
                    Debug.Log()
                    Vector3 popUpOffset;
                    if (!multipleDamageSources) popUpOffset = new Vector3(damagePopupOffset.x + UnityEngine.Random.Range(-2f, 1f), damagePopupOffset.y);
                    else popUpOffset = Vector3.zero;

                    var dmgNumber = Instantiate(damagePopup, transform.position + popUpOffset, Quaternion.identity, GetComponentInChildren<Canvas>().transform).GetComponent<TextMeshProUGUI>();
                    dmgNumber.text = damage.ToString();
                    dmgNumber.color = damagePopupGradient.Evaluate(Mathf.Clamp01(damage / 100));
                    dmgNumber.fontSize += Mathf.Round(damage / fontSizeToDamageScaler);
                }
                if (Data.bloodEffect)
                {
                    Instantiate(Data.bloodEffect, new Vector3(transform.position.x + UnityEngine.Random.Range(-1,1)/5 + Data.bloodOffset.x, transform.position.y + UnityEngine.Random.Range(-1, 1)/5 + Data.bloodOffset.y, transform.position.z), Quaternion.identity);
                }

                if (hp <= 0 && !Data.dummy)
                {
                    Die();
                    return;
                }
                else if (damageSound)
                {
                    damageSound.pitch = UnityEngine.Random.Range(Data.pitchVarianceDamage[0], Data.pitchVarianceDamage[1]);
                    damageSound.Play();
                }

                if (ContainsParam(animator, "hurt")) animator.SetBool("hurt", true);
                if (ContainsParam(animator, "idle")) animator.SetBool("idle", false);
                StartCoroutine(StartIdleCoroutine(0.25f, "hurt"));

                if (!multipleDamageSources)
                {
                    immune = true;

                    //StartCoroutine(StopImmuneCoroutine(0.1f));
                    StartCoroutine(ChangeBoolCoroutine(0.1f, newValue => immune = newValue[0], new[] { false }));
                }

                TakeKnockback(damage + knockback);

                if(DetectAIRange) DetectAIRange.Alerted = true;
            }
        }
    }
    private void TakeKnockback(float damage)
    {
        if (Data.dummy) return;

        float knockback = damage * Data.knockbackModifier / Mathf.Sqrt(level);

        if (playerTrans.position.x < transform.position.x)
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
        if (Dead) return;
        if (Data.boss)
        {
            gameManager.DeactivateObject(4, hpBar.gameObject);

            gameManager.BossesKilled.Add(BossId);

            AudioManager.Instance.UnPauseMusic();
            bossMusicPlayer.Stop();
        }

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

        if(Data.maxCoinsDropped > 0)
        {
            var coins = GetComponentInChildren<CoinPickup>()?.gameObject;
            if(coins) coins.transform.parent = null;
            coins?.GetComponent<ParticleSystem>().Emit(coinsDropped);
        }

        if (deathEffect)
        {
            deathEffect.SetActive(true);
                deathEffect.transform.parent = null;
            if (deathEffect.GetComponent<Death>().AdaptSize) deathEffect.transform.localScale = transform.localScale;
            if (deathEffect.GetComponent<Death>().AdaptDirection) deathEffect.transform.localScale = new Vector2(deathEffect.transform.localScale.y * FacingDirection, deathEffect.transform.localScale.y);
        }

        Dead = true;

        gameObject.SetActive(false);
    }

    public void Attack(bool meleeRanged)
    {
        if (attackCooldown || sleeping) return;

        attacking = true;

        AttackAnimationHelper();

        if (Data.fixRotationWhenAttacking) fixRotation = true;

        attackCooldown = true;
        //StartCoroutine(AttackCooldownCoroutine(Data.attackSpeed));
        StartCoroutine(ChangeBoolCoroutine(Data.attackSpeed, newValue => attackCooldown = newValue[0], new[] { false }));

        if (ContainsParam(animator, "idle")) animator.SetBool("idle", false);

        StartCoroutine(AttackSpawnCoroutine(Data.damageTriggerTime, meleeRanged));


        if (ContainsParam(animator, "attack")) animator.SetBool("attack", true);

        if (!meleeRanged && (DetectAIRange.InSight || AttackAIRange.InSight))
        {
            OldPlayerPosition = playerTrans.position;
            StartCoroutine(AimDelayCoroutine(Data.aimDelay));
            if (ContainsParam(animator, "ranged")) animator.SetBool("ranged", true);

            if (rangedAttackSound) StartCoroutine(PlayRangedAttackSoundCoroutine(Data.rangedAttackSoundDelay));
        }

        if (Data.rootWhenAttacking)
        {
            //StartCoroutine(StartRootedCoroutine(0.1f));
            StartCoroutine(ChangeBoolCoroutine(0.1f, newValue => rooted = newValue[0], new[] { true }));
            //StartCoroutine(StopRootedCoroutine(Data.attackAnimationLength + 0.1f));
            StartCoroutine(ChangeBoolCoroutine(Data.attackAnimationLength + 0.1f, newValue => rooted = newValue[0], new[] { false }));
        }

        if (Data.moveWhenAttacking)
        {
            offset.Set(
            transform.position.x + (Data.cancelMove.center.x * FacingDirection * -1),
            transform.position.y + Data.HitBox.center.y
            );

            detected = Physics2D.OverlapBoxAll(offset, Data.HitBox.size, 0f, Data.DetectableLayers);

            foreach (Collider2D obj in detected)
            {
                player = obj.gameObject.GetComponent<Player>();

                if (player)
                {
                    return;
                }
            }
            StartCoroutine(ChangeBoolCoroutine(Data.movementDelay, newValue => rooted = newValue[0], new[] { false }));
            StartCoroutine(MovementCoroutine(Data.movementDelay, -Data.direction * new Vector2(FacingDirection * -1, 0), Data.velocity));
        }
    }

    public void SpecialRangedAttack()
    {
        if (attackCooldown || SpecialRangedAttackCooldown || sleeping) return;

        if (groundCollider) groundCollider.isTrigger = true;

        SpecialRangedAttackCooldown = true;
        attackCooldown = true;

        //StartCoroutine(SpecialRangedAttackCooldownCoroutine(Data.specialRangedAttackCooldown));
        StartCoroutine(ChangeBoolCoroutine(Data.specialRangedAttackCooldown, newValue => specialRangedAttackCooldown = newValue[0], new[] { false }));
        //StartCoroutine(AttackCooldownCoroutine(Data.attackSpeed + Data.specialRangedAttackChargeTime));
        StartCoroutine(ChangeBoolCoroutine(Data.attackSpeed + Data.specialRangedAttackChargeTime, newValue => attackCooldown = newValue[0], new[] { false }));
        StartCoroutine(SpecialRangedAttackSpawnCoroutine(Data.specialRangedAttackChargeTime));

        //StartCoroutine(StopRootedCoroutine(Data.specialRangedAttackChargeTime + Data.specialRangedAttackChargeExecutionTime));
        StartCoroutine(ChangeBoolCoroutine(Data.specialRangedAttackChargeTime + Data.specialRangedAttackChargeExecutionTime, newValue => rooted = newValue[0], new[] { false }));

        if (ContainsParam(animator, "idle")) animator.SetBool("idle", false);
        if (ContainsParam(animator, "specialRanged")) animator.SetBool("specialRanged", true);

        if (aiPath)
        {
            aiPath.enabled = false;
            //StartCoroutine(EnableAIPathCoroutine(Data.specialRangedAttackChargeTime + Data.specialRangedAttackChargeExecutionTime));
            StartCoroutine(ChangeBoolCoroutine(Data.specialRangedAttackChargeTime + Data.specialRangedAttackChargeExecutionTime, newValue => aiPath.enabled = newValue[0], new[] { true }));
            matchPlayerY = true;
            //StartCoroutine(StopMatchPlayerYCoroutine(Data.specialRangedAttackChargeTime));
            StartCoroutine(ChangeBoolCoroutine(Data.specialRangedAttackChargeTime, newValue => matchPlayerY = newValue[0], new[] { false }));
        }
    }

    public void Dash()
    {
        if (dashCooldown || sleeping) return;

        if (ContainsParam(animator, "dash")) animator.SetBool("dash", true);
        StartCoroutine(StartIdleCoroutine(Data.dashDuration, "dash"));

        dashCooldown = true;
        //StartCoroutine(DashCooldownCoroutine(Data.dashCooldown));
        StartCoroutine(ChangeBoolCoroutine(Data.dashCooldown, newValue => dashCooldown = newValue[0], new[] { false }));

        attackCooldown = true;
        //StartCoroutine(AttackCooldownCoroutine(Data.attackSpeed));
        StartCoroutine(ChangeBoolCoroutine(Data.attackSpeed, newValue => attackCooldown = newValue[0], new[] { false }));

        isDashing = true;
        //StartCoroutine(StopDashCoroutine(Data.dashDuration));
        StartCoroutine(ChangeBoolCoroutine(Data.dashDuration, newValue => isDashing = newValue[0], new[] { false }));
        StartCoroutine(StopMomentumCoroutine(Data.dashDuration));

        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        if (playerTrans.position.x > transform.position.x) rb.AddForce(new Vector2(-Data.dashStrength, 0), ForceMode2D.Impulse);
        else rb.AddForce(new Vector2(Data.dashStrength, 0), ForceMode2D.Impulse);
    }

    #endregion
    
    #region Spawners
    private void AttackSpawn(bool bossMelee)
    {
        if (arrow && Data.ranged && FirePoint)
        {
            GameObject attackProjectile = Instantiate(arrow, FirePoint);

            if (attackProjectile.activeInHierarchy == false)
            {
                attackProjectile.SetActive(true);
            }
        }
        else if (Data.bossProjectile && !bossMelee && FirePoint2)
        {
            attacking = false;

            if (cancelRangedAIRange.InRange)
            {
                if (ContainsParam(animator, "ranged")) animator.SetBool("ranged", false);

                if (Data.fixRotationWhenAttacking) fixRotation = false;

                return;
            } 

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

        int calibrate = 1;
        if (FacingDirection == 1 && Data.fixRotationWhenAttacking)
        {
            calibrate = -1;
        }
        else if(Data.fixRotationWhenAttacking)
        {
            calibrate = 1;
        }

        offset.Set(
            transform.position.x + (Data.HitBox.center.x * FacingDirection * calibrate * - 1),
            transform.position.y + Data.HitBox.center.y
        );
        

        detected = Physics2D.OverlapBoxAll(offset, Data.HitBox.size, 0f, Data.DetectableLayers);

        AttackHelper();

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

        //StartCoroutine(StopFixRotationCoroutine(Data.specialRangedAttackChargeExecutionTime));
        StartCoroutine(ChangeBoolCoroutine(Data.specialRangedAttackChargeExecutionTime, newValue => fixRotation = newValue[0], new[] { false }));
        StartCoroutine(ChangeBoolCoroutine(Data.specialRangedAttackChargeExecutionTime, newValue => groundCollider.isTrigger = newValue[0], new[] { false }));

        cameraShake.ShakeCamera(Data.specialRangedAttackChargeExecutionTime, 2f);

        if (ContainsParam(animator, "idle")) animator.SetBool("idle", true);
        if (ContainsParam(animator, "specialRanged")) animator.SetBool("specialRanged", false);

        return;
    }
    #endregion

    #endregion

    #region Coroutines
    IEnumerator ChangeBoolCoroutine(float time, Action<bool[]> boolSetter, bool[] newValue)
    {
        yield return new WaitForSeconds(time);
        boolSetter(newValue);
    }
    IEnumerator AttackSpawnCoroutine(float delay, bool type)
    {
        yield return new WaitForSeconds(delay);
        AttackSpawn(type);
    }

    IEnumerator AimDelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        OldPlayerPosition = playerTrans.position;
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

    IEnumerator WakeUpCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        sleeping = false;
        sleepingObj.SetActive(true);
    }

    IEnumerator PlayRangedAttackSoundCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        rangedAttackSound.pitch = UnityEngine.Random.Range(Data.pitchVarianceRangedAttack[0], Data.pitchVarianceRangedAttack[1]);
        rangedAttackSound.Play();
    }

    IEnumerator StartIdleCoroutine(float delay, string boolToDisable)
    {
        yield return new WaitForSeconds(delay);
        if(ContainsParam(animator, "idle") && ContainsParam(animator, boolToDisable))
        {
            animator.SetBool(boolToDisable, false);
            animator.SetBool("idle", true);
        }
    }

    IEnumerator MovementCoroutine(float delay, Vector2 direction, float velocity)
    {
        yield return new WaitForSeconds(delay);
        rb.AddForce(direction * velocity);
    }

    IEnumerator StopMomentumCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.velocity = Vector2.zero;
    }
    #endregion

    #region Helper Methods
    private void AttackHelper()
    {
        if (attackSound)
        {
            attackSound.pitch = UnityEngine.Random.Range(Data.pitchVarianceAttack[0], Data.pitchVarianceAttack[1]);
            attackSound.Play();
        }

        if (Data.fixRotationWhenAttacking) fixRotation = false;
    }

    private void AttackAnimationHelper()
    {
        StartCoroutine(StartIdleCoroutine(Data.attackAnimationLength, "attack"));

        StartCoroutine(ChangeBoolCoroutine(Data.attackAnimationLength, newValue => attacking = newValue[0], new[] { false }));
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

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        if (Data.dummy) return;

        Gizmos.color = UnityEngine.Color.red;
        offset.Set(
        transform.position.x + (Data.HitBox.center.x * FacingDirection * -1),
        transform.position.y + Data.HitBox.center.y
        );
        Vector2 cancelMove = new Vector2(
        transform.position.x + (Data.cancelMove.center.x * FacingDirection * -1),
        transform.position.y + Data.cancelMove.center.y
        );

        //Vector3 adjustedCenter = transform.position + new Vector3(Data.HitBox.center.x * FacingDirection * -1, Data.HitBox.center.y, 0f);
        //Gizmos.DrawWireCube(adjustedCenter, Data.HitBox.size);
        Gizmos.DrawWireCube(offset, Data.HitBox.size);
        Gizmos.DrawWireCube(cancelMove, Data.cancelMove.size);
    }
    #endregion
}
