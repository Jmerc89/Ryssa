using UnityEngine;

public class Player_Climb : MonoBehaviour
{
    public LayerMask climbableMask;
    private ClimbData activeClimbData;

    private bool isClimbing = false;
    private bool nearClimbable = false;

    private Rigidbody rb;
    private PlayerController playerController;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>(); // to override isGrounded
    }

    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        if (nearClimbable && Mathf.Abs(verticalInput) > 0.1f)
        {
            StartClimbing();
        }
        else if (!nearClimbable || Mathf.Abs(verticalInput) < 0.1f)
        {
            StopClimbing();
        }

        if (isClimbing && activeClimbData != null)
        {
            Vector3 climbVelocity = Vector3.zero;

            // Vertical movement
            climbVelocity.y = verticalInput * activeClimbData.climbSpeed;

            // Optional horizontal movement while climbing
            if (activeClimbData.allow_Strafe_Movement)
            {
                climbVelocity.x = horizontalInput * activeClimbData.climbSpeed;
            }

            rb.linearVelocity = climbVelocity;

            // Tell the controller we are "grounded" while climbing
            playerController.OverrideGrounded(true);
        }
        else
        {
            playerController.OverrideGrounded(false);
        }
    }

    private void StartClimbing()
    {
        if (!isClimbing && activeClimbData != null)
        {
            isClimbing = true;
            rb.useGravity = false;
        }
    }

    private void StopClimbing()
    {
        if (isClimbing)
        {
            isClimbing = false;
            rb.useGravity = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Climable_SurfaceMB surface = other.GetComponent<Climable_SurfaceMB>();
        if (surface != null && surface.climbSettings != null)
        {
            nearClimbable = true;
            activeClimbData = surface.climbSettings;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Climable_SurfaceMB>() != null)
        {
            nearClimbable = false;
            activeClimbData = null;
        }
    }

    public bool IsClimbing() => isClimbing;
    public ClimbData GetActiveClimbData() => activeClimbData;
}
