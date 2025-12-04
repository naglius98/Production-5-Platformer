using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour
{
   [Header("Target")]
    private Transform Player;
    
    [Header("Movement")]
    public float ChaseSpeed = 3.0f;
    public float PatrolSpeed = 1.5f;
    public float JumpForce = 5.0f;
    
    [Header("Detection")]
    public float DetectionRadius = 10f;
    public float PatrolDistance = 5f;
    public LayerMask GroundLayer;
    
    [Header("AI Intelligence")]
    [Range(0f, 1f)] public float JumpAccuracy = 0.8f;
    [Range(0f, 1f)] public float DropChance = 0.4f; // Chance to drop down instead of jumping
    public bool UseLineOfSight = true;
    public bool CanPredictMovement = false;
    
    [Header("Combat")]
    public int Damage = 1;
    public int MaxHealth = 3;
    private int CurrentHealth;
    
    [Header("Audio")]
    public AudioClip DeathSound;
    
    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private Color OriginalColor;
    
    // State
    private bool IsGrounded;
    private bool ShouldJump;
    private bool isPatrolling = false;
    private Vector3 patrolPoint;
    private Vector3 patrolStartPoint;
    public bool CanSeePlayer { get; private set; }
    
    // Stuck detection
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private const float STUCK_THRESHOLD = 0.1f;
    private const float STUCK_TIME_LIMIT = 1.5f;
    
    // Wall stuck detection 
    private float wallStuckTimer = 0f;
    private const float WALL_STUCK_TIME_LIMIT = 0.5f;
    private bool isTouchingWall = false;
    private float lastMoveDirection = 1f;
    
    // Gap decision cooldown 
    private float gapDecisionCooldown = 0f;
    private bool lastGapDecision = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        CurrentHealth = MaxHealth;
        OriginalColor = spriteRenderer.color;
        lastPosition = transform.position;
        patrolStartPoint = transform.position;
        SetNewPatrolPoint();
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
        if (Player == null)
        {
            FindPlayer();
            return;
        }

        // Update cooldowns
        if (gapDecisionCooldown > 0)
        {
            gapDecisionCooldown -= Time.deltaTime;
        }

        // Ground check 
        Vector2 groundCheckPos = new Vector2(transform.position.x, transform.position.y - 0.1f);
        IsGrounded = Physics2D.Raycast(groundCheckPos, Vector2.down, 1.0f, GroundLayer);

        /
        CheckWallCollision();
        DetectIfStuck();

        // Decide behavior based on player distance and line of sight
        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);
        CanSeePlayer = !UseLineOfSight || HasLineOfSight();
        bool canSeePlayer = CanSeePlayer;

        if (distanceToPlayer <= DetectionRadius && canSeePlayer)
        {
            isPatrolling = false;
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void ChasePlayer()
    {
        Vector2 targetPosition = CanPredictMovement ? PredictPlayerPosition() : (Vector2)Player.position;
        float direction = Mathf.Sign(targetPosition.x - transform.position.x);
        
        // If direction = 0, move in the last known direction
        if (Mathf.Approximately(direction, 0f))
        {
            direction = lastMoveDirection;
        }
        else
        {
            lastMoveDirection = direction;
        }

        // Move toward player
        rb.linearVelocity = new Vector2(direction * ChaseSpeed, rb.linearVelocity.y);

        // Only check for jumps when grounded
        if (IsGrounded)
        {
            CheckForJumpOpportunity(direction);
        }
    }

    private void Patrol()
    {
        isPatrolling = true;

        // Check if reached patrol point
        if (Vector2.Distance(transform.position, patrolPoint) < 0.5f)
        {
            SetNewPatrolPoint();
        }

        float direction = Mathf.Sign(patrolPoint.x - transform.position.x);
        
        // If direction = 0, move in the last known direction
        if (Mathf.Approximately(direction, 0f))
        {
            direction = lastMoveDirection;
        }
        else
        {
            lastMoveDirection = direction;
        }
            
        rb.linearVelocity = new Vector2(direction * PatrolSpeed, rb.linearVelocity.y);

        // Only check for jumps when grounded
        if (IsGrounded)
        {
            CheckForJumpOpportunity(direction);
        }
    }

    private void SetNewPatrolPoint()
    {
        float randomOffset = Random.Range(-PatrolDistance, PatrolDistance);

        // Ensure we don't pick a point too close to current position
        if (Mathf.Abs(randomOffset) < 1f)
        {
            randomOffset = lastMoveDirection * PatrolDistance * 0.5f;
        }
        patrolPoint = patrolStartPoint + new Vector3(randomOffset, 0, 0);
    }

    private void CheckForJumpOpportunity(float direction)
    {
        // Don't check for jumps if direction is invalid
        if (Mathf.Approximately(direction, 0f))
        {
            return;
        }
        
        // Check for wall in front
        RaycastHit2D wallInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 1.5f, GroundLayer);
        
        // Check for gap ahead
        Vector2 gapCheckPos = transform.position + new Vector3(direction * 0.8f, 0, 0);
        RaycastHit2D gapAhead = Physics2D.Raycast(gapCheckPos, Vector2.down, 2.0f, GroundLayer);
        
        // Check for platform above
        RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 3.0f, GroundLayer);
        bool isPlayerAbove = Player != null && Player.position.y > transform.position.y + 1f;
        bool isPlayerBelow = Player != null && Player.position.y < transform.position.y - 1f;

        // Wall ahead 
        if (wallInFront.collider != null)
        {
            ShouldJump = true;
        }

        // Do we jump or drop
        else if (gapAhead.collider == null)
        {
            // Check if there's ground below the gap 
            RaycastHit2D groundBelow = Physics2D.Raycast(gapCheckPos, Vector2.down, 10f, GroundLayer);
            bool canDropSafely = groundBelow.collider != null;
            
            // If player is below and we can drop, always drop
            if (isPlayerBelow && canDropSafely && !isPatrolling)
            {
                ShouldJump = false; 
            }

            // If we can drop safely, randomly decide to jump or drop
            else if (canDropSafely)
            {
                if (gapDecisionCooldown <= 0)
                {
                    gapDecisionCooldown = 0.5f; 
                    lastGapDecision = Random.value > DropChance; // Jump if roll > DropChance
                }
                ShouldJump = lastGapDecision;
            }
            else
            {
                ShouldJump = true;
            }
        }
        // Player above and platform to jump to
        else if (isPlayerAbove && platformAbove.collider != null && !isPatrolling)
        {
            ShouldJump = true;
        }
    }

    private void CheckWallCollision()
    {
        float direction = Mathf.Sign(rb.linearVelocity.x);
        if (Mathf.Approximately(direction, 0f))
        {
            direction = lastMoveDirection;
        }
        
        // Check for wall in the direction we're moving
        RaycastHit2D wallCheck = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 0.6f, GroundLayer);
        isTouchingWall = wallCheck.collider != null;
        
        // If touching wall and not grounded, we might be stuck on side of platform
        if (isTouchingWall && !IsGrounded)
        {
            wallStuckTimer += Time.deltaTime;
            
            if (wallStuckTimer > WALL_STUCK_TIME_LIMIT)
            {
                // Push away from wall and reverse direction
                rb.linearVelocity = new Vector2(-direction * ChaseSpeed * 0.5f, rb.linearVelocity.y);
                wallStuckTimer = 0f;
                
                if (isPatrolling)
                {
                    SetNewPatrolPoint();
                }
            }
        }
        else
        {
            wallStuckTimer = 0f;
        }
    }

    private bool CanMakeJump(float gapWidth, float gapHeight)
    {
        // Calculate jump physics
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float jumpTime = Mathf.Sqrt(2 * JumpForce / gravity);
        float horizontalDistance = (isPatrolling ? PatrolSpeed : ChaseSpeed) * jumpTime;
        
        // Can we clear the gap horizontally and vertically?
        return horizontalDistance >= gapWidth * 0.8f && JumpForce >= gapHeight * 2f;
    }

    private bool HasLineOfSight()
    {
        if (Player == null)
        {
            return false;
        }
        
        Vector2 direction = Player.position - transform.position;
        float distance = direction.magnitude;
        
        if (distance > DetectionRadius)
        {
            return false;
        }
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance, GroundLayer);
        
        // If raycast didn't hit anything, we have clear line of sight
        return hit.collider == null;
    }

    private Vector2 PredictPlayerPosition()
    {
        if (Player == null)
        {
            return Vector2.zero;
        }
        
        Rigidbody2D playerRb = Player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            float timeToReach = Vector2.Distance(transform.position, Player.position) / ChaseSpeed;
            return (Vector2)Player.position + playerRb.linearVelocity * timeToReach * 0.5f;
        }
        return Player.position;
    }

    private void DetectIfStuck()
    {
        // Check if enemy is not moving
        float movementThreshold = IsGrounded ? STUCK_THRESHOLD : STUCK_THRESHOLD * 0.5f;
        
        if (Vector3.Distance(transform.position, lastPosition) < movementThreshold)
        {
            stuckTimer += Time.deltaTime;
            
            float timeLimit = IsGrounded ? STUCK_TIME_LIMIT : STUCK_TIME_LIMIT * 0.75f;
            
            if (stuckTimer > timeLimit)
            {
                if (IsGrounded)
                {
                    ShouldJump = true;
                }
                else
                {
                    // Push away from whatever we're stuck on
                    float pushDirection = isTouchingWall ? -lastMoveDirection : lastMoveDirection;
                    rb.linearVelocity = new Vector2(pushDirection * ChaseSpeed * 0.5f, rb.linearVelocity.y - 0.5f);
                }
                
                stuckTimer = 0f;
                
                if (isPatrolling)
                {
                    SetNewPatrolPoint();
                }
            }
        }
        else
        {
            stuckTimer = 0f;
        }
        
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (IsGrounded && ShouldJump)
        {
            ShouldJump = false;
            
            // Calculate jump direction
            Vector2 jumpDirection;
            if (Player != null && !isPatrolling)
            {
                Vector2 targetPos = CanPredictMovement ? PredictPlayerPosition() : (Vector2)Player.position;
                Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
                jumpDirection = new Vector2(direction.x * JumpForce * 0.5f, JumpForce);
            }
            else
            {
                // Patrol jump 
                float direction = Mathf.Sign(rb.linearVelocity.x);
                if (Mathf.Approximately(direction, 0f))
                {
                    direction = lastMoveDirection;
                }
                jumpDirection = new Vector2(direction * JumpForce * 0.3f, JumpForce);
            }
            
            rb.AddForce(jumpDirection, ForceMode2D.Impulse);
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        StartCoroutine(FlashWhite());
        
        if (CurrentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        // Disable enemy
        rb.linearVelocity = Vector2.zero;
        this.enabled = false;
        
        // Play death sound
        if (DeathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(DeathSound);
            yield return new WaitForSeconds(0.2f);
        }
        
        Destroy(gameObject);
    }

    private IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.25f);
        spriteRenderer.color = OriginalColor;
    }

    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        // Detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);
        
        // Patrol range
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(patrolStartPoint + Vector3.left * PatrolDistance, 
                           patrolStartPoint + Vector3.right * PatrolDistance);
            Gizmos.DrawWireSphere(patrolPoint, 0.3f);
        }
        
        // Line of sight
        if (Player != null && UseLineOfSight)
        {
            Gizmos.color = HasLineOfSight() ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, Player.position);
        }
    }
}
