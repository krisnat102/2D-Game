using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Krisnat
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private float stuckTime = 1f;
        [SerializeField] private float fadeTimer = 1f;

        private float damage;
        private float speed;
        private int direction;
        private Bardent.CoreSystem.Core core;
        private Player player;
        private Vector2 offset;

        private Rigidbody2D rb;

        private bool arrowStuck = false;
        private float transparency = 1f;
        private SpriteRenderer sprite;

        public void SetArrowStats(float damage, float speed, int direction, Vector2 offset, Bardent.CoreSystem.Core core)
        {
            this.damage = damage;
            this.speed = speed;
            this.direction = direction;
            this.core = core;
            this.offset = offset;   
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            rb.velocity = speed * Time.deltaTime * new Vector2(direction, 0);

            player = core?.GetComponentInParent<Player>();
            if(player != null) transform.position = player.transform.position + (Vector3) offset;
            sprite = GetComponent<SpriteRenderer>();
        }
        private void Update()
        {
            TrackMovement();
            
            if (arrowStuck)
            {
                transparency -= Time.deltaTime * fadeTimer;
                if(sprite != null) sprite.color = new Color(sprite.color.r, sprite.color.b, sprite.color.g, transparency);
                if(transparency <= 0f)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var enemy = collision.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage, 0, true);
                damage = 0;
                Instantiate(enemy.BloodEffect, transform.position, Quaternion.identity);
            }
            if (collision.tag == "Ground")
            {
                rb.simulated = false;
                Invoke("StartFade", stuckTime);
            }
        }

        private void TrackMovement()
        {
            var direction = rb.velocity;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void StartFade()
        {
            arrowStuck = true;
        }
    }
}
