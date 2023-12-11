using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{

    [SerializeField] private GameObject hitbox;
    [SerializeField] private MeleeWeaponSprite meleeWeaponSprite;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Rigidbody2D rb;

    [Header("Stats")]
    [SerializeField] private bool mainWeapon = true;
    [SerializeField] private float attackSpeed = 1.3f;

    [Header("Offsets")]
    [SerializeField] private float offsetX = 0.3f;
    [SerializeField] private float offsetY = 0f;

    private float nextAttackTime = 0.0f;

    public bool PositionUpDown { get; private set; }
    public Rigidbody2D RB { get => rb; private set => rb = value; }

    private void Start()
    {
        gameObject.SetActive(mainWeapon);
    }

    void Update()
    {
        if (GameManager.gamePaused == false)
        {
            if (InputManager.Instance.AttackInput)
            {
                Attack();
            }
        }
    }
    
    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1.0f / attackSpeed;

            //FindObjectOfType<AudioManager>().Play("SwordAttack");

            if (meleeWeaponSprite.Side)
            {
                Vector3 offset = new Vector3(offsetX, offsetY, 0);

                Instantiate(hitbox, playerTransform.position + offset, playerTransform.rotation);
            }
            else
            {
                Vector3 offset = new Vector3(-offsetX, offsetY, 0);

                Instantiate(hitbox, playerTransform.position + offset, playerTransform.rotation);
            }

            if (PositionUpDown)
            {
                PositionUpDown = false;
            }
            else
            {
                PositionUpDown = true;
            }
        }
    }
}
