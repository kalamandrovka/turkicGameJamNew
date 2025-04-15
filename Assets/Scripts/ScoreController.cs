using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public int feather = 0;
    public int collectibles = 0;
    private const int maxCollectibles = 5;
    private bool isUltimateReady;

    public Slider powerBar;
    public Image fillImage;
    public Sprite normalFillSprite;  // Default fill sprite
    public Sprite fullFillSprite;    // Sprite when bar is full

    void Start()
    {
        InitializePowerBar();
    }

    void InitializePowerBar()
    {
        powerBar.maxValue = maxCollectibles;
        powerBar.value = 0;
        isUltimateReady = false;
        fillImage.sprite = normalFillSprite; // Set default sprite
    }

    public void AddPowerUp()
    {
        collectibles = Mathf.Clamp(collectibles + 1, 0, maxCollectibles);
        UpdatePowerBar();

        if (collectibles >= maxCollectibles)
        {
            ActivateUltimateReadyState();
        }
        else
        {
            // Revert to normal sprite if not full
            fillImage.sprite = normalFillSprite;
        }
    }

    void ActivateUltimateReadyState()
    {
        fillImage.sprite = fullFillSprite; // Change to full sprite
        isUltimateReady = true;
    }

    void UpdatePowerBar()
    {
        powerBar.value = collectibles;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Feather"))
        {
            UpdateFeatherScore();
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Collectibles"))
        {
            AddPowerUp();
            
            Destroy(collision.gameObject);
        }
    }

    public void UpdateFeatherScore()
    {
        feather++;
    }
}