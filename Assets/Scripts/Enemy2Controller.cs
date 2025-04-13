using UnityEngine;
using System.Collections;

public class Enemy2Controller : MonoBehaviour
{
    // ------------------------------
    // Health Settings (Hit Count)
    // ------------------------------
    //private int hitCount = 0;          // Number of hits taken.
    //public int hitLimit = 2;           // Enemy dies after 2 hits.
    //private bool isDead = false;


    public GameObject bulletPrefab; // Assign your bullet prefab in the Inspector
    public Transform spawnPoint;    // Assign the spawn location (empty GameObject)
    public float bulletSpeed = 10f;

    // ------------------------------
    // Movement & AI Settings
    // ------------------------------
    public float moveSpeed = 2f;       // Movement speed while chasing.
    public float senseRange = 6f;      // Distance within which the enemy senses the player.
    public float attackRange = 1.5f;   // Range within which the enemy attacks.
    public float attackCooldown = 1f;  // Cooldown (in seconds) between attacks.
    private bool isAttacking = false;
    private bool isFacingRight = true; // Tracks the enemy's facing direction.

    // ------------------------------
    // References
    // ------------------------------
    public Transform player;         // Assign the player's transform in the Inspector.
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        /*if (isDead)
            return;*/

        // Calculate the horizontal distance between enemy and player.
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // If the player is within sense range, start moving or attacking.
        if (distanceToPlayer <= senseRange)
        {
            // If outside attack range, chase the player.
            if (distanceToPlayer > attackRange)
            {
                ChasePlayer();
            }
            else // Within attack range: attack.
            {
                if (!isAttacking)
                    StartCoroutine(Attack());
            }
        }
        else
        {
            // Player is out of range: enemy idles.
            Idle();
        }
    }

    /// <summary>
    /// Chases the player by setting velocity based on a fixed moveSpeed
    /// and turns on the "Walking" boolean.
    /// </summary>
    void ChasePlayer()
    {
        animator.SetBool("Walking", true);
        // Determine normalized horizontal direction toward the player.
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        // Flip sprite if needed.
        if (direction.x < 0 && !isFacingRight)
            Flip();
        else if (direction.x > 0 && isFacingRight)
            Flip();

        // Ensure the enemy is not attacking.
        animator.SetBool("Attack", false);
    }

    /// <summary>
    /// Sets the enemy to idle: stops movement and turns off "Walking" and "Attack".
    /// </summary>
    void Idle()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("Walking", false);
        animator.SetBool("Attack", false);
    }

    /// <summary>
    /// Performs an attack by stopping movement, turning on "Attack",
    /// waiting for a brief moment (to simulate attack timing), dealing damage,
    /// and then waiting for the cooldown.
    /// </summary>
    IEnumerator Attack()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("Attack", true);
        Shoot();

        // Adjust this delay to match the animation's hit frame
        yield return new WaitForSeconds(0.1f);

        // Directly apply damage without rechecking distance
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeHit();
        }

        yield return new WaitForSeconds(attackCooldown);
        animator.SetBool("Attack", false);
        isAttacking = false;


    }


    public void Shoot()
    {
        if (player == null || bulletPrefab == null) return;

        // Create bullet at spawn point
        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);

        // Calculate direction to player
        Vector2 shootDirection = (player.position - spawnPoint.position).normalized;

        // Apply velocity to bullet
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = shootDirection * bulletSpeed;
        }
        else
        {
            Debug.LogError("Bullet prefab is missing Rigidbody2D component!");
        }

        // Optional: Rotate bullet to face direction (if needed)
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /// <summary>
    /// Called externally (for example, by a player's hit) to apply damage.
    /// Triggers the Hurt boolean, and if hit count reaches the limit, dies.
    /// </summary>
    /*public void TakeDamage(int damage) // -----------------------------------------------------------
    {
        if (isDead)
            return;

        hitCount += damage;
        Debug.Log("Enemy hit! Total hits: " + hitCount);

        // Trigger Hurt animation.
        animator.SetBool("Hurt", true);
        StartCoroutine(ResetHurt());

        if (hitCount >= hitLimit)
        {
            Die();
        }
    }*/

    /// <summary>
    /// Resets the Hurt boolean after a short delay.
    /// </summary>
   /* IEnumerator ResetHurt()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Hurt", false);
    }*/

    /// <summary>
    /// Handles enemy death by stopping movement and setting the "Die" boolean.
    /// </summary>
    /*void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("Die", true);
        Debug.Log("Enemy died!");
        // Optionally destroy the enemy after a delay:
        Destroy(gameObject, 1.5f);
    }*/

    /// <summary>
    /// Flips the enemy's sprite horizontally.
    /// </summary>
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
