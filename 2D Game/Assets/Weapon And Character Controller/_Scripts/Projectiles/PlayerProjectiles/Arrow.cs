using Bardent.CoreSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Krisnat
{
    public class Arrow : MonoBehaviour
    {
        #region Private Variables
        [SerializeField] private float stuckTime = 1f;
        [SerializeField] private float fadeTimer = 1f;
        [SerializeField] private float range = 15f;  

        private float damage;
        private float speed;
        private float gravity;
        private float transparency = 1f;
        private int chargeTier;
        private int direction;
        private bool arrowStuck = false;
        private Vector2 offset;
        private Bardent.CoreSystem.Core core;
        private Player player;
        private Rigidbody2D rb;
        private SpriteRenderer sprite;
        private LevelHandler levelHandler;
        #endregion

        public void SetArrowStats(float damage, float speed, int direction, Vector2 offset, Bardent.CoreSystem.Core core, int chargeTier)
        {
            this.damage = damage;
            this.speed = speed;
            this.direction = direction;
            this.core = core;
            this.offset = offset;
            this.chargeTier = chargeTier;
        }

        #region Unity Methods
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            player = core?.GetComponentInParent<Player>();
            sprite = GetComponent<SpriteRenderer>();
            levelHandler = Stats.Instance.gameObject.GetComponentInParent<LevelHandler>();

            gravity = rb.gravityScale;
            if(chargeTier != 1) rb.gravityScale = 0f;

            range *= chargeTier;

            rb.velocity = speed * Time.deltaTime * new Vector2(direction, 0);

            if(player != null) transform.position = player.transform.position + (Vector3) offset;
        }
        private void Update()
        {
            RotationFixer();

            if(DistanceTravelled() > range)
            {
                rb.gravityScale = gravity;
            }
            
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
                enemy.TakeDamage(damage * levelHandler.DexterityDamage, 0, true);
                damage = 0;
                if(enemy.BloodEffect) Instantiate(enemy.BloodEffect, transform.position, Quaternion.identity);
            }
            if (collision.tag == "Ground")
            {
                rb.simulated = false;
                Invoke("StartFade", stuckTime);
            }
        }
        #endregion

        #region Other Methods
        private void RotationFixer()
        {
            var direction = rb.velocity;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private float DistanceTravelled() => Mathf.Abs(player.transform.position.x - transform.position.x);

        private void StartFade()
        {
            arrowStuck = true;
        }
        #endregion
    }
}
