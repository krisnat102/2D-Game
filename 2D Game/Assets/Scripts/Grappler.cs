using UnityEngine;

public class Grappler : MonoBehaviour
{
    [SerializeField] private Material ropeMaterial;
    [SerializeField] private float ropeWidth = 0.2f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float range = 30f;
    [SerializeField] private Transform firePoint;

    private DistanceJoint2D rope;
    private LineRenderer lineRenderer;

    private Vector3 mousePos, playerPos;
    private RaycastHit hitInfo;

    private GameObject objectToCheck;

    bool checker;

    void Start()
    {
        gameObject.AddComponent<LineRenderer>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    void Update()
    {
        // Detect mouse position
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        playerPos = gameObject.transform.position;

        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, Input.mousePosition - firePoint.position, Mathf.Infinity, ~layerMask);
        // Shot rope on mouse position
        if (hitInfo)
        {
            Debug.Log(hitInfo.collider.tag);
            if (Input.GetMouseButton(0) && checker == true && hitInfo.collider.gameObject.tag == "Ground")
            {
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
        if (Input.GetMouseButton(0))
        {
            lineRenderer.SetPosition(0, playerPos);
        }

        // Destroy rope
        else if (Input.GetMouseButtonUp(0))
        {
            DestroyImmediate(rope);
            lineRenderer.enabled = false;
            checker = true;
        }
    }
}
