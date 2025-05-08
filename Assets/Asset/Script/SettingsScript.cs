// d:\Unity\FlappyBird\Assets\Asset\Script\SettingsScript.cs
using UnityEngine;
using UnityEngine.UI; // Required for Slider and Button
using UnityEngine.Audio; // Required for AudioMixer
using TMPro; // Optional: If you want text labels for sliders

public class SettingsScript : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Slider to control the Master Volume.")]
    [SerializeField] private Slider masterVolumeSlider; // Added for Master Volume

    [Tooltip("Slider to control the Music Volume.")]
    [SerializeField] private Slider musicVolumeSlider; // Renamed from masterVolumeSlider

    [Tooltip("Slider to control the Sound Effects (SFX) Volume.")]
    [SerializeField] private Slider sfxVolumeSlider;

    [Tooltip("Button to save the current settings.")]
    [SerializeField] private Button saveButton;

    // Optional: Text labels to show slider values
    [Tooltip("Text to display the master volume value (Optional).")]
    [SerializeField] private TextMeshProUGUI masterVolumeLabel; // Added for Master Volume
    [Tooltip("Text to display the music volume value (Optional).")]
    [SerializeField] private TextMeshProUGUI musicVolumeLabel; // Renamed from masterVolumeLabel
    [Tooltip("Text to display the SFX volume value (Optional).")]
    [SerializeField] private TextMeshProUGUI sfxVolumeLabel;


    [Header("Visuals (Optional)")]
    [Tooltip("Optional sprite image 1 (e.g., for button state or icon).")]
    [SerializeField] private Sprite spriteImage1; // Example: Muted icon

    [Tooltip("Optional sprite image 2 (e.g., for button state or icon).")]
    [SerializeField] private Sprite spriteImage2; // Example: Unmuted icon


    [Header("Audio Mixer")]
    [Tooltip("The AudioMixer that controls the game's audio levels.")]
    [SerializeField] private AudioMixer audioMixer;

    // --- PlayerPrefs Keys (Constants to avoid typos) ---
    private const string MASTER_VOLUME_KEY = "MasterVolume"; // Added for Master Volume
    private const string MUSIC_VOLUME_KEY = "MusicVolume"; // Renamed from MASTER_VOLUME_KEY
    private const string SFX_VOLUME_KEY = "SfxVolume";

    // --- Mixer Parameter Names ---
    // IMPORTANT: These strings MUST exactly match the names of the parameters
    // you exposed in your AudioMixer asset for the Master, Music, and SFX groups.
    // Ensure "MasterVolume" exists and is exposed in your AudioMixer!
    private const string MIXER_MASTER_VOLUME = "MasterVolume"; // Added for Master Volume
    // Common names might be "MusicVolume" and "SfxVolume".
    private const string MIXER_MUSIC_VOLUME = "MusicVolume"; // Changed from MIXER_MASTER_VOLUME
    private const string MIXER_SFX_VOLUME = "SfxVolume";

    // Default volume if nothing is saved yet (0.0 to 1.0 range)
    private const float DEFAULT_VOLUME = 0.75f;

    void Start()
    {
        // --- Initialization ---
        // 1. Load saved settings or defaults
        LoadSettings();

        // 2. Add listeners to sliders to update audio mixer in real-time
        if (masterVolumeSlider != null)
        {
            // Call the new SetMasterVolume function
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }
        else
        {
            Debug.LogError("Master Volume Slider is not assigned in the SettingsScript inspector!", this.gameObject);
        }

        if (musicVolumeSlider != null)
        {
            // Call the renamed SetMusicVolume function
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        else
        {
            Debug.LogError("Music Volume Slider is not assigned in the SettingsScript inspector!", this.gameObject);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
        }
        else
        {
            Debug.LogError("SFX Volume Slider is not assigned in the SettingsScript inspector!", this.gameObject);
        }

        // 3. Add listener to the save button
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveSettings);
        }
        else
        {
            Debug.LogError("Save Button is not assigned in the SettingsScript inspector!", this.gameObject);
        }

        // 4. Initial UI Update (Labels)
        UpdateVolumeLabels();
    }

    /// <summary>
    /// Loads volume settings from PlayerPrefs and applies them to the sliders and AudioMixer.
    /// Uses default values if no saved settings are found.
    /// </summary>
    void LoadSettings()
    {
        // Load values from PlayerPrefs using the updated keys
        float masterVol = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, DEFAULT_VOLUME);
        float musicVol = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, DEFAULT_VOLUME);
        float sfxVol = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, DEFAULT_VOLUME);

        // Apply loaded values to sliders
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = masterVol;
        }
        if (musicVolumeSlider != null)

        {
            musicVolumeSlider.value = musicVol;
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = sfxVol;
        }

        // Apply loaded values to the AudioMixer IMMEDIATELY
        // Note: We call the Set...Volume methods which handle the dB conversion
        SetMasterVolume(masterVol); // Apply Master Volume
        SetMusicVolume(musicVol); // Call renamed function
        SetSfxVolume(sfxVol);

        Debug.Log($"Settings Loaded: Master={masterVol}, Music={musicVol}, SFX={sfxVol}");
    }

    /// <summary>
    /// Saves the current slider values to PlayerPrefs.
    /// Typically called by the Save Button.
    /// </summary>
    public void SaveSettings()
    {
        // Read values from the correct sliders
        float masterVol = masterVolumeSlider != null ? masterVolumeSlider.value : DEFAULT_VOLUME;
        float musicVol = musicVolumeSlider != null ? musicVolumeSlider.value : DEFAULT_VOLUME;
        float sfxVol = sfxVolumeSlider != null ? sfxVolumeSlider.value : DEFAULT_VOLUME;

        // Save using the updated keys
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, masterVol);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVol);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVol);
        PlayerPrefs.Save(); // Ensure data is written to disk

        Debug.Log($"Settings Saved: Master={masterVol}, Music={musicVol}, SFX={sfxVol}");

        // Optional: Provide feedback to the user
    }

    /// <summary>
    /// Sets the Music Volume on the AudioMixer based on the slider value.
    /// Converts linear slider value (0-1) to logarithmic decibels (dB).
    /// </summary>
    /// <param name="value">Linear volume value from the slider (0.0 to 1.0).</param>
    public void SetMasterVolume(float value) // Added this function
    {
        if (audioMixer != null)
        {
            // Convert linear (0-1) to dB (-80 to 0). Clamp near zero for Log10.
            float dB = (value > 0.0001f) ? Mathf.Log10(value) * 20f : -80f;
            // Use the master mixer parameter name
            bool success = audioMixer.SetFloat(MIXER_MASTER_VOLUME, dB);
            if (!success)
            {
                 Debug.LogWarning($"Failed to set AudioMixer parameter '{MIXER_MASTER_VOLUME}'. Ensure this parameter is exposed in the AudioMixer asset and the name matches exactly.", this.gameObject);
            }
        }
        else
        {
             Debug.LogWarning("AudioMixer is not assigned. Cannot set Master Volume.", this.gameObject);
        }
        UpdateVolumeLabels(); // Update text label if it exists
    }


    public void SetMusicVolume(float value) // Renamed from SetMasterVolume (Keep this one for Music)
    {
        if (audioMixer != null)
        {
            // Convert linear (0-1) to dB (-80 to 0). Clamp near zero for Log10.
            float dB = (value > 0.0001f) ? Mathf.Log10(value) * 20f : -80f;
            // Use the updated mixer parameter name
            bool success = audioMixer.SetFloat(MIXER_MUSIC_VOLUME, dB);
            if (!success)
            {
                 Debug.LogWarning($"Failed to set AudioMixer parameter '{MIXER_MUSIC_VOLUME}'. Ensure this parameter is exposed in the AudioMixer asset and the name matches exactly.", this.gameObject);
            }
        }
        else
        {
             Debug.LogWarning("AudioMixer is not assigned. Cannot set Music Volume.", this.gameObject);
        }
        UpdateVolumeLabels(); // Update text label if it exists
    }

    /// <summary>
    /// Sets the SFX Volume on the AudioMixer based on the slider value.
    /// Converts linear slider value (0-1) to logarithmic decibels (dB).
    /// </summary>
    /// <param name="value">Linear volume value from the slider (0.0 to 1.0).</param>
    public void SetSfxVolume(float value)
    {
        if (audioMixer != null)
        {
            // Convert linear (0-1) to dB (-80 to 0). Clamp near zero for Log10.
            float dB = (value > 0.0001f) ? Mathf.Log10(value) * 20f : -80f;
            // Use the SFX mixer parameter name
             bool success = audioMixer.SetFloat(MIXER_SFX_VOLUME, dB);
             if (!success)
             {
                 Debug.LogWarning($"Failed to set AudioMixer parameter '{MIXER_SFX_VOLUME}'. Ensure this parameter is exposed in the AudioMixer asset and the name matches exactly.", this.gameObject);
             }
        }
         else
        {
             Debug.LogWarning("AudioMixer is not assigned. Cannot set SFX Volume.", this.gameObject);
        }
        UpdateVolumeLabels(); // Update text label if it exists
    }

    /// <summary>
    /// Updates the optional text labels to show the current slider values (e.g., as percentages).
    /// </summary>
    private void UpdateVolumeLabels()
    {
        if (masterVolumeLabel != null && masterVolumeSlider != null)
        {
            masterVolumeLabel.text = $"{Mathf.RoundToInt(masterVolumeSlider.value * 100)}%";
        }
        // Update using the renamed variables
        if (musicVolumeLabel != null && musicVolumeSlider != null)
        {
            musicVolumeLabel.text = $"{Mathf.RoundToInt(musicVolumeSlider.value * 100)}%";
        }
        if (sfxVolumeLabel != null && sfxVolumeSlider != null)
        {
            sfxVolumeLabel.text = $"{Mathf.RoundToInt(sfxVolumeSlider.value * 100)}%";
        }
    }

    // Optional: Cleanup listeners when the object is destroyed or disabled
    void OnDestroy()
    {
        // Remove listeners using the renamed variables/functions
        if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume); // Added for Master
        if (musicVolumeSlider != null) musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.RemoveListener(SetSfxVolume);
        if (saveButton != null) saveButton.onClick.RemoveListener(SaveSettings);
    }

    // --- How to use the Sprites (Examples) ---
    // (Sprite usage examples remain the same, just adapt if needed for Music/SFX specific visuals)
    /*
    [SerializeField] private Image muteButtonImage;
    private bool isMusicMuted = false;

    public void ToggleMusicMute()
    {
        isMusicMuted = !isMusicMuted;
        float targetVolume = isMusicMuted ? 0.0001f : PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, DEFAULT_VOLUME);
        SetMusicVolume(targetVolume);
        if (!isMusicMuted && musicVolumeSlider != null) musicVolumeSlider.value = targetVolume;
        if (muteButtonImage != null)
        {
            muteButtonImage.sprite = isMusicMuted ? spriteImage1 : spriteImage2;
        }
    }
    */
}
