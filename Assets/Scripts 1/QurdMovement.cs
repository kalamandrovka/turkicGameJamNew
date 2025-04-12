using UnityEngine;
using System.Collections;

public class QurdController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;       // Horizontal movement speed
    public float jumpForce = 10f;      // Jump impulse force

    [Header("Ground Check")]
    public Transform groundCheck;      // Position used for checking ground
    public LayerMask groundLayer;      // Which layers constitute the ground

    [Header("Animation Settings")]
    // These parameters are expected to match the ones in the Animator.
    // For example, a float parameter "Speed" and triggers "Jump", "Attack", "Die"

    [Header("Attack Settings")]
    public float attackCooldown = 1f;  // Time between attacks
    private float attackTimer = 0f;
    private bool isAttacking = false;

    private Rigidbody2D rb;
    private Animator animator;

    // Track whether Qurd is on the ground
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // If Qurd is dying, don't process other movement/attacks.
        // (Dying logic can disable further control.)

        // Update timers
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        // Perform a simple ground check using an OverlapCircle.
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // Process movement input.
        // (For an enemy AI, replace these Input calls with your movement logic.)
        float horizontal = Input.GetAxis("Horizontal");

        // Update horizontal velocity (we use rb.velocity for a 2D physics-based approach).
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        // Update the animation "Speed" parameter to switch between idle and run.
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        // Flip Qurd's sprite horizontally if needed (assuming left-facing is default).
        if (horizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (horizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        // Jumping logic:
        // We check that Qurd is on the ground and the jump button was pressed.
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            // Trigger the jump animation.
            animator.SetTrigger("Jump");
        }

        // Attack logic:
        // Here we use a key (e.g. "J") to trigger an attack.
        // When integrating with AI, you might call the Attack() method directly.
        if (!isAttacking && attackTimer <= 0f && Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(Attack());
        }
    }

    // Example coroutine to manage attack timing and animation.
    IEnumerator Attack()
    {
        isAttacking = true;
        // Trigger the attack animation.
        animator.SetTrigger("Attack");

        // (Optional) Wait for the attack animation duration:
        // You might replace this with an animation event instead.
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        attackTimer = attackCooldown;
    }

    // When Qurd dies, call this method to trigger the death animation and disable further control.
    public void Die()
    {
        // Trigger the death animation.
        animator.SetTrigger("Die");
        // Disable physics so Qurd stops moving.
        rb.linearVelocity = Vector2.zero;
        // Optionally disable further script activity (or the collider) here.
        this.enabled = false;
    }

    // Optional: Visualize the groundCheck sphere in the Unity Editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }
    }
}
