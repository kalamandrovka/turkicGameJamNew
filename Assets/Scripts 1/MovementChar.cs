using UnityEngine;
using System.Collections;


public class PlayerMovement : MonoBehaviour
{
    private CameraShake shake;

    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public Transform groundCheck; // (Optional: can still be used for drawing gizmos)
    public LayerMask groundLayer;

    // Jump settings: set maximum jump count (for double jump = 2)
    public int maxJumpCount = 1;
    private int jumpCount = 0;

    // Attack settings
    public float lightAttackDuration = 0.5f;   // How long the light attack bool remains true
    public float heavyAttackDuration = 1.0f;     // How long the heavy attack bool remains true
    public float heavyAttackCooldown = 1.0f;     // Cooldown duration for heavy attack

    // Dash settings
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f; // Cooldown time after using all dash charges
    public int maxDashCharges = 2;    // Number of dash uses allowed in a row before recharge

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
        shake = Camera.main.GetComponent<CameraShake>();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize dash charges
        dashCharges = maxDashCharges;
    }

    void Update()
    {
        // Update heavy attack cooldown timer
        if (heavyAttackCooldownTimer > 0f)
        {
            heavyAttackCooldownTimer -= Time.deltaTime;
        }

        // Handle dash recharge if all charges are used
        if (dashCharges <= 0)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0f)
            {
                dashCharges = maxDashCharges;
            }
        }

        // The ground check is now performed via collision events.
        // (Previously, CheckGround() using OverlapCircle was called here.)

        HandleDash(); // Process dash input

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

        // Update walking animation parameter (using a float parameter "Walking")
        animator.SetFloat("Walking", Mathf.Abs(moveInput));
    }

    // Updated Jumping: Allow exactly two jumps (initial jump + one mid-air jump)
    void HandleJump()
    {
        // When jump is pressed and jumps remain, perform a jump.
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("Jumping", true);
            jumpCount--; // Consume a jump
        }
        if (isGrounded)
        {
            jumpCount = maxJumpCount; // Reset jump count when grounded
            animator.SetBool("Jumping", false);
        }

    }

    // Remove the CheckGround() method since ground detection is now via collisions.
    // If you still want to display the groundCheck position in the Editor, you may leave OnDrawGizmosSelected().

    void HandleAttack()
    {
        // Light attack: triggered by pressing J
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Light attack triggered.");
            animator.SetBool("LightAttack", true);
            StartCoroutine(ResetAttack("LightAttack", lightAttackDuration));
        }

        // Heavy attack: triggered by pressing K if not on cooldown and not already active
        if (Input.GetKeyDown(KeyCode.K) && heavyAttackCooldownTimer <= 0f && !heavyAttackActive)
        {
            Debug.Log("Heavy attack triggered.");
            heavyAttackActive = true;
            animator.SetBool("HeavyAttack", true);
            StartCoroutine(ResetHeavyAttack());
            heavyAttackCooldownTimer = heavyAttackCooldown;
            StartCoroutine(StartShake());
        }
    }

    // Coroutine to reset an attack bool after a set duration
    IEnumerator ResetAttack(string attackType, float duration)
    {
        yield return new WaitForSeconds(duration);
        animator.SetBool(attackType, false);
        Debug.Log(attackType + " reset after " + duration + " seconds.");
    }

    // Coroutine for heavy attack: waits until the heavy attack animation finishes, then resets
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

    // Dash using SetBool ("Dash") parameter with 2 charges in a row before cooldown
    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && dashCharges > 0)
        {
            Debug.Log("Dash triggered.");
            dashCharges--; // Consume one dash charge
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

        // Determine dash direction: use horizontal input if available; otherwise, use sprite's orientation
        float dashDirection = Input.GetAxisRaw("Horizontal");
        if (dashDirection == 0)
        {
            dashDirection = spriteRenderer.flipX ? -1 : 1;
        }

        // Apply dash velocity
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        animator.SetBool("Dash", false);
        // Optionally, reset horizontal velocity after dash
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        Debug.Log("Dash finished.");
    }

    void UpdateAnimator()
    {
        animator.SetBool("IsGrounded", isGrounded);
    }

    // New collision-based ground detection below:

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object is on the groundLayer using bit masking.
        if ((groundLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            isGrounded = true;
            jumpCount = maxJumpCount; // Reset jump count upon landing
            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // If leaving a collision with an object on the groundLayer, update isGrounded.
        if ((groundLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            isGrounded = false;
        }
    }

    // Optional: Draw the groundCheck sphere in the editor for visualization.
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }
    }

    IEnumerator StartShake()
    {
        yield return new WaitForSeconds(0.15f);
        shake.StartCameraShake(0.25f, 0.3f);
    }
}
