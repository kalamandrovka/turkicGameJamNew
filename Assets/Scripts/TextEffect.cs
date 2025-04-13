using UnityEngine;
using TMPro;
using System.Collections;

public class TextEffect : MonoBehaviour
{
    [Header("Typing Settings")]
    [SerializeField] private float charactersPerSecond = 20f;
    [SerializeField] private float punctuationDelayMultiplier = 5f;
    [SerializeField] private bool startOnEnable = true;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip typingSound;
    [SerializeField][Range(0, 1)] private float typingSoundVolume = 0.5f;
    [SerializeField] private float soundCooldown = 0.05f;

    private string fullText;
    private TMP_Text tmpTextComponent;
    private Coroutine typingCoroutine;
    private AudioSource audioSource;
    private float lastSoundPlayTime;
    private bool soundEnabled = true;

    void Awake()
    {
        tmpTextComponent = GetComponent<TMP_Text>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null && typingSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        fullText = tmpTextComponent.text;
        tmpTextComponent.text = "";
        tmpTextComponent.maxVisibleCharacters = 0;
    }

    void OnEnable()
    {
        if (startOnEnable)
        {
            StartTyping();
        }
    }

    public void StartTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText());
    }

    public void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            tmpTextComponent.maxVisibleCharacters = fullText.Length;
        }
    }

    IEnumerator TypeText()
    {
        tmpTextComponent.text = fullText;
        tmpTextComponent.maxVisibleCharacters = 0;

        int totalCharacters = fullText.Length;
        float baseDelay = 1f / charactersPerSecond;

        for (int i = 0; i <= totalCharacters; i++)
        {
            tmpTextComponent.maxVisibleCharacters = i;

            // Play typing sound with cooldown
            if (soundEnabled && typingSound != null && i < totalCharacters)
            {
                char currentChar = fullText[i];
                if (!char.IsWhiteSpace(currentChar) && Time.time - lastSoundPlayTime > soundCooldown)
                {
                    audioSource.PlayOneShot(typingSound, typingSoundVolume);
                    lastSoundPlayTime = Time.time;
                }
            }

            // Calculate delay for next character
            if (i > 0 && i < totalCharacters)
            {
                char currentChar = fullText[i];
                float delay = baseDelay;

                if (IsPunctuation(currentChar))
                {
                    delay *= punctuationDelayMultiplier;
                }

                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return new WaitForSeconds(baseDelay);
            }
        }

        typingCoroutine = null;
    }

    public void ToggleSound(bool enable)
    {
        soundEnabled = enable;
    }

    private bool IsPunctuation(char character)
    {
        return character == '.' || character == '!' || character == '?' ||
               character == ',' || character == ';' || character == ':';
    }

    public bool IsTyping()
    {
        return typingCoroutine != null;
    }
}
