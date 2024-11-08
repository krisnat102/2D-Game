using UnityEngine;
using System;
using Core;
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

        [Header("Shuriken")]
        [SerializeField] private float rotationSpeed = 0.1f;
        [SerializeField] private float stuckTime = 1f;
        [SerializeField] private float fadeTimer = 1f;

        private Rigidbody2D rb;
        private LevelHandler levelHandler;
        private bool stuckShuriken = false;
        private float transparency = 1f;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            levelHandler = FindAnyObjectByType<Player>().GetComponent<LevelHandler>();

            rb.velocity = move ? transform.right * spell.speed : rb.velocity = new Vector2(0, 0);
            transform.right = dontRotate ? new Vector2(0, 0) : transform.right;
        }

        private void Update()
        {
            Vector3 range = new Vector3(spell.range, spell.range, 0);
            /*if (Math.Abs(Abilities.castPoint.x) + range.x < Math.Abs(transform.position.x) || Math.Abs(Abilities.castPoint.y) + range.y > Math.Abs(transform.position.y))
            {
                Invoke("DestroyObject", 0.5f);

                if(anim != null)
                    if(ContainsParam(anim, "End")) anim.SetBool("End", true);
            }*/

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
                    Destroy(gameObject);
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
                else if (hitInfo.gameObject.layer == groundLayerMask)
                {
                    rb.simulated = false;
                    rotationSpeed = 0;
                    Invoke("DestroyObject", stuckTime);
                }
                else if (shuriken)
                {
                    DestroyObject();
                }
                //else they will be handled by some other script or just left as they are
            }
        }

        private void DestroyObject()
        {
            if (spell.spellDeath != null)
            {
                Instantiate(spell.spellDeath, transform.position, Quaternion.identity);
            }

            stuckShuriken = true;

            if (spell.name != "Shuriken")
            {
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
        }

        /*private bool ContainsParam(Animator _Anim, string _ParamName)
        {
            foreach (AnimatorControllerParameter param in _Anim.parameters)
            {
                if (param.name == _ParamName) return true;
            }
            return false;
        } checks if a parameter exists in the animator controller*/
    }
}