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

    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Cooldown timer for heavy attack and a flag to prevent spamming
    private float heavyAttackCooldownTimer = 0f;
    private bool heavyAttackActive = false;

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

        // Heavy attack: triggered by pressing K, provided cooldown is finished and heavy attack is not already active
        if (Input.GetKeyDown(KeyCode.K) && heavyAttackCooldownTimer <= 0f && !heavyAttackActive)
        {
            Debug.Log("Heavy attack triggered.");
            heavyAttackActive = true; // Block further heavy attack triggers until reset
            animator.SetBool("HeavyAttack", true);
            StartCoroutine(ResetHeavyAttack());
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

    // Coroutine for heavy attack: wait until the heavy attack animation finishes, then reset
    IEnumerator ResetHeavyAttack()
    {
        // Wait until the heavy attack animation state (named "HeavyAttack") finishes its playback.
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
        heavyAttackActive = false; // Allow new heavy attacks to be triggered
        Debug.Log("HeavyAttack reset after animation finished.");
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
