using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    public float rotationSpeed = 5f;

    void Update()
    {
        // Get the mouse position in screen coordinates
        Vector3 mousePos = PlayerInputHandler.Instance.MousePosition;

        // Convert the mouse position to world coordinates
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, transform.position.z));

        // Calculate the direction from the object to the mouse position
        Vector2 direction = (mousePos - transform.position).normalized;

        // Calculate the angle between the direction and the right vector (1,0)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the object towards the mouse position
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}
