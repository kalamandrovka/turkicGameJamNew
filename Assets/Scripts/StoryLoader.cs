using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StoryLoader : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode advanceKey = KeyCode.Return; // Enter key
    [SerializeField] private KeyCode skipKey = KeyCode.Space;    // Spacebar
    [SerializeField] private float skipHoldDuration = 2f;        // Hold for 2 seconds to skip
    [SerializeField] private bool loopAtEnd = false;

    [Header("UI References")]
    [SerializeField] private List<GameObject> storyImages;
    [SerializeField] private GameObject skipPrompt;
    [SerializeField] private Image skipProgressBar; // Optional progress bar

    private int currentImageIndex = 0;
    private bool sequenceActive = false;
    private float skipHoldTimer = 0f;
    private bool isHoldingSkip = false;

    void Start()
    {
        // Validate the image list
        if (storyImages == null || storyImages.Count == 0)
        {
            Debug.LogError("No story images assigned!");
            enabled = false;
            return;
        }

        // Initialize all images
        for (int i = 0; i < storyImages.Count; i++)
        {
            if (storyImages[i] != null)
            {
                storyImages[i].SetActive(i == 0);
            }
        }

        sequenceActive = true;

        // Initialize UI
        if (skipPrompt != null) skipPrompt.SetActive(false);
        if (skipProgressBar != null) skipProgressBar.fillAmount = 0f;
    }

    void Update()
    {
        if (!sequenceActive) return;

        HandleSkipInput();
        HandleAdvanceInput();
    }

    void HandleAdvanceInput()
    {
        // Advance with Enter key
        if (Input.GetKeyDown(advanceKey))
        {
            AdvanceStory();
        }
    }

    void HandleSkipInput()
    {
        // Show skip prompt on any interaction
        if (skipPrompt != null && !skipPrompt.activeSelf &&
            (Input.anyKeyDown || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
        {
            skipPrompt.SetActive(true);
        }

        // Skip logic
        if (Input.GetKeyDown(skipKey))
        {
            isHoldingSkip = true;
        }

        if (Input.GetKeyUp(skipKey))
        {
            isHoldingSkip = false;
            skipHoldTimer = 0f;
            if (skipProgressBar != null) skipProgressBar.fillAmount = 0f;
        }

        if (isHoldingSkip)
        {
            skipHoldTimer += Time.deltaTime;

            // Update progress bar if available
            if (skipProgressBar != null)
            {
                skipProgressBar.fillAmount = skipHoldTimer / skipHoldDuration;
            }

            // Check if held long enough
            if (skipHoldTimer >= skipHoldDuration)
            {
                EndSequence();
            }
        }
    }

    public void AdvanceStory()
    {
        if (!sequenceActive) return;

        // Reset skip progress when advancing normally
        skipHoldTimer = 0f;
        if (skipProgressBar != null) skipProgressBar.fillAmount = 0f;

        // Deactivate current image
        if (currentImageIndex < storyImages.Count && storyImages[currentImageIndex] != null)
        {
            storyImages[currentImageIndex].SetActive(false);
        }

        // Move to next image
        currentImageIndex++;

        // Check if we've reached the end
        if (currentImageIndex >= storyImages.Count)
        {
            if (loopAtEnd)
            {
                currentImageIndex = 0;
            }
            else
            {
                EndSequence();
                return;
            }
        }

        // Activate next image
        if (storyImages[currentImageIndex] != null)
        {
            storyImages[currentImageIndex].SetActive(true);
        }
    }

    public void EndSequence()
    {
        sequenceActive = false;

        // Deactivate all images
        foreach (var image in storyImages)
        {
            if (image != null) image.SetActive(false);
        }

        // Deactivate UI elements
        if (skipPrompt != null) skipPrompt.SetActive(false);
        if (skipProgressBar != null) skipProgressBar.fillAmount = 0f;

        Debug.Log("Story sequence completed");
        // Add your end sequence logic here
    }
}
