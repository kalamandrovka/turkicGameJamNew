using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    // Attack settings
    public float lightAttackDuration = 0.5f;   // How long the light attack bool remains true
    public float heavyAttackDuration = 1.0f;     // How long the heavy attack bool remains true
    public float heavyAttackCooldown = 1.0f;     // Cooldown duration for heavy attack

    // Dash settings
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f; // cooldown time after using all dash charges
    public int maxDashCharges = 2;    // Number of dash uses allowed in a row without cooldown

    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Attack cooldown and state
    private float heavyAttackCooldownTimer = 0f;
    private bool heavyAttackActive = false;

    // Dash state and charges
    private bool isDashing = false;
    private int dashCharges;
    private float dashCooldownTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set initial dash charges
        dashCharges = maxDashCharges;
    }

    void Update()
    {
        // Update heavy attack cooldown timer
        if (heavyAttackCooldownTimer > 0f)
        {
            heavyAttackCooldownTimer -= Time.deltaTime;
        }

        // If no dash charges remain, count down the recharge timer
        if (dashCharges <= 0)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0f)
            {
                dashCharges = maxDashCharges;
            }
        }

        CheckGround();
        HandleDash(); // Process dash input

        // Only allow normal movement if not currently dashing
        if (!isDashing)
        {
            HandleMovement();
        }

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

        // Update walking animation parameter (use a float parameter "Walking" to indicate speed)
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

        // Heavy attack: triggered by pressing K if available
        if (Input.GetKeyDown(KeyCode.K) && heavyAttackCooldownTimer <= 0f && !heavyAttackActive)
        {
            Debug.Log("Heavy attack triggered.");
            heavyAttackActive = true;
            animator.SetBool("HeavyAttack", true);
            StartCoroutine(ResetHeavyAttack());
            heavyAttackCooldownTimer = heavyAttackCooldown;
        }
    }

    // Coroutine to reset the light attack bool after a set duration
    IEnumerator ResetAttack(string attackType, float duration)
    {
        yield return new WaitForSeconds(duration);
        animator.SetBool(attackType, false);
        Debug.Log(attackType + " reset after " + duration + " seconds.");
    }

    // Coroutine for heavy attack: waits until the heavy attack animation finishes before resetting
    IEnumerator ResetHeavyAttack()
    {
        while (true)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("HeavyAttack") && state.normalizedTime >= 1.0f)
            {
                break;
            }
            yield return null;
        }
        animator.SetBool("HeavyAttack", false);
        heavyAttackActive = false;
        Debug.Log("HeavyAttack reset after animation finished.");
    }

    // Dash handled with SetBool ("Dash") parameter
    void HandleDash()
    {
        // Check if the dash key is pressed (Left Shift) and if we have dash charges
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && dashCharges > 0)
        {
            Debug.Log("Dash triggered.");
            // Consume one dash charge
            dashCharges--;
            // If all charges are used, start the dash recharge timer
            if (dashCharges <= 0)
            {
                dashCooldownTimer = dashCooldown;
            }
            animator.SetBool("Dash", true);
            StartCoroutine(PerformDash());
        }
    }

    IEnumerator PerformDash()
    {
        isDashing = true;

        // Determine dash direction using horizontal input; if no input, use sprite orientation
        float dashDirection = Input.GetAxisRaw("Horizontal");
        if (dashDirection == 0)
        {
            dashDirection = spriteRenderer.flipX ? -1 : 1;
        }

        // Set dash velocity
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        animator.SetBool("Dash", false);
        // Optional: Reset horizontal velocity once dash is over
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        Debug.Log("Dash finished.");
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
