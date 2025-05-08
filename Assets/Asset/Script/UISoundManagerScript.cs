using UnityEngine;

public class UISoundManagerScript : MonoBehaviour
{
    public static UISoundManagerScript instance; // Singleton instance

    [Header("Audio Source")]
    [Tooltip("The AudioSource component dedicated to playing UI sound effects.")]
    [SerializeField] private AudioSource uiAudioSource;

    [Header("Default Sounds")]
    [Tooltip("The default sound to play for button clicks.")]
    [SerializeField] private AudioClip defaultClickSound;
    // Add more default sounds here if needed (e.g., hover, error)
    // [SerializeField] private AudioClip defaultHoverSound;

    void Awake()
    {
        // --- Singleton Pattern ---
        if (instance == null)
        {
            instance = this;
            // Optional: Keep the manager alive across scene loads
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If another instance already exists, destroy this one
            Destroy(gameObject);
            return;
        }
        // --- End Singleton ---

        // Validate the AudioSource reference
        if (uiAudioSource == null)
        {
            Debug.LogError("UISoundManager: UI Audio Source is not assigned in the Inspector!", this.gameObject);
            // Try to find one on the same GameObject as a fallback
            uiAudioSource = GetComponent<AudioSource>();
            if (uiAudioSource == null)
            {
                Debug.LogError("UISoundManager: Could not find AudioSource component. Please assign or add one.", this.gameObject);
                this.enabled = false; // Disable script if no source
            }
        }

        // Ensure the source doesn't play on awake
        if (uiAudioSource != null)
        {
            uiAudioSource.playOnAwake = false;
        }
    }

    /// <summary>
    /// Plays the default button click sound.
    /// </summary>
    public void PlayClickSound()
    {
        PlaySound(defaultClickSound);
    }

    /// <summary>
    /// Plays a specific audio clip using the UI AudioSource.
    /// </summary>
    /// <param name="clipToPlay">The AudioClip to play.</param>
    public void PlaySound(AudioClip clipToPlay)
    {
        if (uiAudioSource != null && clipToPlay != null)
        {
            uiAudioSource.PlayOneShot(clipToPlay);
        }
        else
        {
            if (uiAudioSource == null) Debug.LogWarning("UISoundManager: Cannot play sound, UI Audio Source is missing.");
            if (clipToPlay == null) Debug.LogWarning("UISoundManager: Cannot play sound, AudioClip is null.");
        }
    }
}
