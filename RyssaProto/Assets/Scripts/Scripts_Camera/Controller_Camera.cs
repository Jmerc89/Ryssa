using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target & Offset")]
    // The object the camera will follow (e.g., your player)
    public Transform target;

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    [Header("Tilt Settings")]
    public float tiltSpeed = 2f;
    public float minTilt = 20f;
    public float maxTilt = 80f;

    [Header("Horizontal Rotation Settings")]
    public float horizontalRotationSpeed = 2f; // Speed at which the player rotates horizontally

    // Current camera parameters
    private float currentZoom;
    private float currentTilt;
    private float currentYaw;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("CameraController: No target assigned.");
            return;
        }

        // Calculate the initial distance from the target.
        currentZoom = Vector3.Distance(transform.position, target.position);

        // Get the initial tilt and yaw from the camera's rotation.
        currentTilt = transform.eulerAngles.x;
        currentYaw = transform.eulerAngles.y;
    }

    void Update()
    {
        if (target == null) return;

        HandleZoom();
        HandleTilt();
        HandleHorizontalRotation();
        UpdateCameraPosition();
    }

    // Adjust zoom based on the mouse scroll wheel input
    private void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scrollInput * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    // Adjust tilt based on mouse Y movement
    private void HandleTilt()
    {
        float mouseY = Input.GetAxis("Mouse Y");
        currentTilt -= mouseY * tiltSpeed;
        currentTilt = Mathf.Clamp(currentTilt, minTilt, maxTilt);
    }

    // Adjust horizontal rotation based on mouse X movement and update the target rotation
    private void HandleHorizontalRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        currentYaw += mouseX * horizontalRotationSpeed;

        // Rotate the player (target) horizontally.
        // This sets the player's Y-axis rotation to match the camera's yaw.
        target.rotation = Quaternion.Euler(0, currentYaw, 0);
    }

    // Update the camera position based on the current zoom, tilt, and yaw values.
    private void UpdateCameraPosition()
    {
        // Calculate the new rotation based on current tilt and yaw.
        Quaternion rotation = Quaternion.Euler(currentTilt, currentYaw, 0);

        // Calculate the new position based on the target position, rotation, and zoom distance.
        Vector3 direction = rotation * Vector3.forward;
        transform.position = target.position - direction * currentZoom;

        // Ensure the camera always looks at the target.
        transform.LookAt(target);
    }
}
