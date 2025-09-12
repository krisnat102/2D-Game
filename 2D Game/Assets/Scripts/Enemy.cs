using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Bardent.CoreSystem;
using Inventory;
using Pathfinding;
using Spells;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Krisnat
{
    public class Enemy : MonoBehaviour
    {
        #region Private Variables

        #region Serialized Variables
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
        [SerializeField] private List<AudioSource> attackSounds;
        [SerializeField] private List<AudioSource> damageSounds;

        [SerializeField] private List<AudioSource> rangedAttackSounds;

        [SerializeField] private AudioSource bossMusicPlayer;

        [Header("Other")]
        [SerializeField] private EnemyData data;
        [SerializeField] private GameObject deathEffect;
        [SerializeField] private GameObject bossSpecialProjectile;
        [SerializeField] private GameObject sleepingObj;
        [SerializeField] private BoxCollider2D groundCollider;
        #endregion

        #region Other Variables
        private int coinsDropped;
        private float damage;
        private float offsetXSave;
        private float hp;
        private bool immune;
        private bool dashCooldown;
        private bool bloodCooldown;
        private bool isDashing;
        private bool isPatrolling;
        private bool stopPatrol;
        private bool patrollingDirection = true;
        private bool rooted;
        private bool fixRotation;
        private bool matchPlayerY;
        private bool bossMusicTracker = true;
        private bool attacking;
        private bool alerted;
        private float leftPatrolBarrierPositionX;
        private float rightPatrolBarrierPositionX;
        private float previousRotationX;
        private float previousLocalScaleX;
        private Vector2 offset;
        private Vector2 previousPosition;
        private EnemyAI enemyAI;
        private AIPath aiPath;
        private Rigidbody2D rb;
        private Animator animator;
        private Collider2D[] detected;
        private Player player;
        private DamageReceiver damageReceiver;
        private Transform canvas;
        private GameObject arrow;
        private CameraShake cameraShake;
        private CoreClass.GameManager gameManager;
        #endregion

        #region Animator Variables
        private static readonly int Idle = Animator.StringToHash("idle");
        private static readonly int Sleep = Animator.StringToHash("sleep");
        private static readonly int Up = Animator.StringToHash("wakeUp");
        private static readonly int WakeUpSpeed = Animator.StringToHash("wakeUpSpeed");
        private static readonly int SpecialRanged = Animator.StringToHash("specialRanged");
        private static readonly int Ranged = Animator.StringToHash("ranged");
        private static readonly int Combat = Animator.StringToHash("combat");
        private static readonly int Run = Animator.StringToHash("run");
        private static readonly int Jump = Animator.StringToHash("jump");
        private static readonly int Attack1 = Animator.StringToHash("attack");
        private static readonly int Hurt = Animator.StringToHash("hurt");
        private static readonly int Dash1 = Animator.StringToHash("dash");
        #endregion

        #endregion

        #region Properties
        public bool ActionCooldown { get; private set; }
        public bool SpecialRangedAttackCooldown { get; private set; } = true;
        public bool Dead { get; private set; }
        public int FacingDirection { get; private set; } = 1;
        public float EnemyLevelScale { get; private set; }

        public string BossId { get; private set; }
        private Transform PlayerTrans { get; set; }

        public EnemyData Data => data;
        public Transform FirePoint { get; private set; }

        private Transform FirePoint2 { get; set; }
        public EnemyAttackAI DetectAIRange => detectAIRange;
        public EnemyAttackAI AttackAIRange => attackAIRange;
        private EnemyAttackAI DashAIRange => dashAIRange;
        public Vector2 OldPlayerPosition { get; private set; }

        public bool AttackCooldown { get; private set; }
        public bool Sleeping { get; private set; }

        #endregion

        #region Unity Methods
        private void Update()
        {
            if (Dead) gameObject.SetActive(false);

            if (Data.dummy) return;

            if (DashAIRange && DashAIRange.InRange && Data.canDash && !dashCooldown && !AttackCooldown && !Data.boss) Dash();
            else if (AttackAIRange.InRange && !Data.ranged && !Data.boss)
            {
                Attack(true);
            }
            else if (AttackAIRange.InSight && Data.ranged && !Data.boss)
            {
                Attack(false);
            }

            #region Facing Direction

            if (PlayerTrans && Data.lookAtPlayer && DetectAIRange.Alerted && !isDashing && !(Data.commitDirectionWhenAttacking && attacking))
            {
                if (PlayerTrans.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3((Data.facingDirection ? 1 : -1) * -1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else if (PlayerTrans.position.x > transform.position.x)
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
            FacingDirection = (int)Mathf.Sign(transform.localScale.x);

            #endregion

            #region AI

            if (enemyAI)
            {
                if (DetectAIRange.Alerted || AttackAIRange.Alerted)
                {
                    enemyAI.enabled = true;
                }
                else enemyAI.enabled = false;
            }
            if (aiPath)
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
                if (ContainsParam(animator, "combat")) animator.SetBool(Combat, true);
                isPatrolling = false;

                if (!hpBar.gameObject.activeInHierarchy)
                {
                    StartCoroutine(HpBarDelayCoroutine(Data.hpBarDelay));
                }
                if (bossMusicPlayer && bossMusicTracker)
                {
                    bossMusicTracker = false;
                    AudioManager.instance.PauseMusic();
                    bossMusicPlayer.Play();
                }
                if (!alerted)
                {
                    //StartCoroutine(SpecialRangedAttackCooldownCoroutine(Data.specialRangedAttackCooldown));
                    StartCoroutine(ChangeBoolCoroutine(Data.specialRangedAttackCooldown, newValue => SpecialRangedAttackCooldown = newValue[0], new[] { false }));

                    alerted = true;
                }
                if (alerted && ContainsParam(animator, "sleep") && animator.GetBool(Sleep))
                {
                    WakeUp();
                }
            }

            if (!alerted && ContainsParam(animator, "sleep"))
            {
                FallAsleep();
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
                if (PlayerTrans.transform.position.y > FirePoint.position.y - 0.5f && PlayerTrans.transform.position.y < FirePoint.position.y + 0.5f)
                {
                    rooted = true;
                }
                else if (PlayerTrans.transform.position.y > FirePoint.position.y)
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
                            animator.SetBool(Run, true);
                            animator.SetBool(Idle, false);
                        }
                        else
                        {
                            animator.SetBool(Run, false);
                            animator.SetBool(Idle, true);
                        }
                    }

                    if (ContainsParam(animator, "jump"))
                    {
                        if (rb.velocity.y != 0)
                        {
                            animator.SetBool(Jump, true);
                            animator.SetBool(Idle, false);
                        }
                        else
                        {
                            animator.SetBool(Jump, false);
                            animator.SetBool(Idle, true);
                        }
                    }
                }

                if (animator.GetBool(Attack1))
                {
                    if (ContainsParam(animator, "jump")) animator.SetBool(Jump, false);
                    if (ContainsParam(animator, "run")) animator.SetBool(Run, false);
                    if (ContainsParam(animator, "idle")) animator.SetBool(Idle, false);
                    if (ContainsParam(animator, "attack")) animator.SetBool(Attack1, true);
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
            FirePoint = gameObject.transform.Find("FirePoint");
            FirePoint2 = gameObject.transform.Find("FirePoint2");
            arrow = GetComponentInChildren<RangedAttack>(true)?.gameObject;
            if (rightPatrolBarrier) rightPatrolBarrierPositionX = rightPatrolBarrier.transform.position.x;
            if (leftPatrolBarrier) leftPatrolBarrierPositionX = leftPatrolBarrier.transform.position.x;
            isPatrolling = Data.patrol;
            cameraShake = CameraShake.instance;
            previousLocalScaleX = transform.localScale.x;
            gameManager = CoreClass.GameManager.instance;
            PlayerTrans = PlayerInputHandler.Instance.gameObject.transform;
            canvas = GetComponentInChildren<Canvas>().transform;
            player = Player.instance;
            damageReceiver = player.Core.GetCoreComponent<DamageReceiver>();
            #endregion

            if (Data.dummy) return;

            #region Calculations
            EnemyLevelScale = level * 0.1f + 0.9f;

            Sleeping = Data.wakeUpTime > 0;

            hp = UnityEngine.Random.Range(Data.minHp, Data.maxHp) * EnemyLevelScale;
            if (hpBar) hpBar.maxValue = hp;

            coinsDropped = UnityEngine.Random.Range(Data.minCoinsDropped, Data.maxCoinsDropped);

            damage = UnityEngine.Random.Range(Data.minDamage, Data.maxDamage);

            SpecialRangedAttackCooldown = true;
            StartCoroutine(ChangeBoolCoroutine(Data.specialRangedAttackCooldown, newValue => SpecialRangedAttackCooldown = newValue[0], new[] { false }));
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
                    if (canvas)
                    {
                        var popUpOffset = !multipleDamageSources ? new Vector3(damagePopupOffset.x + UnityEngine.Random.Range(-2f, 1f), damagePopupOffset.y) : Vector3.zero;

                        var dmgNumber = Instantiate(damagePopup, transform.position + popUpOffset, Quaternion.identity, canvas).GetComponent<TextMeshProUGUI>();
                        dmgNumber.text = damage.ToString(CultureInfo.CurrentCulture);
                        dmgNumber.color = damagePopupGradient.Evaluate(Mathf.Clamp01(damage / 100));
                        dmgNumber.fontSize += Mathf.Round(damage / fontSizeToDamageScaler);

                        if (data.stagger > 1) rb.velocity = new Vector2(rb.velocity.x / data.stagger, rb.velocity.y);

                        if (hp <= 0 && !Data.dummy)
                        {
                            dmgNumber.transform.localScale = new Vector2(Mathf.Abs(dmgNumber.transform.localScale.x), dmgNumber.transform.localScale.y);
                            hpBar?.gameObject.SetActive(false);
                            canvas.transform.SetParent(gameManager.UIs);
                        }
                    }
                    if (Data.bloodEffect && !bloodCooldown)
                    {
                        bloodCooldown = true;
                        StartCoroutine(ChangeBoolCoroutine(0.2f, newValue => bloodCooldown = newValue[0], new[] { false }));

                        Instantiate(Data.bloodEffect,
                            new Vector3(transform.position.x + UnityEngine.Random.Range(-1, 1) / 5 + Data.bloodOffset.x,
                                transform.position.y + UnityEngine.Random.Range(-1, 1) / 5 + Data.bloodOffset.y,
                                transform.position.z), Quaternion.identity);
                    }

                    if (hp <= 0 && !Data.dummy)
                    {
                        Die();
                        return;
                    }
                    else if (damageSounds.Count > 0)
                    {
                        int n = UnityEngine.Random.Range(0, damageSounds.Count);
                        damageSounds[n].pitch = UnityEngine.Random.Range(Data.pitchVarianceDamage[0], Data.pitchVarianceDamage[1]);
                        damageSounds[n].Play();
                    }

                    if (ContainsParam(animator, "hurt")) animator.SetBool(Hurt, true);
                    if (ContainsParam(animator, "idle")) animator.SetBool(Idle, false);
                    StartCoroutine(StartIdleCoroutine(0.25f, "hurt"));

                    if (!multipleDamageSources)
                    {
                        immune = true;

                        //StartCoroutine(StopImmuneCoroutine(0.1f));
                        StartCoroutine(ChangeBoolCoroutine(0.1f, newValue => immune = newValue[0], new[] { false }));
                    }

                    TakeKnockback(damage + knockback);

                    if (DetectAIRange) DetectAIRange.Alerted = true;
                }
            }
        }

        private void TakeKnockback(float damage)
        {
            if (Data.dummy) return;

            float knockback = damage * Data.knockbackModifier / Mathf.Sqrt(level);

            if (PlayerTrans.position.x < transform.position.x)
            {
                rb.AddForce(transform.up * knockback / data.knockbackWeight, ForceMode2D.Force);
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

                AudioManager.instance.UnPauseMusic();
                bossMusicPlayer.Stop();
            }

            if (Data.itemDrop)
            {
                var random = new System.Random();
                if (random.NextDouble() <= Data.itemDropChance)
                {
                    InventoryManager.Instance.Add(Data.itemDrop, true);
                }
                if (random.NextDouble() <= Data.spellDropChance)
                {
                    SpellManager.Instance.Add(Data.spellDrop);
                }
            }

            if (Data.maxCoinsDropped > 0)
            {
                var coins = GetComponentInChildren<CoinPickup>()?.gameObject;
                if (coins) coins.transform.parent = null;
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

        // ReSharper disable Unity.PerformanceAnalysis
        public void Attack(bool meleeRanged)
        {
            if (AttackCooldown || Sleeping || ActionCooldown) return;

            attacking = true;

            AttackAnimationHelper();

            if (Data.fixRotationWhenAttacking) fixRotation = true;

            AttackCooldown = true;
            StartCoroutine(ChangeBoolCoroutine(Data.attackSpeed, newValue => AttackCooldown = newValue[0], new[] { false }));

            ActionCooldown = true;
            StartCoroutine(ChangeBoolCoroutine(Data.attackSpeed + Data.specialRangedAttackChargeTime, newValue => ActionCooldown = newValue[0], new[] { false }));

            if (ContainsParam(animator, "idle")) animator.SetBool(Idle, false);

            StartCoroutine(AttackSpawnCoroutine(Data.damageTriggerTime, meleeRanged));


            if (ContainsParam(animator, "attack")) animator.SetBool(Attack1, true);

            if (!meleeRanged && (DetectAIRange.InSight || AttackAIRange.InSight))
            {
                OldPlayerPosition = PlayerTrans.position;
                StartCoroutine(AimDelayCoroutine(Data.aimDelay));
                if (ContainsParam(animator, "ranged")) animator.SetBool(Ranged, true);

                if (rangedAttackSounds.Count > 0) StartCoroutine(PlayRangedAttackSoundCoroutine(Data.rangedAttackSoundDelay));
            }

            if (Data.rootWhenAttacking)
            {
                StartCoroutine(ChangeBoolCoroutine(0.1f, newValue => rooted = newValue[0], new[] { true }));
                StartCoroutine(ChangeBoolCoroutine(Data.attackAnimationLength + 0.1f, newValue => rooted = newValue[0], new[] { false }));
            }

            if (Data.moveWhenAttacking)
            {
                offset.Set(
                    transform.position.x + (Data.cancelMove.center.x * FacingDirection * -1),
                    transform.position.y + Data.hitBox.center.y
                );

                detected = Physics2D.OverlapBoxAll(offset, Data.hitBox.size, 0f, Data.detectableLayers);

                foreach (Collider2D obj in detected)
                {
                    bool playerDetected = obj.transform.CompareTag("Player");

                    if (playerDetected)
                    {
                        return;
                    }
                }

                StartCoroutine(ChangeBoolCoroutine(Data.movementDelay, newValue => rooted = newValue[0], new[] { false }));
                StartCoroutine(MovementCoroutine(Data.movementDelay, -Data.direction * new Vector2(FacingDirection * -1, 0), Data.velocity));
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void SpecialRangedAttack()
        {
            if (AttackCooldown || SpecialRangedAttackCooldown || Sleeping || ActionCooldown) return;

            if (groundCollider) groundCollider.isTrigger = true;

            ActionCooldown = true;
            StartCoroutine(ChangeBoolCoroutine(Data.attackSpeed + Data.specialRangedAttackChargeTime, newValue => ActionCooldown = newValue[0], new[] { false }));

            SpecialRangedAttackCooldown = true;
            StartCoroutine(ChangeBoolCoroutine(Data.specialRangedAttackCooldown, newValue => SpecialRangedAttackCooldown = newValue[0], new[] { false }));

            AttackCooldown = true;
            StartCoroutine(ChangeBoolCoroutine(Data.actionCooldown, newValue => AttackCooldown = newValue[0], new[] { false }));

            StartCoroutine(SpecialRangedAttackSpawnCoroutine(Data.specialRangedAttackChargeTime));

            StartCoroutine(ChangeBoolCoroutine(Data.specialRangedAttackChargeTime + Data.specialRangedAttackChargeExecutionTime, newValue => rooted = newValue[0], new[] { false }));

            if (ContainsParam(animator, "idle")) animator.SetBool(Idle, false);
            if (ContainsParam(animator, "specialRanged")) animator.SetBool(SpecialRanged, true);

            if (aiPath)
            {
                aiPath.enabled = false;
                StartCoroutine(ChangeBoolCoroutine(Data.specialRangedAttackChargeTime + Data.specialRangedAttackChargeExecutionTime, newValue => aiPath.enabled = newValue[0], new[] { true }));

                matchPlayerY = true;
                StartCoroutine(ChangeBoolCoroutine(Data.specialRangedAttackChargeTime, newValue => matchPlayerY = newValue[0], new[] { false }));
            }
        }

        public void Dash()
        {
            if (dashCooldown || Sleeping || ActionCooldown) return;

            if (ContainsParam(animator, "dash")) animator.SetBool(Dash1, true);
            StartCoroutine(StartIdleCoroutine(Data.dashDuration, "dash"));

            dashCooldown = true;
            StartCoroutine(ChangeBoolCoroutine(Data.dashCooldown, newValue => dashCooldown = newValue[0], new[] { false }));

            ActionCooldown = true;
            StartCoroutine(ChangeBoolCoroutine(Data.actionCooldown, newValue => ActionCooldown = newValue[0], new[] { false }));

            isDashing = true;
            StartCoroutine(ChangeBoolCoroutine(Data.dashDuration, newValue => isDashing = newValue[0], new[] { false }));
            StartCoroutine(StopMomentumCoroutine(Data.dashDuration));

            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            rb.AddForce(
                PlayerTrans.position.x > transform.position.x
                    ? new Vector2(-Data.dashStrength, 0)
                    : new Vector2(Data.dashStrength, 0), ForceMode2D.Impulse);
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
                    if (ContainsParam(animator, "ranged")) animator.SetBool(Ranged, false);

                    if (Data.fixRotationWhenAttacking) fixRotation = false;

                    return;
                }

                GameObject attackProjectile = Instantiate(Data.bossProjectile, FirePoint2);

                if (attackProjectile.activeInHierarchy == false)
                {
                    attackProjectile.SetActive(true);
                }

                if (ContainsParam(animator, "idle")) animator.SetBool(Idle, true);
                if (ContainsParam(animator, "ranged")) animator.SetBool(Ranged, false);

                return;
            }

            if (data.boss) cameraShake.ShakeCamera(0.5f, 1.2f);

            int calibrate = 1;
            if (FacingDirection == 1 && Data.fixRotationWhenAttacking)
            {
                calibrate = -1;
            }
            else if (Data.fixRotationWhenAttacking)
            {
                calibrate = 1;
            }

            offset.Set(
                transform.position.x + (Data.hitBox.center.x * FacingDirection * calibrate * -1),
                transform.position.y + Data.hitBox.center.y
            );

            detected = Physics2D.OverlapBoxAll(offset, Data.hitBox.size, 0f, Data.detectableLayers);

            AttackHelper();

            if (detected.Length == 0) return;

            foreach (Collider2D obj in detected)
            {
                bool playerDetected = obj.transform.CompareTag("Player");

                if (playerDetected)
                {
                    damageReceiver.Damage(damage * EnemyLevelScale, Data.damageType);
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

            if (ContainsParam(animator, "idle")) animator.SetBool(Idle, true);
            if (ContainsParam(animator, "specialRanged")) animator.SetBool(SpecialRanged, false);
        }
        #endregion

        #endregion

        #region General Methods
        public void WakeUp()
        {
            if (ContainsParam(animator, "wakeUp")) animator.SetBool(Up, true);
            if (ContainsParam(animator, "wakeUpSpeed")) animator.SetFloat(WakeUpSpeed, UnityEngine.Random.Range(0.8f, 1.2f));
            animator.SetBool(Sleep, false);

            rooted = true;
            StartCoroutine(ChangeBoolCoroutine(Data.wakeUpTime, newValue => rooted = newValue[0], new[] { false }));

            immune = true;
            StartCoroutine(ChangeBoolCoroutine(Data.wakeUpTime, newValue => immune = newValue[0], new[] { false }));

            fixRotation = true;
            StartCoroutine(ChangeBoolCoroutine(Data.wakeUpTime, newValue => fixRotation = newValue[0], new[] { false }));

            StartCoroutine(ChangeBoolCoroutine(Data.wakeUpTime, newValue => Sleeping = newValue[0], new[] { false }));
            StartCoroutine(StartIdleCoroutine(Data.wakeUpTime - 0.2f, "wakeUp"));
        }

        private void FallAsleep()
        {
            Sleeping = true;
            rooted = true;
            animator.SetBool(Idle, false);
            if (ContainsParam(animator, "sleep")) animator.SetBool(Sleep, true);
        }
        #endregion

        #region Coroutines
        private IEnumerator ChangeBoolCoroutine(float time, Action<bool[]> boolSetter, bool[] newValue)
        {
            yield return new WaitForSeconds(time);
            boolSetter(newValue);
        }
        private IEnumerator AttackSpawnCoroutine(float delay, bool type)
        {
            yield return new WaitForSeconds(delay);
            AttackSpawn(type);
        }

        private IEnumerator AimDelayCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            OldPlayerPosition = PlayerTrans.position;
        }
        private IEnumerator HpBarDelayCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            hpBar.gameObject.SetActive(true);
        }

        private IEnumerator SpecialRangedAttackSpawnCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            SpecialRangedAttackSpawn();
        }

        private IEnumerator FlipCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            Flip();
        }

        private IEnumerator PlayRangedAttackSoundCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            int n = UnityEngine.Random.Range(0, damageSounds.Count);
            damageSounds[n].pitch = UnityEngine.Random.Range(Data.pitchVarianceDamage[0], Data.pitchVarianceDamage[1]);
            damageSounds[n].Play();
        }

        private IEnumerator StartIdleCoroutine(float delay, string boolToDisable)
        {
            yield return new WaitForSeconds(delay);
            if (ContainsParam(animator, "idle") && ContainsParam(animator, boolToDisable))
            {
                animator.SetBool(boolToDisable, false);
                animator.SetBool(Idle, true);
            }
        }

        private IEnumerator MovementCoroutine(float delay, Vector2 direction, float velocity)
        {
            yield return new WaitForSeconds(delay);
            rb.AddForce(direction * velocity);
        }

        private IEnumerator StopMomentumCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            rb.velocity = Vector2.zero;
        }
        #endregion

        #region Helper Methods
        private void AttackHelper()
        {
            if (attackSounds.Count > 0)
            {
                int n = UnityEngine.Random.Range(0, attackSounds.Count);
                attackSounds[n].pitch = UnityEngine.Random.Range(Data.pitchVarianceAttack[0], Data.pitchVarianceAttack[1]);
                attackSounds[n].Play();
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
        public bool ContainsParam(Animator anim, string paramName)
        {
            if (!anim) return false;

            foreach (AnimatorControllerParameter param in anim.parameters)
            {
                if (param.name == paramName) return true;
            }
            return false;
        }
        #endregion

        #region Gizmos
        private void OnDrawGizmosSelected()
        {
            if (Data.dummy) return;

            int calibrate = 1;
            if (FacingDirection == 1 && Data.fixRotationWhenAttacking)
            {
                calibrate = -1;
            }
            else if (Data.fixRotationWhenAttacking)
            {
                calibrate = 1;
            }

            Gizmos.color = Color.red;
            offset.Set(
                transform.position.x + (Data.hitBox.center.x * FacingDirection * calibrate * -1),
                transform.position.y + Data.hitBox.center.y
            );
            Vector2 cancelMove = new Vector2(
                transform.position.x + (Data.cancelMove.center.x * FacingDirection * -1),
                transform.position.y + Data.cancelMove.center.y
            );

            //Vector3 adjustedCenter = transform.position + new Vector3(Data.HitBox.center.x * FacingDirection * -1, Data.HitBox.center.y, 0f);
            //Gizmos.DrawWireCube(adjustedCenter, Data.HitBox.size);
            Gizmos.DrawWireCube(offset, Data.hitBox.size);
            Gizmos.DrawWireCube(cancelMove, Data.cancelMove.size);
        }
        #endregion
    }
}