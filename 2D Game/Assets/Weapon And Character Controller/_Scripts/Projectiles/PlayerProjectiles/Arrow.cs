using Bardent.CoreSystem;
using UnityEngine;

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
        private int piercing;
        private float gravity;
        private float transparency = 1f;
        private int chargeTier;
        private int direction;
        private bool arrowStuck = false;
        private bool hitGround;
        private Vector2 offset;
        private Vector2 lastVelocity;
        private Enemy previousEnemy;
        private Core core;
        private Player player;
        private Rigidbody2D rb;
        private SpriteRenderer sprite;
        private LevelHandler levelHandler;
        #endregion

        public void SetArrowStats(float damage, float speed, int piercing, int direction, Vector2 offset, Bardent.CoreSystem.Core core, int chargeTier)
        {
            this.damage = damage;
            this.speed = speed;
            this.piercing = piercing;
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
            levelHandler = CoreClass.GameManager.instance.gameObject.GetComponent<LevelHandler>();

            if(!player) gameObject.SetActive(false);

            gravity = rb.gravityScale;
            if(chargeTier != 1) rb.gravityScale = 0f;

            range *= chargeTier;

            rb.velocity = speed * Time.deltaTime * new Vector2(direction, 0);

            if(player != null) transform.position = player.transform.position + (Vector3) offset;
        }

        private void FixedUpdate()
        {
            if (!hitGround)
            {
                RotationFixer();
                lastVelocity = rb.velocity;
            }

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
                    gameObject.SetActive(false);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var enemy = collision.GetComponent<Enemy>();

            if(enemy != null && (!previousEnemy || previousEnemy != enemy))
            {
                previousEnemy = enemy;

                enemy.TakeDamage(damage * levelHandler.DexterityDamage, 0, true);

                if(enemy.Data.bloodEffect && damage != 0) Instantiate(enemy.Data.bloodEffect, transform.position, Quaternion.identity);

                piercing--;

                if (piercing == 0)
                {
                    gameObject.SetActive(false);
                }
            }
            if (collision.tag == "Ground" || collision.tag == "Door")
            {
                float angle = Mathf.Atan2(lastVelocity.normalized.y, lastVelocity.normalized.x) * Mathf.Rad2Deg;

                hitGround = true;

                rb.velocity = Vector2.zero;
                rb.simulated = false;

                transform.rotation = Quaternion.Euler(0, 0, angle);

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

        private float DistanceTravelled()
        {
            if(player) return Mathf.Abs(player.transform.position.x - transform.position.x);
            return 0f;
        }

        private void StartFade()
        {
            arrowStuck = true;
        }
        #endregion
    }
}
