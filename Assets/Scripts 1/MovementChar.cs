using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    // Attack settings
    public float lightAttackDuration = 0.5f; // Duration for the light attack bool (using coroutine)
    // heavyAttackDuration is no longer needed since heavy attack reset is not timer-based
    public float heavyAttackCooldown = 1.0f; // Cooldown duration for heavy attack

    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Cooldown timer for heavy attack
    private float heavyAttackCooldownTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Update heavy attack cooldown timer
        if (heavyAttackCooldownTimer > 0f)
        {
            heavyAttackCooldownTimer -= Time.deltaTime;
        }

        CheckGround();
        HandleMovement();
        HandleJump();
        HandleAttack();
        UpdateAnimator();
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Mirror the sprite when moving left
        if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }

        // Update walking animation parameters
        animator.SetFloat("Walking", Mathf.Abs(moveInput));
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("Jump");
        }
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void HandleAttack()
    {
        // Light attack: triggered by pressing J
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Light attack triggered.");
            animator.SetBool("LightAttack", true);
            StartCoroutine(ResetAttack("LightAttack", lightAttackDuration));
        }

        // Heavy attack: triggered by pressing K, provided the cooldown is finished
        if (Input.GetKeyDown(KeyCode.K) && heavyAttackCooldownTimer <= 0f)
        {
            Debug.Log("Heavy attack triggered.");
            animator.SetBool("HeavyAttack", true);
            // Note: No timer (coroutine) is used here to reset the heavy attack.
            // Instead, add an Animation Event at the end of the heavy attack animation clip
            // that calls the ResetHeavyAttack() function.
            heavyAttackCooldownTimer = heavyAttackCooldown; // Set the cooldown timer
        }
    }

    // Coroutine to reset the light attack bool after a set duration
    IEnumerator ResetAttack(string attackType, float duration)
    {
        yield return new WaitForSeconds(duration);
        animator.SetBool(attackType, false);
        Debug.Log(attackType + " reset after " + duration + " seconds.");
    }

    // This method should be called by an animation event at the end of the heavy attack animation
    public void ResetHeavyAttack()
    {
        animator.SetBool("HeavyAttack", false);
        Debug.Log("Heavy attack reset via animation event.");
    }

    void UpdateAnimator()
    {
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }
    }
}
