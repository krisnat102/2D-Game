using UnityEngine;
using Core;

public class MeleeWeaponSprite : MeleeWeapon
{
    [Header("Other")]
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private SpriteRenderer sprite;

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

    public bool Side { get; private set; }

    void Update()
    {
        if (Side == true)
        {
            if (PositionUpDown)
            {
                RB.position = new Vector2(playerRb.position.x + swordPositionOffsetX, playerRb.position.y + swordPositionOffsetYUp);
                RB.SetRotation(upRotationRight);
            }
            else
            {
                RB.position = new Vector2(playerRb.position.x + swordPositionOffsetX, playerRb.position.y + swordPositionOffsetYDown);
                RB.SetRotation(downRotationRight);
            }
        }
        else
        {
            if (PositionUpDown)
            {
                RB.position = new Vector2(playerRb.position.x + swordPositionOffsetX + PLAYER_WIDTH, playerRb.position.y + swordPositionOffsetYUp);
                RB.SetRotation(upRotationLeft);
            }
            else
            {
                RB.position = new Vector2(playerRb.position.x + swordPositionOffsetX + PLAYER_WIDTH, playerRb.position.y + swordPositionOffsetYDown);
                RB.SetRotation(downRotationLeft);
            }
        }

        if (InputManager.Instance.NormInputX < 0)
        {
            sprite.flipX = true;

            Side = false;
        }

        if (InputManager.Instance.NormInputX > 0)
        {
            sprite.flipX = false;

            Side = true;
        }


    }
}

