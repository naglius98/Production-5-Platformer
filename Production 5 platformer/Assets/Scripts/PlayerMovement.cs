using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    // Movement variables - Movement is implemented using the unity Input system
    [Header("Movement")]
    public float MovementSpeed = 5.0f;
    private float HorizontalMovement;

    // Same as movement
    [Header("Jumping")] 
    public float JumpPower = 10.0f;

    // Ground checks variables to not infinitely jump 
    [Header("Groundcheck")]
    public Transform GroundCheckPos; // Check the position
    public Vector2 GroundCheckSize = new Vector2(0.5f, 0.05f); // How big is the "contact zone"
    public LayerMask groundLayer;


    void Update()
    {
        // update the left and right velocity
        rb.linearVelocity = new Vector2(HorizontalMovement * MovementSpeed, rb.linearVelocity.y);
    }

    // Control movement
    public void Move(InputAction.CallbackContext context)
    {
        HorizontalMovement = context.ReadValue<Vector2>().x;

    }

    // Control jumping
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded())
        {
            // Start jump
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpPower);
        }
        else if (context.canceled)
        {
            // Cut jump short (no grounded check here)
            if (rb.linearVelocity.y > 0) // Only reduce if still going up
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }
    }

    // Used to visualize the GroundCheckSize
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(GroundCheckPos.position, GroundCheckSize);
    }

    // Check if we are grounded
    private bool isGrounded()
    {
        if (Physics2D.OverlapBox(GroundCheckPos.position, GroundCheckSize, 0, groundLayer))
        {
            return true;
        }
        return false;
    }
}
