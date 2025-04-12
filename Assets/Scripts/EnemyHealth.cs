using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private int hitCount = 0;      // Number of hits taken.
    private int hitLimit = 2;      // Enemy dies after 2 hits.
    Animator animator;

    public void TakeDamage(int damage)
    {
        animator.SetBool("Hurt", true);
        StartCoroutine(NoHurt());
        hitCount += damage; // Each attack adds one hit.
        Debug.Log("Enemy hit! Total hits: " + hitCount);
        if (hitCount >= hitLimit)
        {
            Die();
        }
    }
    IEnumerator NoHurt()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Hurt", false);
    }
    void Die()
    {
        Debug.Log("Enemy died");
        // Optionally play a death animation here before destroying.
        Destroy(gameObject);
    }
}
