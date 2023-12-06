using UnityEngine;

public class MeleeWeaponSprite : MonoBehaviour
{
    [Header("Other")]
    [SerializeField] private Rigidbody2D rb, playerRb;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private MeleeWeapon meleeWeapon;

    [Header("Offsets")]
    [SerializeField] private float swordPositionOffsetX = 0f;
    [SerializeField] private float swordPositionOffsetYUp = 0f;
    [SerializeField] private float swordPositionOffsetYDown = 0f;
    [Range(0, 360)]
    [SerializeField] private float upRotationRight = 80f;
    [Range(0, 360)]
    [SerializeField] private float downRotationRight = 80f;
    [Range(0, 360)]
    [SerializeField] private float upRotationLeft = 80f;
    [Range(0, 360)]
    [SerializeField] private float downRotationLeft = 80f;

    const float PLAYER_WIDTH = -0.8f;

    private bool side = true;

    public bool Side { get => side; set => side = value; }

    void Start()
    {
        gameObject.SetActive(false);
    }


    void Update()
    {
        if (Side == true)
        {
            if (meleeWeapon.PositionUpDown)
            {
                rb.position = new Vector2(playerRb.position.x + swordPositionOffsetX, playerRb.position.y + swordPositionOffsetYUp);
                rb.SetRotation(upRotationRight);
            }
            else
            {
                rb.position = new Vector2(playerRb.position.x + swordPositionOffsetX, playerRb.position.y + swordPositionOffsetYDown);
                rb.SetRotation(downRotationRight);
            }
        }
        else
        {
            if (meleeWeapon.PositionUpDown)
            {
                rb.position = new Vector2(playerRb.position.x + swordPositionOffsetX + PLAYER_WIDTH, playerRb.position.y + swordPositionOffsetYUp);
                rb.SetRotation(upRotationLeft);
            }
            else
            {
                rb.position = new Vector2(playerRb.position.x + swordPositionOffsetX + PLAYER_WIDTH, playerRb.position.y + swordPositionOffsetYDown);
                rb.SetRotation(downRotationLeft);
            }
        }

        if (Input.GetKeyDown("a"))
        {
            sprite.flipX = true;

            Side = false;
        }

        if (Input.GetKeyDown("d"))
        {
            sprite.flipX = false;

            Side = true;
        }
    }
}

