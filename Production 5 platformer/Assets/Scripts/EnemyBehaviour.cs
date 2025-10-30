using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour
{
    private Transform Player; // Who we are chasing
    public float ChaseSpeed = 3.0f; // How fast we are chasing
    public float JumpForce = 5.0f; // How high we are jumping
    public LayerMask GroundLayer; // What layer is the ground on

    // Damage
    public int Damage = 1;

    // Health
    public int MaxHealth = 3;
    private int CurrentHealth;
    private SpriteRenderer spriteRenderer;
    private Color OriginalColor;
    
    private Rigidbody2D rb;
    private bool IsGrounded;
    private bool ShouldJump;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CurrentHealth = MaxHealth;
        OriginalColor = spriteRenderer.color;
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            Player = playerObj.GetComponent<Transform>();
        }
    }

    void Update()
    {
        // Make sure we have a player reference
        if (Player == null)
        {
            FindPlayer();
            return;
        }

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
        if (Player != null && IsGrounded && ShouldJump)
        {
            ShouldJump = false;
            Vector2 direction = (Player.position - transform.position).normalized;
            Vector2 JumpDirection = direction * JumpForce;
            rb.AddForce(new Vector2(JumpDirection.x, JumpForce), ForceMode2D.Impulse);
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        StartCoroutine(FlashWhite());
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = OriginalColor;
    }
}
