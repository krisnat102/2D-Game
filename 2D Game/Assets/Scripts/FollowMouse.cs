using UnityEngine;
using Core;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private Rigidbody2D rb, playerRb;

    [SerializeField] private Transform trans;
    private Vector2 mousePos, movement;

    [SerializeField] private float gunSize = 0f;

    [SerializeField] private float gunPositionOffsetX = 0f;
    [SerializeField] private float gunPositionOffsetY = 0f;

    [SerializeField] private float playerWidth = -1.35f;

    private bool side = true;

    void Update()
    {
        if (Core.GameManager.gamePaused == false)
        {
            mousePos = cam.ScreenToWorldPoint(InputManager.Instance.MousePosition);

            movement.x = InputManager.Instance.NormInputX;
            movement.y = InputManager.Instance.NormInputY;

            if (InputManager.Instance.NormInputX < 0)
            {
                side = false;
            }

            if (InputManager.Instance.NormInputX > 0)
            {
                side = true;
            }
        }
    }

    void FixedUpdate()
    {

        Vector2 lookDir = mousePos - rb.position;

        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        rb.rotation = angle;

        if (side == true)
        {
            rb.position = new Vector2(playerRb.position.x + gunPositionOffsetX, playerRb.position.y + gunPositionOffsetY);
        }
        else
        {
            rb.position = new Vector2(playerRb.position.x + gunPositionOffsetX + playerWidth, playerRb.position.y + gunPositionOffsetY);
        }

        if ((angle >= 90 && angle <= 180) || (angle >= -180 && angle <= -90))
        {
            trans.transform.localScale = new Vector3(gunSize, -gunSize, gunSize);

        }
        else if ((angle < 90 && angle > 0) || (angle < 0 && angle > -90))
        {
            trans.transform.localScale = new Vector3(gunSize, gunSize, gunSize);

        }
    }
}