using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    // Movement variables - Movement is implemented using the unity Input system
    [Header("Movement")]
    public float MovementSpeed = 5.0f;
    private float HorizontalMovement;
    private bool isFacingRight = true; // used to flip the sprite

    // Same as movement
    [Header("Jumping")] 
    public float JumpPower = 10.0f;
    public int MaxJumps = 2; // Maximum number of jumps allowed
    private int JumpsRemaining;

    [Header("Dashing")] 
    public float DashingSpeed = 20.0f;
    public float DashDuration = 0.2f;
    public float DashCooldown = 0.1f;
    private bool isDashing; // Used to check if we can call dash again
    private bool DashAvailable = true;
    // TrailRenderer DashTrailRenderer - Will be used later when adding effects


    // Ground checks variables to not infinitely jump 
    [Header("Groundcheck")]
    public Transform GroundCheckPos; // Check the position
    public Vector2 GroundCheckSize = new Vector2(0.5f, 0.05f); // How big is the "contact zone"
    public LayerMask groundLayer;
    private bool isGrounded;

    // Gravity variables
    [Header("Gravity")] 
    public float BaseGravity = 2.0f;
    public float maxFallSpeed = 18.0f;
    public float fallSpeedMultiplier = 2.0f;

    // Wall sliding variables
    [Header("Wallcheck")]
    public Transform WallCheckPos; // Check the position
    public Vector2 WallCheckSize = new Vector2(0.5f, 0.05f); // How big is the "contact zone"
    public LayerMask WallLayer;

    [Header("WallMovement")]
    public float WallSlideSpeed = 2;
    private bool isWallSliding;

    [Header("WallJumping")] 
    private bool isWallJumping;
    float WallJumpDirection;
    private float WallJumpTime = 0.5f;
    private float WallJumpTimer;
    public Vector2 WallJumpPower = new Vector2(5.0f, 10.0f);




    void Update()
    {
        // Make it so that we can't change direction while we are dashing
        if (isDashing)
        {
            return;
        }

        GroundCheck();
        Gravity();
        WallSlide();
        WallJump();

        // update the left and right velocity if we are not walljumping
        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(HorizontalMovement * MovementSpeed, rb.linearVelocity.y);
            Flip();
        }

        // Quit the application
        if (Input.GetKey(KeyCode.Q))
        {
            Application.Quit();
        }
    }

    // Control movement
        public void Move(InputAction.CallbackContext context)
    {
        HorizontalMovement = context.ReadValue<Vector2>().x;

    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && DashAvailable)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    // Control jumping
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && JumpsRemaining > 0)
        {
            // Start jump
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpPower);
            JumpsRemaining--;
        }
        else if (context.canceled)
        {
            // Cut jump short (no grounded check here)
            if (rb.linearVelocity.y > 0) // Only reduce if still going up
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                JumpsRemaining--;
            }
        }

        // Wall Jumping
        if (context.performed && WallJumpTimer > 0.0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(WallJumpDirection * WallJumpPower.x, WallJumpPower.y); // Wall jump
            WallJumpTimer = 0.0f;

            if (transform.localScale.x != WallJumpDirection)
            {
                isFacingRight = !isFacingRight; // flip the bool
                Vector3 localScale = transform.localScale;
                localScale.x *= -1.0f; // flip the sprit
                transform.localScale = localScale; // set the transform to the new value
            }

            Invoke(nameof(CancelWallJump), WallJumpTime + 0.1f); // WallJump again at wall jump + 0.1f
        }
    }

    // Used to visualize the GroundCheckSize
    private void OnDrawGizmosSelected()
    {
        // Draw the groundcheck
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(GroundCheckPos.position, GroundCheckSize);

        // Draw the wallcheck
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(WallCheckPos.position, WallCheckSize);
    }

    // Check if we are grounded
    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(GroundCheckPos.position, GroundCheckSize, 0, groundLayer))
        {
            JumpsRemaining = MaxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    // Check if we are touching a wall
    private bool WallCheck()
    {
        return Physics2D.OverlapBox(WallCheckPos.position, WallCheckSize, 0, WallLayer);
    }

    // Gravity function
    private void Gravity()
    {
        if (rb.linearVelocity.y < 0) // if the player is moving downward. Negative velocity means we are moving downward
        {
            rb.gravityScale = BaseGravity * fallSpeedMultiplier; // Make the player fall increasingly faster
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed)); // Cap fall velocity
        }
        else
        {
            rb.gravityScale = BaseGravity;
        }
    }

    private void Flip()
    {
        if (isFacingRight && HorizontalMovement < 0 || !isFacingRight && HorizontalMovement > 0)
        {
            isFacingRight = !isFacingRight; // flip the bool
            Vector3 localScale = transform.localScale;
            localScale.x *= -1.0f; // flip the sprit
            transform.localScale = localScale; // set the transform to the new value

        }
    }

    private void WallSlide()
    {
        if (!isGrounded & WallCheck() & HorizontalMovement != 0) // slide if we aren't grounded, we are on a wall and our speed is different than 0
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -WallSlideSpeed)); // Cap the fall rate
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            WallJumpDirection = -transform.localScale.x; // jump in the opposite direction
            WallJumpTimer = WallJumpTime; // reset the timer

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (WallJumpTimer > 0)
        {
            WallJumpTimer -= Time.deltaTime; // tick the timer down
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    private IEnumerator DashCoroutine()
    {
        DashAvailable = false;
        isDashing = true;
        float DashDirection = isFacingRight ? 1f : -1f; // if we are facing right, set direction to 1, if not set it to -1

        rb.linearVelocity = new Vector2(DashDirection * DashingSpeed, rb.linearVelocity.y); // Dash movement

        yield return new WaitForSeconds(DashDuration); // Return to normal speed when this ends
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // Reset velocity
        isDashing = false;

        yield return new WaitForSeconds(DashCooldown); // Dash again when this ends
        DashAvailable = true;
    }
}

