using UnityEngine;

public class Grappler : MonoBehaviour
{

    [Header ("Rope")]
    [SerializeField] private Material ropeMaterial;
    [SerializeField] private float ropeWidth = 0.2f;

    [Header("Other")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Spell grapplingHook;
    [SerializeField] private Abilities abilities;

    private DistanceJoint2D rope;
    private LineRenderer lineRenderer;
    
    private Vector3 mousePos;
    private RaycastHit2D hitInfo;

    private bool checker;

    void Start()
    {
        gameObject.AddComponent<LineRenderer>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    void Update()
    {
        Grapple();

        if (Input.GetButton("Ability"))
        {
            lineRenderer.SetPosition(0, firePoint.position);
        }

        // Destroy rope
        else if (Input.GetButtonUp("Ability"))
        {
            DestroyImmediate(rope);
            lineRenderer.enabled = false;
            checker = true;
        }
    }

    public void Grapple()
    {
        // Detect mouse position
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 distance = mousePos - firePoint.position;

        Debug.DrawRay(firePoint.position, distance);

        hitInfo = Physics2D.Raycast(firePoint.position, distance, grapplingHook.range, ~layerMask);
        // Shot rope on mouse position
        if (hitInfo)
        {
            if (Input.GetButton("Ability") && checker == true && hitInfo.collider.gameObject.tag == "Ground" && PlayerStats.stam >= grapplingHook.cost && abilities.AbilityCooldown1 == false)
            {
                PlayerStats.stam -= grapplingHook.cost;

                rope = gameObject.AddComponent<DistanceJoint2D>();
                lineRenderer.enabled = true;

                rope.enableCollision = true;
                rope.connectedAnchor = mousePos;

                checker = false;

                lineRenderer.material = ropeMaterial;
                lineRenderer.startWidth = ropeWidth;
                lineRenderer.SetPosition(1, mousePos);
            }
        }
    }
}