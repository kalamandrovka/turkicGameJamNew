using UnityEngine;

public class BulletShooter : MonoBehaviour
{
    public GameObject bulletPrefab; // Assign your bullet prefab in the Inspector
    public Transform spawnPoint;    // Assign the spawn location (empty GameObject)
    public float bulletSpeed = 10f;
    public Transform player;        // Assign the player's transform

    void Update()
    {
        
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
            rb.velocity = shootDirection * bulletSpeed;
        }
        else
        {
            Debug.LogError("Bullet prefab is missing Rigidbody2D component!");
        }

        // Optional: Rotate bullet to face direction (if needed)
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}