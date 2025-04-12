using UnityEngine;
using System.Collections;

/// <summary>
/// A simple Enemy Creature script demonstrating how to trigger
/// multiple animations: Idle, Walk, Attack, Hurt, Death, Spell,
/// Attack-NoEffect, Hurt-NoEffect, Death-NoEffect, Spell-NoEffect.
/// This example includes basic chasing logic and a simple health system.
/// Adapt to your own game flow and Animator transitions.
/// </summary>
public class CreatureAI : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;         // Maximum health
    private int currentHealth;
    private bool isDead = false;

    [Header("Movement")]
    public float moveSpeed = 2f;      // Movement speed while walking/chasing
    public float senseRange = 5f;     // Range to sense the player
    public float attackRange = 1.5f;  // Range to trigger an attack
    private bool isFacingRight = true;

    [Header("Spell Settings")]
    public float spellCooldown = 5f;  // Time between spells
    private float spellTimer = 0f;

    [Header("References")]
    public Transform player;          // Assign your player's transform here
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // If the creature is dead, do nothing.
        if (isDead) return;

        // Basic countdown for deciding when to cast a spell.
        if (spellTimer > 0f)
            spellTimer -= Time.deltaTime;

        // Basic "AI": check distance to player.
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= senseRange)
        {
            // Move toward the player.
            MoveTowardPlayer();

            // If close enough to attack, do so.
            if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
            // Otherwise, occasionally cast spells if the timer allows.
            else if (spellTimer <= 0f)
            {
                Spell();
                spellTimer = spellCooldown;
            }
        }
        else
        {
            // If the player is out of range, stay idle.
            animator.Play("Idle");
            rb.linearVelocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Moves the creature toward the player, switching to the "Walk" animation.
    /// </summary>
    private void MoveTowardPlayer()
    {
        // Switch to "Walk" animation if not already in an attack or special state.
        animator.Play("Walk");

        // Determine direction to the player.
        Vector2 direction = (player.position - transform.position).normalized;

        // Move in that direction.
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        // Flip sprite based on direction.x.
        if (direction.x > 0 && !isFacingRight) Flip();
        else if (direction.x < 0 && isFacingRight) Flip();
    }

    /// <summary>
    /// Performs a normal Attack (for a successful strike) by triggering
    /// the "Attack" animation from Any State.
    /// </summary>
    private void Attack()
    {
        // Stop movement
        rb.linearVelocity = Vector2.zero;

        // Trigger the Attack animation
        animator.SetTrigger("Attack");
    }

    /// <summary>
    /// Performs an Attack with No Effect (if, for example, the creature missed
    /// or the player is invulnerable).
    /// </summary>
    public void AttackNoEffect()
    {
        animator.SetTrigger("Attack-NoEffect");
    }

    /// <summary>
    /// Triggers the "Spell" animation from Any State.
    /// </summary>
    private void Spell()
    {
        // You might also stop movement or only cast spells if certain conditions are met.
        animator.SetTrigger("Spell");
    }

    /// <summary>
    /// Triggers a "Spell-NoEffect" animation for cases where a spell fizzles.
    /// </summary>
    public void SpellNoEffect()
    {
        animator.SetTrigger("Spell-NoEffect");
    }

    /// <summary>
    /// Called when the creature takes damage. If the health drops below 1,
    /// it calls Die(). Otherwise it triggers a "Hurt" animation.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth > 0)
        {
            animator.SetTrigger("Hurt");
        }
        else
        {
            Die();
        }
    }

    /// <summary>
    /// If the creature takes damage with no effect (e.g., the player has a weak weapon),
    /// you might trigger "Hurt-NoEffect".
    /// </summary>
    public void TakeDamageNoEffect()
    {
        if (!isDead)
            animator.SetTrigger("Hurt-NoEffect");
    }

    /// <summary>
    /// Kills the creature, triggering the "Death" animation.
    /// </summary>
    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Death");
        // Optionally: disable collisions, AI logic, etc.
    }

    /// <summary>
    /// If the creature dies but it has no effect (for example, a special scenario),
    /// you can call this method to trigger "Death-NoEffect".
    /// </summary>
    public void DieNoEffect()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Death-NoEffect");
    }

    /// <summary>
    /// Flips the sprite horizontally.
    /// </summary>
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
