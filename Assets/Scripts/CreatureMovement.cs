using UnityEngine;
using System.Collections;

public class EnemyCreatureController : MonoBehaviour
{
    // ------------------------------
    // Movement & AI Settings
    // ------------------------------
    public float moveSpeed = 2f;
    public float patrolSpeed = 1.5f;       // Speed while patrolling
    public float senseRange = 6f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float patrolDuration = 2f;      // Time to move in each direction
    private bool isAttacking = false;
    private bool isFacingRight = true;
    private int patrolDirection = 1;       // 1 for right, -1 for left
    private float patrolTimer;

    // ------------------------------
    // References
    // ------------------------------
    public Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        patrolTimer = 0;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= senseRange)
        {
            if (distanceToPlayer > attackRange)
            {
                ChasePlayer();
            }
            else
            {
                if (!isAttacking)
                    StartCoroutine(Attack());
            }
        }
        else
        {
            // Player is out of range - patrol left/right
            Patrol();
        }
    }

    void Patrol()
    {
        animator.SetBool("Walking", true);

        // Update patrol timer and direction
        patrolTimer += Time.deltaTime;
        if (patrolTimer >= patrolDuration)
        {
            patrolDirection *= -1;
            patrolTimer = 0;
        }

        // Flip sprite if needed
        if ((patrolDirection < 0 && !isFacingRight) || (patrolDirection > 0 && isFacingRight))
        {
            Flip();
        }

        // Apply movement
        rb.linearVelocity = new Vector2(patrolDirection * patrolSpeed, rb.linearVelocity.y);
    }

    void ChasePlayer()
    {
        animator.SetBool("Walking", true);
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        if (direction.x < 0 && !isFacingRight)
            Flip();
        else if (direction.x > 0 && isFacingRight)
            Flip();

        animator.SetBool("Attack", false);
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("Attack", true);

        yield return new WaitForSeconds(0.1f);

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeHit();
        }

        yield return new WaitForSeconds(attackCooldown);
        animator.SetBool("Attack", false);
        isAttacking = false;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}