
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float acceleration = 10f;
    public float airControlMultiplier = 0.5f;
    private Player_Climb climbSys;
    private ClimbData climbData;

    [Header("Jump & Gravity Settings")]
    public float jumpForce = 8f;
    public float gravityMultiplier = 2f;
    private bool externalGroundOverride = false;


    [Header("Ground Check Settings")]
    public LayerMask groundMask;
    public float groundCheckDistance = 0.2f;
    public float groundSphereOffset = -0.5f;
    public float groundSphereRadius = 0.3f;
    public bool showGroundCheck = true;
    public Color groundSphereColor = Color.red;

    [Header("Camera Reference")]
    public Transform cameraTransform;

    private Rigidbody rb;
    private bool isGrounded = false;
    private Vector3 moveInput;
    private Vector3 moveVelocity;
    private bool isClimbing;

    public void OverrideGrounded(bool state)
    {
        externalGroundOverride = state;
    }

    public bool IsGrounded()
    {
        return isGrounded || externalGroundOverride;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent unwanted physics rotation
        climbSys = GetComponent<Player_Climb>();
    }

    void Update()
    {

        HandleInput();
        GroundCheck();

        if (Input.GetButton("Jump") && IsGrounded())
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
        ApplyExtraGravity();
        isClimbing = climbSys.IsClimbing();


    }

    private void HandleInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        moveInput = (camForward * verticalInput + camRight * horizontalInput).normalized;

        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= sprintMultiplier;
        }
        moveVelocity = moveInput * currentSpeed;
    }

    private void MovePlayer()
    {
        if (isClimbing && climbData != null)
        {
            // Move vertically using climb speed
            Vector3 climbDirection = Vector3.up * climbData.climbSpeed;
            rb.linearVelocity = new Vector3(0f, climbDirection.y, 0f); // Optionally allow X/Z movement
            return;
        }

        // Standard grounded or airborne movement
        if (moveInput.magnitude > 0.1f)
        {
            Vector3 targetVelocity = moveVelocity;
            if (!isGrounded)
                targetVelocity *= airControlMultiplier;

            Vector3 velocityChange = targetVelocity - rb.linearVelocity;
            velocityChange.y = 0; // Preserve existing vertical velocity

            rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
        }
    }


    private void Jump()
    {
        Vector3 jumpVelocity = rb.linearVelocity;
        jumpVelocity.y = jumpForce;
        rb.linearVelocity = jumpVelocity;
    }

    private void ApplyExtraGravity()
    {
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    private void GroundCheck()
    {
        Vector3 castOrigin = transform.position + Vector3.down * groundSphereOffset;

        isGrounded = Physics.SphereCast(castOrigin, groundSphereRadius,
            Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask);
    }

    private void OnDrawGizmosSelected()
    {

        if (!showGroundCheck) return;
        Gizmos.color = groundSphereColor;

        // Match ground check logic exactly 
        Vector3 castOrigin = transform.position + Vector3.down * groundSphereOffset;
        Gizmos.DrawWireSphere(castOrigin, groundSphereRadius);


    }
}
