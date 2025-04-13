using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene loading
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
    [SerializeField] private UnityEngine.UI.Image skipProgressBar; // Optional progress bar

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

        // Initialize all images: only the first one is active
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
            (Input.anyKeyDown || Mathf.Abs(Input.GetAxis("Mouse X")) > 0 || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0))
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
            if (skipProgressBar != null)
                skipProgressBar.fillAmount = skipHoldTimer / skipHoldDuration;

            // If held long enough, end the sequence.
            if (skipHoldTimer >= skipHoldDuration)
            {
                EndSequence();
            }
        }
    }

    public void AdvanceStory()
    {
        if (!sequenceActive) return;

        // Reset skip progress when advancing
        skipHoldTimer = 0f;
        if (skipProgressBar != null) skipProgressBar.fillAmount = 0f;

        // Deactivate current image.
        if (currentImageIndex < storyImages.Count && storyImages[currentImageIndex] != null)
        {
            storyImages[currentImageIndex].SetActive(false);
        }

        // Increment index.
        currentImageIndex++;

        // Check if we've reached the end.
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

        // Activate the next image.
        if (storyImages[currentImageIndex] != null)
            storyImages[currentImageIndex].SetActive(true);
    }

    public void EndSequence()
    {
        sequenceActive = false;

        // Deactivate all images.
        foreach (var image in storyImages)
        {
            if (image != null) image.SetActive(false);
        }

        // Deactivate UI elements.
        if (skipPrompt != null) skipPrompt.SetActive(false);
        if (skipProgressBar != null) skipProgressBar.fillAmount = 0f;

        Debug.Log("Story sequence completed. Loading Main Scene...");

        // Load the Main Scene (replace "MainScene" with your actual scene name).
        SceneManager.LoadScene("main");
    }
}
