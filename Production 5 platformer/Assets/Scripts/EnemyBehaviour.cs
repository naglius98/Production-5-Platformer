using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public Transform Player; // Who we are chasing
    public float ChaseSpeed = 3.0f; // How fast we are chasing
    public float JumpForce = 5.0f; // How high we are jumping
    public LayerMask GroundLayer; // What layer is the ground on
    
    private Rigidbody2D rb;
    private bool IsGrounded;
    private bool ShouldJump;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Are we grounded
        IsGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, GroundLayer);

        // Player direction
        float Direction = Mathf.Sign(Player.position.x - transform.position.x);

        // Player above dection
        bool IsPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, 1 << Player.gameObject.layer);

        if (IsGrounded)
        {

            // Chase the player
            rb.linearVelocity = new Vector2(Direction * ChaseSpeed, rb.linearVelocity.y);

            RaycastHit2D GroundInFront = Physics2D.Raycast(transform.position, new Vector2(Direction, 0), 2f, GroundLayer);

            RaycastHit2D GapAhead = Physics2D.Raycast(transform.position + new Vector3(Direction, 0, 0), Vector2.down, 2.0f, GroundLayer);
            RaycastHit2D PlatformAbove = Physics2D.Raycast(transform.position, Vector2.up, 3.0f, GroundLayer);

            if (!GroundInFront.collider && !GapAhead.collider)
            {
                ShouldJump = true;
            }
            else if (IsPlayerAbove && PlatformAbove.collider)
            {
                ShouldJump = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (IsGrounded && ShouldJump)
        {
            ShouldJump = false;
            Vector2 direction = (Player.position - transform.position).normalized;
            Vector2 JumpDirection = direction * JumpForce;
            rb.AddForce(new Vector2(JumpDirection.x, JumpForce), ForceMode2D.Impulse);
        }
    }
}
