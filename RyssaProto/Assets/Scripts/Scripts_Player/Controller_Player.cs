using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;         // Base horizontal movement speed
    public float sprintMultiplier = 1.5f;  // Sprint multiplier when Left Shift is held

    [Header("Jump & Gravity Settings")]
    public float jumpForce = 8f;         // Initial jump velocity
    public float gravity = 20f;          // Gravity acceleration
    private bool canJump=true;

    [Header("Camera Reference")]
    public Transform cameraTransform;    // Reference to the camera for relative movement

    private float verticalVelocity = 0f; // Current vertical speed (for jumping and gravity)
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("PlayerController: No CharacterController found on this GameObject.");
        }
    }

    void Update()
    {
        MovePlayerRelativeToCamera();
    }

    private void MovePlayerRelativeToCamera()
    {
        // Get horizontal and vertical input from keyboard (WASD or Arrow keys)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Get camera's forward and right vectors, ignoring any vertical component.
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        // Calculate desired move direction relative to the camera.
        Vector3 moveDirection = (camForward * verticalInput) + (camRight * horizontalInput);
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // Check for sprint input (using Left Shift) and apply sprint multiplier.
        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= sprintMultiplier;
        }

        if (Input.GetButton("Jump")&& canJump)
        {
            canJump = false;
            verticalVelocity = jumpForce;
        }
        // Handle jumping and gravity:
        if (controller.isGrounded)
        {
            verticalVelocity = 0f; // Reset vertical velocity when grounded.
            canJump = true;
        }
        else
        {
            // Apply gravity while airborne.
            verticalVelocity -= gravity * Time.deltaTime;
        }

        // Combine horizontal movement with vertical velocity.
        Vector3 finalMovement = moveDirection * currentSpeed;
        finalMovement.y = verticalVelocity;

        // Use the CharacterController to move the player.
        controller.Move(finalMovement * Time.deltaTime);
    }
}
