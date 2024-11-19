using UnityEngine;

namespace Spells
{
    public class BearTrap : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private Spell trap;
        [SerializeField] private float holdTime;

        private bool triggered = false;
        private bool holdTarget = false;
        private Animator animator;
        private GameObject target;
        private Rigidbody2D rb, targetRB;
        private AudioSource shutSound;

        private void OnTriggerEnter2D(Collider2D hitInfo)
        {
            if ((layerMask.value & (1 << hitInfo.gameObject.layer)) != 0 && triggered == false)
            {
                Enemy enemy = hitInfo.GetComponent<Enemy>();
                if (enemy)
                {
                    animator.SetTrigger("closed");
                    triggered = true;
                    holdTarget = true;
                    Invoke("Release", holdTime);
                    target = hitInfo.gameObject;
                    target?.GetComponent<Enemy>().TakeDamage(trap.damage, 0, false);
                    targetRB = target?.GetComponent<Rigidbody2D>();
                    shutSound.Play();
                }
            }
            if ((groundLayerMask.value & (1 << hitInfo.gameObject.layer)) != 0)
            {
                rb.velocity = new Vector2(0, 0);
                rb.isKinematic = true;
            }
        }

        private void Update()
        {
            if (holdTarget && targetRB != null)
            {
                targetRB.velocity = new Vector2(0, 0);
                targetRB.isKinematic = true;

                rb.velocity = new Vector2(0, 0);
                rb.isKinematic = true;
            }
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            shutSound = GetComponent<AudioSource>();
        }

        private void Release()
        {
            holdTarget = false;
            if (targetRB != null) targetRB.isKinematic = false;
            gameObject.SetActive(false);
        }
    }
}
