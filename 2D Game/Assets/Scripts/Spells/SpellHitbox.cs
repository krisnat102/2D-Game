using UnityEngine;
using Krisnat;

namespace Spells
{
    public class SpellHitbox : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Spell spell;
        [SerializeField] private LayerMask layerMask, groundLayerMask;

        [Header("Spell Function")]
        [SerializeField] private bool move;
        [SerializeField] private bool flip;
        [SerializeField] private bool doNotRotate;
        [SerializeField] private bool destroyOnTouch;
        [SerializeField] private bool shuriken;
        [SerializeField] private bool spellHitSummon;
        [SerializeField] private float collisionTimeOffset = 0.03f;
        [SerializeField] private GameObject deathEffect;
        [SerializeField] private Rect hitBox;

        [Header("Shuriken")]
        [SerializeField] private float rotationSpeed = 0.1f;
        [SerializeField] private float stuckTime = 1f;
        [SerializeField] private float fadeTimer = 1f;

        private Rigidbody2D rb;
        private LevelHandler levelHandler;
        private Abilities abilities;
        private bool stuckShuriken = false;
        private float transparency = 1f;
        private float angle;
        private int spellDirection;
        private bool wallCollider = false;
        #endregion

        #region Unity Methods
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            Vector2 castDirection = (doNotRotate) ? Vector2.zero : transform.right.normalized;
            spellDirection = (doNotRotate || castDirection.x == 0) ? 1 : (int)Mathf.Sign(transform.right.x);

            rb.velocity = move ? castDirection * spell.speed : Vector2.zero * Time.deltaTime;

            if (doNotRotate)
            {
                transform.rotation = Quaternion.identity;
            }
            else
            {
                transform.right = rb.velocity.normalized;
            }

            angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            Invoke("NullAngle", 0.2f);
            Invoke("WallColliderOn", 0.01f);
        }

        private void Start()
        {
            levelHandler = CoreClass.GameManager.Instance.GetComponent<LevelHandler>();
            abilities = Abilities.instance;

            if (flip)
            {
                int facingDir = abilities.Side ? 1 : -1;
                transform.localScale = new Vector3(transform.localScale.x * facingDir, transform.localScale.y, transform.localScale.z);
            }
        }

        private void Update()
        {
            if (transform.rotation.eulerAngles.x != 0 || transform.rotation.eulerAngles.y != 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
            }

            if (shuriken)
            {
                transform.Rotate(0f, 0f, rotationSpeed, Space.Self);
                SpriteRenderer sprite = GetComponent<SpriteRenderer>();
                if (stuckShuriken)
                {
                    transparency -= Time.deltaTime * fadeTimer;
                    sprite.color = new Color(sprite.color.r, sprite.color.b, sprite.color.g, transparency);
                }
                if (sprite.color.a <= 0)
                {
                    gameObject.SetActive(false);
                }
            }
        }
         
        private void OnTriggerEnter2D(Collider2D hitInfo)
        {
            //if (hitInfo.tag != "Player" && hitInfo.tag != "Item" && hitInfo.tag != "Climbable" && hitInfo.tag != "AttackRange" && hitInfo.tag != "BackgroundObject" && hitInfo.tag != "PickupRange")

            if ((layerMask.value & (1 << hitInfo.gameObject.layer)) != 0)
            {
                Enemy enemy = hitInfo.GetComponent<Enemy>();
                if (enemy)
                {
                    if (spell.spell)
                    {
                        if(spellHitSummon) enemy.TakeDamage(spell.spellHitSummonDamage * levelHandler.IntelligenceDamage, 0, false);
                        else enemy.TakeDamage(spell.damage * levelHandler.IntelligenceDamage, 0, false);

                        if (spell.spellHitSummon && !spellHitSummon)
                        {
                            Invoke("SummonHitEffect", spell.spellHitSummonDelay);
                        }
                    }
                    else
                    {
                        enemy.TakeDamage(spell.damage * levelHandler.DexterityDamage, 0, false);
                    }

                    if (shuriken)
                    {
                        Instantiate(enemy.Data.bloodEffect, transform.position, Quaternion.identity);
                    }
                }
                if (!shuriken)
                {
                    if (destroyOnTouch)
                    {
                        DestroyObject();
                    }
                    else if ((groundLayerMask.value & (1 << hitInfo.gameObject.layer)) != 0)
                    {
                        Invoke("Stuck", collisionTimeOffset);
                    }
                }
                //else they will be handled by some other script or just left as they are
            }
            else if ((groundLayerMask.value & (1 << hitInfo.gameObject.layer)) != 0 && wallCollider && destroyOnTouch)
            {
                Invoke("Stuck", collisionTimeOffset);
            }
        }
        #endregion

        #region Object Manager Methods
        private void Stuck()
        {
            rb.simulated = false;
            rotationSpeed = 0;
            Invoke("DestroyObject", stuckTime);
        }
        private void DestroyObject()
        {
            if (deathEffect)
            {
                deathEffect.transform.localScale = transform.localScale;
                deathEffect.SetActive(true);
                deathEffect.transform.parent = null;
            }
            else if (spell.spellDeath)
            {
                Instantiate(spell.spellDeath, transform.position, Quaternion.identity);
            }

            stuckShuriken = true;

            if (spell.name != "Shuriken")
            {
                gameObject.SetActive(false);
            }
        }

        private void NullAngle() => angle = 0;

        private void WallColliderOn() => wallCollider = true;
        #endregion

        #region Spell Methods
        private void SpawnEffect()
        {
            var offset = new Vector2();
            offset.Set(
            transform.position.x + hitBox.center.x,
            transform.position.y + hitBox.center.y
            );

            var detected = Physics2D.OverlapBoxAll(offset, hitBox.size, 0f, layerMask);

            if (detected.Length == 0) return;

            foreach (Collider2D obj in detected)
            {
                var enemy = obj.gameObject.GetComponent<Enemy>();

                if (enemy)
                {
                    if (spellHitSummon) enemy.TakeDamage(spell.spellHitSummonDamage * levelHandler.IntelligenceDamage, 0, false);
                    else enemy.TakeDamage(spell.damage * levelHandler.IntelligenceDamage, 0, false);
                }
            }
        }
        #endregion

        #region Hit Effect
        private void SummonHitEffect()
        {
            //int facingDir = Abilities.instance.Side ? 1 : -1;
            float offsetX = 0.4f * spellDirection;
            Vector2 spawnPosition = new Vector2(transform.position.x + offsetX, transform.position.y + 0.4f);
            Instantiate(spell.spellHitSummon, spawnPosition, Quaternion.identity);
        }
        #endregion

        #region Gizmos
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            var offset = new Vector2();
            offset.Set(
            transform.position.x + hitBox.center.x,
            transform.position.y + hitBox.center.y
            );

            Gizmos.DrawWireCube(offset, hitBox.size);
        }
        #endregion
    }
}