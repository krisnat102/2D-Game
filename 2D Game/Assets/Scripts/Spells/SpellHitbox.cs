 using UnityEngine;
using System;
using CoreClass;
using Krisnat;

namespace Spells
{
    public class SpellHitbox : MonoBehaviour
    {
        [SerializeField] private Spell spell;
        [SerializeField] private LayerMask layerMask, groundLayerMask;

        [Header("Spell Function")]
        [SerializeField] private bool move;
        [SerializeField] private bool destroyOnTouch;
        [SerializeField] private bool dontRotate;
        [SerializeField] private bool shuriken;
        [SerializeField] private float collisionTimeOffset = 0.03f;

        [Header("Shuriken")]
        [SerializeField] private float rotationSpeed = 0.1f;
        [SerializeField] private float stuckTime = 1f;
        [SerializeField] private float fadeTimer = 1f;

        private Rigidbody2D rb;
        private LevelHandler levelHandler;
        private GameObject death;
        private bool stuckShuriken = false;
        private float transparency = 1f;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            levelHandler = FindAnyObjectByType<Player>().GetComponent<LevelHandler>();

            rb.velocity = move ? transform.right * spell.speed : rb.velocity = new Vector2(0, 0);
            transform.right = dontRotate ? new Vector2(0, 0) : transform.right;
            death = GetComponentInChildren<Death>(true)?.gameObject;
        }

        private void Update()
        {
            if (transform.rotation.x != 0 || transform.rotation.y != 0) transform.rotation = new Quaternion(0, 0, transform.rotation.z, transform.rotation.w);

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
            Debug.Log(hitInfo.gameObject.name);
            if ((layerMask.value & (1 << hitInfo.gameObject.layer)) != 0)
            {
                Debug.Log(1);
                Enemy enemy = hitInfo.GetComponent<Enemy>();
                if (enemy)
                {
                    if (spell.spell)
                    {
                        enemy.TakeDamage(spell.value * levelHandler.IntelligenceDamage, 0, false);
                    }
                    else
                    {
                        enemy.TakeDamage(spell.value * levelHandler.DexterityDamage, 0, false);
                    }

                    if (shuriken)
                    {
                        Instantiate(enemy.Data.bloodEffect, transform.position, Quaternion.identity);
                    }
                }
                if (destroyOnTouch)
                {
                    DestroyObject();
                }
                else if ((groundLayerMask.value & (1 << hitInfo.gameObject.layer)) != 0)
                {
                    Invoke("Stuck", collisionTimeOffset);
                }
                else if (shuriken)
                {
                    DestroyObject();
                }
                //else they will be handled by some other script or just left as they are
            }
            else if ((groundLayerMask.value & (1 << hitInfo.gameObject.layer)) != 0)
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
    }
}