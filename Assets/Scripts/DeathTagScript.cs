using UnityEngine;

public class DeathTagScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("DeathTag") && (collision.gameObject)))
        {

            Die();
        }
    }

    void Die()
    {
        if (PlayerController.LastTeleportPosition != Vector2.zero)
        {
            transform.position = PlayerController.LastTeleportPosition;
        }
        else
        {
            transform.position = Vector2.zero;
        }

    }
}
