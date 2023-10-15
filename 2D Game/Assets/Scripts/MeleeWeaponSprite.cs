using UnityEngine;

public class MeleeWeaponSprite : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb, playerRb;
    [SerializeField] private SpriteRenderer sprite;

    [SerializeField] private float swordPositionOffsetX = 0f;
    [SerializeField] private float swordPositionOffsetY = 0f;

    [SerializeField] private float playerWidth = -1.35f;

    public static bool side = true;

    void Start()
    {
        gameObject.SetActive(false);
    }

    
    void Update()
    {
        if (side == true)
        {
            rb.position = new Vector2(playerRb.position.x + swordPositionOffsetX, playerRb.position.y + swordPositionOffsetY);
        }
        else
        {
            rb.position = new Vector2(playerRb.position.x + swordPositionOffsetX + playerWidth, playerRb.position.y + swordPositionOffsetY);
        }
            

        if (Input.GetKeyDown("a"))
        {
            sprite.flipX = true;

            side = false;
        }

        if (Input.GetKeyDown("d"))
        {
            sprite.flipX = false;

            side = true;
        }
    }
}
