using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // ------------------------------
    // Health (Hit Count) Settings
    // ------------------------------
    private int hitCount = 0;      // Number of hits taken
    private int hitLimit = 100;      // Player dies at 5 hits

    // ------------------------------
    // Camera Shake Reference
    // ------------------------------
    private CameraShake shake;

    // ------------------------------
    // Movement Settings
    // ------------------------------
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public Transform groundCheck; // (Optional: used for visualizing ground detection)
    public LayerMask groundLayer;

    // Jumping: (maxJumpCount = 1 means a single jump)
    public int maxJumpCount = 1;
    private int jumpCount = 0;

    // ------------------------------
    // Attack Settings
    // ------------------------------
    public float lightAttackDuration = 0.5f;
    public float heavyAttackDuration = 1.0f;
    public float heavyAttackCooldown = 1.0f;

    // The attack ranges are used to check if an enemy is close enough.
    public float lightAttackRange = 0.55f;
    public float heavyAttackRange = 1f;

    public int lightAttackDamage = 1;
    public int heavyAttackDamage = 2;

    // ------------------------------
    // Dash Settings
    // ------------------------------
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f;
    public int maxDashCharges = 2;

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

        dashCharges = maxDashCharges;
    }

    void Update()
    {
        // Update heavy attack cooldown timer.
        if (heavyAttackCooldownTimer > 0f)
            heavyAttackCooldownTimer -= Time.deltaTime;

        // Recharge dash charges.
        if (dashCharges <= 0)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0f)
                dashCharges = maxDashCharges;
        }

        HandleDash();
        if (!isDashing)
            HandleMovement();

        HandleJump();
        HandleAttack();
        UpdateAnimator();
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Flip sprite based on input direction.
        if (moveInput < 0)
            spriteRenderer.flipX = true;
        else if (moveInput > 0)
            spriteRenderer.flipX = false;

        animator.SetFloat("Walking", Mathf.Abs(moveInput));
    }

    void HandleJump()
    {
        // If jump button is pressed and jumps remain, jump.
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("Jumping", true);
            jumpCount--; // Consume a jump.
        }

        // When grounded, reset the jump counter and clear the jump animation.
        if (isGrounded)
        {
            jumpCount = maxJumpCount;
            animator.SetBool("Jumping", false);
        }
    }

    // Attack logic now checks for key presses and then finds an enemy (by tag "Enemy")
    // that is within range. If one is found, it applies one hit.
    void HandleAttack()
    {
        // Light attack with "J" key.
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Light attack triggered.");
            animator.SetBool("LightAttack", true);
            DamageEnemy(lightAttackRange, lightAttackDamage);
            StartCoroutine(ResetAttack("LightAttack", lightAttackDuration));
        }

        // Heavy attack with "K" key.
        if (Input.GetKeyDown(KeyCode.K) && heavyAttackCooldownTimer <= 0f && !heavyAttackActive)
        {
            Debug.Log("Heavy attack triggered.");
            heavyAttackActive = true;
            animator.SetBool("HeavyAttack", true);
            DamageEnemy(heavyAttackRange, heavyAttackDamage);
            StartCoroutine(ResetHeavyAttack());
            heavyAttackCooldownTimer = heavyAttackCooldown;
            StartCoroutine(StartShake());
        }
    }

    // Searches for an enemy in range (using the given range parameter) and, if found,
    // applies one hit.
    void DamageEnemy(float range, int damage)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (Vector2.Distance(transform.position, enemy.transform.position) <= range)
            {
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage); // Apply one hit (damage=1).
                }
                break; // Only hit one enemy per attack.
            }
        }
    }

    IEnumerator ResetAttack(string attackType, float duration)
    {
        yield return new WaitForSeconds(duration);
        animator.SetBool(attackType, false);
        Debug.Log(attackType + " reset after " + duration + " seconds.");
    }

    IEnumerator ResetHeavyAttack()
    {
        while (true)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("HeavyAttack") && state.normalizedTime >= 1.0f)
                break;
            yield return null;
        }
        animator.SetBool("HeavyAttack", false);
        heavyAttackActive = false;
        Debug.Log("HeavyAttack reset after animation finished.");
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && dashCharges > 0)
        {
            Debug.Log("Dash triggered.");
            dashCharges--; // Use one dash charge.
            if (dashCharges <= 0)
                dashCooldownTimer = dashCooldown;
            animator.SetBool("Dash", true);
            StartCoroutine(PerformDash());
        }
    }

    IEnumerator PerformDash()
    {
        isDashing = true;
        float dashDirection = Input.GetAxisRaw("Horizontal");
        if (dashDirection == 0)
            dashDirection = spriteRenderer.flipX ? -1 : 1;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        animator.SetBool("Dash", false);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        Debug.Log("Dash finished.");
    }

    void UpdateAnimator()
    {
        animator.SetBool("IsGrounded", isGrounded);
    }

    // Ground detection using collision events.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            isGrounded = true;
            jumpCount = maxJumpCount;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) != 0)
            isGrounded = false;
    }

    // Optional: Visualize the groundCheck area.
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

    // ------------------------------
    // Health Functions (Hit Count)
    // ------------------------------
    public void TakeHit()
    {
        hitCount++;
        Debug.Log("Player hit! Total hits: " + hitCount);
        if (hitCount >= hitLimit)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        animator.SetTrigger("Die");
        rb.linearVelocity = Vector2.zero;
        // Optionally disable further control/input here.
        this.enabled = false;
    }
}