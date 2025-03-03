using UnityEngine;
using Krisnat;
using Bardent.CoreSystem;

namespace Spells
{
    public class SpellHitbox : MonoBehaviour
    {
        [SerializeField] private Spell spell;
        [SerializeField] private LayerMask layerMask, groundLayerMask;

        [Header("Spell Function")]
        [SerializeField] private bool move;
        [SerializeField] private bool destroyOnTouch;
        [SerializeField] private bool doNotRotate;
        [SerializeField] private bool flip;
        [SerializeField] private bool shuriken;
        [SerializeField] private float collisionTimeOffset = 0.03f;

        [Header("Shuriken")]
        [SerializeField] private float rotationSpeed = 0.1f;
        [SerializeField] private float stuckTime = 1f;
        [SerializeField] private float fadeTimer = 1f;

        private Rigidbody2D rb;
        private LevelHandler levelHandler;
        private Abilities abilities;
        private GameObject death;
        private bool stuckShuriken = false;
        private float transparency = 1f;
        private float angle;
        private bool wallCollider = false;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            Vector2 castDirection = (doNotRotate) ? Vector2.zero : transform.right.normalized;
            rb.velocity = move ? castDirection * spell.speed : Vector2.zero * Time.deltaTime;
            death = GetComponentInChildren<Death>(true)?.gameObject;

            if (doNotRotate)
            {
                transform.rotation = Quaternion.identity;
            }
            else
            {
                transform.right = rb.velocity.normalized;
            }

            if (flip)
            {
                int facingDir = abilities.Side ? 1 : -1;
                transform.localScale = new Vector3(transform.localScale.x * facingDir, transform.localScale.y, transform.localScale.z);
            }

            angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            Invoke("NullAngle", 0.2f);
            Invoke("WallColliderOn", 0.01f);
        }

        private void Start()
        {
            levelHandler = CoreClass.GameManager.Instance.GetComponent<LevelHandler>();
            abilities = Stats.Instance.gameObject.GetComponent<Abilities>();
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
                        enemy.TakeDamage(spell.damage * levelHandler.IntelligenceDamage, 0, false);
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

        private void Stuck()
        {
            rb.simulated = false;
            rotationSpeed = 0;
            Invoke("DestroyObject", stuckTime);
        }
        private void DestroyObject()
        {
            if (death)
            {
                if (death.GetComponent<Death>().AdaptSize) death.transform.localScale = transform.localScale;
                death.SetActive(true);
                death.transform.parent = null;
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
    }
}