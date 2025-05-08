// d:\Unity\FlappyBird\Assets\Asset\Script\GameplayMusicManager.cs
using UnityEngine;

// Ensure this GameObject always has an AudioSource component
[RequireComponent(typeof(AudioSource))]
public class GameplayMusicManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [Tooltip("The music to play during normal gameplay.")]
    [SerializeField] private AudioClip gameplayMusicClip; // Music 1

    [Tooltip("The music to play when the game is over.")]
    [SerializeField] private AudioClip gameOverMusicClip; // Music 2

    [Header("Settings")]
    [Tooltip("Should the main gameplay music loop?")]
    [SerializeField] private bool loopGameplayMusic = true;

    [Tooltip("Should the game over music loop?")]
    [SerializeField] private bool loopGameOverMusic = false; // Usually game over music doesn't loop

    // Reference to the AudioSource component on this GameObject
    private AudioSource audioSource;

    void Awake()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        // Optional: Configure AudioSource defaults if needed
        audioSource.playOnAwake = false; // We'll control playback manually
    }

    // Subscribe to the event when this component becomes active/enabled
    void OnEnable()
    {
        // Ensure the GameManager instance exists before subscribing
        if (GameManager.instance != null)
        {
            // Subscribe the HandleGameOver method to the OnGameOver event
            GameManager.instance.OnGameOver += HandleGameOver;
            Debug.Log("GameplayMusicManager subscribed to OnGameOver.");
        }
        else
        {
            Debug.LogWarning("GameplayMusicManager: GameManager instance not found on Enable! Cannot subscribe to OnGameOver.");
            // If GameManager might initialize later, you could try subscribing in Start() as a fallback,
            // but OnEnable is generally preferred for event subscription.
        }
    }

    // IMPORTANT: Unsubscribe when the component is disabled or destroyed to prevent errors/leaks
    void OnDisable()
    {
        // Check if the GameManager instance still exists before unsubscribing
        // (It might be destroyed before this object in some scene transitions)
        if (GameManager.instance != null)
        {
            // Unsubscribe the HandleGameOver method from the OnGameOver event
            GameManager.instance.OnGameOver -= HandleGameOver;
            Debug.Log("GameplayMusicManager unsubscribed from OnGameOver.");
        }
    }

    // Start is called once before the first frame update
    void Start()
    {
        // Start playing the main gameplay music when the game begins
        PlayMusic(gameplayMusicClip, loopGameplayMusic, "Gameplay");
    }

    /// <summary>
    /// This method is called automatically when GameManager invokes the OnGameOver event.
    /// </summary>
    private void HandleGameOver()
    {
        Debug.Log("HandleGameOver received in GameplayMusicManager.");
        // Stop current music and play the game over music
        PlayMusic(gameOverMusicClip, loopGameOverMusic, "Game Over");
    }

    /// <summary>
    /// Helper method to play a specific audio clip with looping options.
    /// </summary>
    /// <param name="clipToPlay">The AudioClip to play.</param>
    /// <param name="shouldLoop">Whether the clip should loop.</param>
    /// <param name="clipNameForLog">A descriptive name for logging purposes.</param>
    private void PlayMusic(AudioClip clipToPlay, bool shouldLoop, string clipNameForLog)
    {
        if (audioSource == null)
        {
            Debug.LogError("GameplayMusicManager: AudioSource component is missing!", this.gameObject);
            return;
        }

        if (clipToPlay != null)
        {
            audioSource.Stop(); // Stop any currently playing music
            audioSource.clip = clipToPlay; // Assign the new clip
            audioSource.loop = shouldLoop; // Set looping
            audioSource.Play(); // Play the new clip
            Debug.Log($"Playing {clipNameForLog} music.");
        }
        else
        {
            // If the requested clip is null, stop playback and log a warning.
            audioSource.Stop();
            Debug.LogWarning($"GameplayMusicManager: {clipNameForLog} music clip is not assigned in the inspector. Music stopped.");
        }
    }

    // Update is called once per frame (not needed for this logic, but keep the method stub)
    // void Update()
    // {
    //
    // }
}
