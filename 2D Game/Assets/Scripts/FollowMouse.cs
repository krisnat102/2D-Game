using UnityEngine;
using Core;
using Pathfinding;

public class FollowMouse : MonoBehaviour
{
    private Camera cam;
    private Rigidbody2D rb;
    private Vector2 mousePos;
    private bool _side = true;

    public delegate void SideChangedEventHandler(bool newSide);

    public event SideChangedEventHandler OnSideChanged;

    public bool Side
    {
        get { return _side; }
        set
        {
            if (_side != value)
            {
                _side = value;

                OnSideChanged?.Invoke(_side);
            }
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = FindObjectOfType<Camera>();
    }

    void Update()
    {
        if (Core.GameManager.gamePaused == false)
        {
            if (PlayerInputHandler.Instance.NormInputX < 0)
            {
                Side = false;
            }

            if (PlayerInputHandler.Instance.NormInputX > 0)
            {
                Side = true;
            }
            //TODO: fix when side is true. The direction they launch is messed up. Find a way to make the y rotation of the transform -180 when side is true and 0 when its false
        }
    }

    void FixedUpdate()
    {
        mousePos = cam.ScreenToWorldPoint(PlayerInputHandler.Instance.MousePosition);
        Vector2 lookDir = mousePos - rb.position;

        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        angle = Mathf.Repeat(angle + 180f, 360f) - 180f;

        rb.MoveRotation(angle);
    }

    [System.Obsolete]
    private void OnSideChange(bool newSide)
    {
        transform.rotation.SetEulerRotation(0, -180, 0);
    }

    private void OnEnable()
    {
        OnSideChanged += OnSideChange;
    }

    private void OnDisable()
    {
        OnSideChanged -= OnSideChange;
    }
}