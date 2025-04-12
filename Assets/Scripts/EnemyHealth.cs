using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private int hitCount = 0;      // Number of hits taken.
    private int hitLimit = 2;      // Enemy dies after 2 hits.

    public void TakeDamage(int damage)
    {
        hitCount += damage; // Each attack adds one hit.
        Debug.Log("Enemy hit! Total hits: " + hitCount);
        if (hitCount >= hitLimit)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died");
        // Optionally play a death animation here before destroying.
        Destroy(gameObject);
    }
}
