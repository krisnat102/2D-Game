using Bardent.CoreSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class LandingDamage : MonoBehaviour
    {
        [SerializeField] private float damage;
        [SerializeField] private float groundCheckRadius = 0.5f;
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private LayerMask entitiesLayers;

        private bool grounded;
        private Animator anim;
        private Enemy enemy;
        private Player player;
        private Rigidbody2D rb;

        private void Start()
        {
            anim = GetComponent<Animator>();
            rb = GetComponentInParent<Rigidbody2D>();
        }

        private bool IsGrounded()
        {
            grounded = Physics2D.OverlapCircle(transform.position, groundCheckRadius, whatIsGround);
            return grounded;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!IsGrounded() && ((1 << collision.gameObject.layer) & entitiesLayers) != 0)
            {
                enemy = collision.gameObject.GetComponent<Enemy>();
                player = collision.gameObject.GetComponent<Player>();

                if (rb.velocity.y > 0) return; 

                if (enemy)
                {
                    enemy.TakeDamage(damage * 3f * rb.mass, 0, false);
                }

                if (player)
                {
                    player.Core.GetCoreComponent<DamageReceiver>().Damage(damage * rb.mass, true);
                }
            }

            if (((1 << collision.gameObject.layer) & whatIsGround) != 0)
            {
                anim.SetTrigger("Land");
            }
        }
    }
}
