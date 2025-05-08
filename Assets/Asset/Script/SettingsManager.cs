// d:\Unity\FlappyBird\Assets\Asset\Script\SettingsManager.cs
using UnityEngine;
using UnityEngine.Audio; // Required for AudioMixer

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance; // Singleton instance

    [Header("Audio Mixer")]
    [Tooltip("The AudioMixer that controls the game's audio levels. Should be the same one used in SettingsScript.")]
    [SerializeField] private AudioMixer audioMixer;

    // --- Publicly Accessible Volume Levels (Read-Only) ---
    // These store the loaded linear volume (0.0 to 1.0)
    public float MasterVolume { get; private set; }
    public float MusicVolume { get; private set; } // Renamed from MasterVolume
    public float SfxVolume { get; private set; }

    // --- Constants (Match these with SettingsScript.cs) ---
    // PlayerPrefs Keys
    private const string MASTER_VOLUME_KEY = "MasterVolume"; // Added for Master Volume
    private const string MUSIC_VOLUME_KEY = "MusicVolume"; // Renamed from MASTER_VOLUME_KEY
    private const string SFX_VOLUME_KEY = "SfxVolume";

    // Mixer Parameter Names (Must match the exposed parameters in your AudioMixer asset)
    private const string MIXER_MASTER_VOLUME = "MasterVolume"; // Added for Master Volume (Ensure this exists in your Mixer!)
    private const string MIXER_MUSIC_VOLUME = "MusicVolume";
    private const string MIXER_SFX_VOLUME = "SfxVolume";

    // Default volume if nothing is saved yet (0.0 to 1.0 range)
    private const float DEFAULT_VOLUME = 0.75f;

    void Awake()
    {
        // --- Singleton Pattern ---
        if (instance == null)
        {
            instance = this;
            // Optional: Make this manager persist across scene loads
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If another instance already exists, destroy this one
            Destroy(gameObject);
            return;
        }
        // --- End Singleton ---

        // --- Validate AudioMixer ---
        if (audioMixer == null)
        {
            Debug.LogError("SettingsManager: Audio Mixer is not assigned in the Inspector!", this.gameObject);
            // Disable the script if the core dependency is missing
            this.enabled = false;
            return;
        }

        // --- Load and Apply Settings ---
        // Load settings from PlayerPrefs and immediately apply them to the mixer
        LoadAndApplySettings();
    }

    /// <summary>
    /// Loads volume settings from PlayerPrefs, stores them in public properties,
    /// and applies them to the assigned AudioMixer.
    /// </summary>
    public void LoadAndApplySettings()
    {
        // Load values from PlayerPrefs, using defaults if keys don't exist
        MasterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, DEFAULT_VOLUME); // Load Master Volume
        MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, DEFAULT_VOLUME);
        SfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, DEFAULT_VOLUME);

        // Update log message
        Debug.Log($"SettingsManager Loaded: Master={MasterVolume}, Music={MusicVolume}, SFX={SfxVolume}");

        // Apply the loaded values to the AudioMixer
        // Apply Master Volume first, as Music/SFX groups are often children of a Master group
        ApplyVolumeToMixer(MIXER_MASTER_VOLUME, MasterVolume);
        // Use the renamed mixer parameter constant
        ApplyVolumeToMixer(MIXER_MUSIC_VOLUME, MusicVolume);
        ApplyVolumeToMixer(MIXER_SFX_VOLUME, SfxVolume);
    }

    /// <summary>
    /// Converts a linear volume value (0-1) to decibels (dB) and sets it
    /// on the specified AudioMixer parameter.
    /// </summary>
    /// <param name="mixerParameterName">The exact name of the exposed parameter in the AudioMixer.</param>
    /// <param name="linearVolume">The volume value from 0.0 to 1.0.</param>
    private void ApplyVolumeToMixer(string mixerParameterName, float linearVolume)
    {
        if (audioMixer == null)
        {
            Debug.LogError($"SettingsManager: Cannot apply volume for '{mixerParameterName}', AudioMixer is not assigned.", this.gameObject);
            return;
        }

        // Convert linear (0-1) to dB (-80 to 0). Log(0) is undefined, so clamp near zero.
        // Use a small minimum value to avoid Mathf.Log10(0).
        float dB = (linearVolume > 0.0001f) ? Mathf.Log10(linearVolume) * 20f : -80f;

        // Set the value on the mixer
        bool success = audioMixer.SetFloat(mixerParameterName, dB);

        if (!success)
        {
            Debug.LogWarning($"SettingsManager: Failed to set AudioMixer parameter '{mixerParameterName}'. Ensure this parameter is exposed in the AudioMixer asset and the name matches exactly.", this.gameObject);
        }
        // else {
        //     Debug.Log($"SettingsManager Applied: {mixerParameterName} set to {dB} dB (Linear: {linearVolume})");
        // }
    }

    // --- How other classes use this ---
    // Other classes generally DON'T need to directly read MasterVolume, MusicVolume or SfxVolume.
    // Instead, they should rely on their AudioSource components having their
    // "Output" field correctly assigned to the appropriate AudioMixerGroup (e.g., Music, SFX).
    // This SettingsManager ensures the *Mixer Groups* have the correct volume applied based on saved settings.

    // Example of how another script *could* read the value (e.g., for display purposes):
    /*
    void SomeOtherScriptMethod()
    {
        if (SettingsManager.instance != null)
        {
            float currentMaster = SettingsManager.instance.MasterVolume;
            float currentMusic = SettingsManager.instance.MusicVolume;
            Debug.Log($"Current Volume Settings (Linear): Master={currentMaster}, Music={currentMusic}");
        }
    }
    */
}
