using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    // Attack settings
    public float hardAttackThreshold = 2f; // Hold duration (in seconds) required for a hard attack
    public float lightAttackDuration = 0.5f; // How long the light attack bool remains true
    public float hardAttackDuration = 1.0f;  // How long the hard attack bool remains true

    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;

    private float attackButtonHoldTime = 0f;
    private bool isHoldingAttack = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
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

        // Update walking animation values
        animator.SetFloat("walking", Mathf.Abs(moveInput));
        animator.SetBool("walking", moveInput != 0 && isGrounded);
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
        // Start timing when the attack key is first pressed
        if (Input.GetKeyDown(KeyCode.J))
        {
            isHoldingAttack = true;
            attackButtonHoldTime = 0f;
            Debug.Log("Attack key pressed.");
        }

        if (Input.GetKey(KeyCode.J))
        {
            attackButtonHoldTime += Time.deltaTime;
        }

        // On releasing the attack key, check the hold time to determine attack type
        if (Input.GetKeyUp(KeyCode.J))
        {
            isHoldingAttack = false;
            if (attackButtonHoldTime >= hardAttackThreshold)
            {
                // Hard attack
                Debug.Log("Hard attack triggered. Held for: " + attackButtonHoldTime + " seconds.");
                animator.SetBool("HardAttack", true);
                StartCoroutine(ResetAttack("HardAttack", hardAttackDuration));
            }
            else
            {
                // Light attack
                Debug.Log("Light attack triggered. Held for: " + attackButtonHoldTime + " seconds.");
                animator.SetBool("LightAttack", true);
                StartCoroutine(ResetAttack("LightAttack", lightAttackDuration));
            }
            attackButtonHoldTime = 0f;
        }
    }

    // Coroutine to reset the attack bool after a set duration
    IEnumerator ResetAttack(string attackType, float duration)
    {
        yield return new WaitForSeconds(duration);
        animator.SetBool(attackType, false);
        Debug.Log(attackType + " reset after " + duration + " seconds.");
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
