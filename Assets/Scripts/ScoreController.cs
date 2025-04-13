using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public int feather = 0; // Player's score

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        
    }


    public void UpdateScore()
    {
        feather++;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Collectibles") && (collision.gameObject)))
        {
            UpdateScore();
            Destroy(collision.gameObject);
        }
    }

    
}
