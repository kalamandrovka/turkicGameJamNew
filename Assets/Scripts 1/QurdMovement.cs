using UnityEngine;
using System.Collections;

public class WolfController : MonoBehaviour
{
    // ------------------------------
    // Patrol Settings
    // ------------------------------
    public Transform pointA;   // Patrol point A.
    public Transform pointB;   // Patrol point B.
    public float patrolSpeed = 2f;

    // ------------------------------
    // Chase & Attack Settings
    // ------------------------------
    public float chaseSpeed = 4f;
    public float senseRange = 5f;    // Distance at which the wolf detects the player.
    public float attackRange = 1.5f; // Range within which the wolf attacks.
    public float attackCooldown = 1f; // Delay between attacks.

    // ------------------------------
    // Jumping Settings
    // ------------------------------
    public float jumpForce = 7f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    // ------------------------------
    // Health (Hit Count)
    // ------------------------------
    private int hitCount = 0;       // Number of hits taken.
    private int hitLimit = 2;       // Wolf dies after 2 hits.

    // ------------------------------
    // References
    // ------------------------------
    public Transform player;  // Reference to the player's Transform.

    private Rigidbody2D rb;
    private Animator animator;
    private bool isFacingRight = true;  // Tracks the wolf's facing direction.

    // Internal state variables.
    private Vector3 currentPatrolTarget;
    private bool isAttacking = false;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentPatrolTarget = pointA.position;  // Start patrol at point A.
    }

    void Update()
    {
        if (isDead)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // When player is sensed...
        if (distanceToPlayer <= senseRange)
        {
            if (distanceToPlayer > attackRange)
            {
                ChasePlayer();
                // Optional: jump if the player is higher.
                if (player.position.y > transform.position.y + 0.5f && IsGrounded())
                    Jump();
            }
            else
            {
                if (!isAttacking)
                    StartCoroutine(Attack());
            }
        }
        else
        {
            Patrol();
        }
    }

    // Patrolling between point A and B.
    void Patrol()
    {
        Vector3 direction = (currentPatrolTarget - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * patrolSpeed, rb.linearVelocity.y);

        if (direction.x > 0 && !isFacingRight)
            Flip();
        else if (direction.x < 0 && isFacingRight)
            Flip();

        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        // Switch target when close to current one.
        if (Vector2.Distance(transform.position, currentPatrolTarget) < 0.2f)
        {
            currentPatrolTarget = (currentPatrolTarget == pointA.position) ? pointB.position : pointA.position;
        }
    }

    // Chasing the player.
    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);

        if (direction.x > 0 && !isFacingRight)
            Flip();
        else if (direction.x < 0 && isFacingRight)
            Flip();

        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero; // Stop moving to attack.
        animator.SetTrigger("Attack");
        // (Optional: add damage logic to affect the player.)
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetTrigger("Jump");
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    // ------------------------------
    // Health Functions (Hit Count)
    // ------------------------------
    public void TakeHit()
    {
        hitCount++;
        Debug.Log("Wolf hit! Total hits: " + hitCount);
        if (hitCount >= hitLimit)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Die");
        // Optionally disable further AI actions.
        this.enabled = false;
    }

    // Flip the wolf's facing direction.
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    // Optional: Visualize groundCheck area.
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }
    }
}
