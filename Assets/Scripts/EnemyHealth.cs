using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private int hitCount = 0;      // Number of hits taken.
    private int hitLimit = 2;      // Enemy dies after 2 hits.
    Animator animator;
    public GameObject bloodEffectPrefab;
    public GameObject bloodEffectPrefab2; // Second blood effect prefab.

    // The duration in seconds after which the blood effect will be destroyed.
    public float effectDuration = 2.0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SpawnBloodEffect(Vector3 position)
    {
        if (bloodEffectPrefab != null)
        {
            // Instantiate the blood effect prefab at the specified position with no rotation.
            GameObject instance = Instantiate(bloodEffectPrefab, position, Quaternion.identity);
            GameObject instance2 = Instantiate(bloodEffectPrefab2, position + new Vector3(0.05f, 0.01f, 0f), Quaternion.identity);


            // Destroy the instance after 'effectDuration' seconds.
            Destroy(instance, effectDuration);
            Destroy(instance2, effectDuration);
        }
        else
        {
            Debug.LogWarning("BloodEffectPrefab is not assigned in the inspector.");
        }
    }

    public void TakeDamage(int damage)
    {
        animator.SetBool("Hurt", true);
        StartCoroutine(NoHurt());
        SpawnBloodEffect(transform.position /*+ new Vector3(0.2f,0.1f, 0f)*/);
        hitCount += damage; // Each attack adds one hit.
        Debug.Log("Enemy hit! Total hits: " + hitCount);
        if (hitCount >= hitLimit)
        {
            animator.SetBool("Die", true);
            StartCoroutine(DieAnimator());
        }
    }
    IEnumerator NoHurt()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Hurt", false);
    }
    IEnumerator DieAnimator()
    {
        yield return new WaitForSeconds(1.3f);
        Die();
        
    }
    void Die()
    {
        Debug.Log("Enemy died");
        // Optionally play a death animation here before destroying.
        Destroy(gameObject);
    }
}
