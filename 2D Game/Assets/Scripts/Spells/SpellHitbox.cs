using UnityEngine;
using System;
using Core;

namespace Spells
{
    public class SpellHitbox : MonoBehaviour
    {
        [SerializeField] private Spell spell;
        [SerializeField] private LayerMask layerMask, groundLayerMask;

        [Header("Spell Function")]
        [SerializeField] private bool move; 
        [SerializeField] private bool destroyOnTouch;

        [Header("Shuriken")]
        [SerializeField] private float rotationSpeed = 0.1f;
        [SerializeField] private float stuckTime = 1f;
        [SerializeField] private float fadeTimer = 1f;

        private Rigidbody2D rb;
        private bool stuckShuriken = false;
        private float transparency = 1f;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            rb.velocity = move ? transform.right * spell.speed : rb.velocity = new Vector2(0, 0);
            transform.right = new Vector2(0, 0);
        }

        private void Update()
        {
            if (Core.GameManager.gamePaused == false)
            {
                Vector3 range = new Vector3(spell.range, spell.range, 0);
                /*if (Math.Abs(Abilities.castPoint.x) + range.x < Math.Abs(transform.position.x) || Math.Abs(Abilities.castPoint.y) + range.y > Math.Abs(transform.position.y))
                {
                    Invoke("DestroyObject", 0.5f);

                    if(anim != null)
                        if(ContainsParam(anim, "End")) anim.SetBool("End", true);
                }*/

                if (spell.name == "Shuriken")
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
        }

        private void OnTriggerEnter2D(Collider2D hitInfo)
        {
            //if (hitInfo.tag != "Player" && hitInfo.tag != "Item" && hitInfo.tag != "Climbable" && hitInfo.tag != "AttackRange" && hitInfo.tag != "BackgroundObject" && hitInfo.tag != "PickupRange")
            if ((layerMask.value & (1 << hitInfo.gameObject.layer)) != 0)
            {
                Enemy enemy = hitInfo.GetComponent<Enemy>();
                if (enemy)
                {
                    enemy.TakeDamage(spell.value, 0);
                    if (spell.name == "Shuriken")
                    {
                        Instantiate(enemy.BloodEffect, transform.position, Quaternion.identity);
                    }
                }
                if (destroyOnTouch && spell.name != "Shuriken")
                {
                    DestroyObject();
                }
                else if (hitInfo.gameObject.layer == groundLayerMask)
                {
                    rb.simulated = false;
                    rotationSpeed = 0;
                    Invoke("DestroyObject", stuckTime);
                }
                else if (spell.name == "Shuriken")
                {
                    DestroyObject();
                }
                //else they will be handled by some other script or just left as they are
            }
        }

        private void DestroyObject()
        {
            if (spell.spellDeath != null)
                Instantiate(spell.spellDeath, transform.position, Quaternion.identity);
            stuckShuriken = true;
            if (spell.name != "Shuriken")
            {
                Destroy(gameObject);

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