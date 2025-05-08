// d:\Unity\FlappyBird\Assets\Asset\Script\SkinManager.cs
using UnityEngine;
using System.Collections.Generic; // Required for List
using System.Linq; // Required for LINQ methods like FirstOrDefault

public class SkinManager : MonoBehaviour
{
    public static SkinManager instance; // Singleton instance

    [Header("Skin Configuration")]
    [Tooltip("List of all available skin ScriptableObjects.")]
    [SerializeField] private List<SO_Skins> availableSkins = new List<SO_Skins>();

    [Header("Player References")]
    [Tooltip("Reference to the FlappyBehavior script on the player GameObject.")]
    [SerializeField] private FlappyBehavior playerFlappyBehavior; // Assign your player GameObject here

    // Player components cache
    private SpriteRenderer playerSpriteRenderer;
    private Animator playerAnimator;

    // PlayerPrefs key
    private const string SelectedSkinPrefKey = "SelectedSkinID";
    private const int DefaultSkinID = 0; // Define the ID for the default skin

    // Currently applied skin data
    public SO_Skins CurrentSkin { get; private set; }

    private void Awake()
    {
        // --- Singleton Pattern ---
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // Optional: If SkinManager needs to persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // --- End Singleton ---

        // --- Validate Player Reference ---
        if (playerFlappyBehavior == null)
        {
            Debug.LogError("SkinManager: Player FlappyBehavior reference not set in the Inspector!", this.gameObject);
            this.enabled = false; // Disable if player isn't set
            return;
        }

        // --- Get Player Components ---
        playerSpriteRenderer = playerFlappyBehavior.GetComponent<SpriteRenderer>();
        playerAnimator = playerFlappyBehavior.GetComponent<Animator>();

        if (playerSpriteRenderer == null)
        {
            Debug.LogError("SkinManager: Player is missing a SpriteRenderer component!", playerFlappyBehavior.gameObject);
            this.enabled = false;
            return;
        }
        if (playerAnimator == null)
        {
            Debug.LogError("SkinManager: Player is missing an Animator component!", playerFlappyBehavior.gameObject);
            // Decide if this is critical. Maybe some skins don't need animators?
            // For now, let's assume it's needed.
            this.enabled = false;
            return;
        }
        // --- End Component Checks ---


        // --- Load Saved Skin ---
        LoadAndApplySkin();
    }

    // Start is called before the first frame update
    // void Start() { } // Awake is sufficient for initial setup

    /// <summary>
    /// Loads the selected skin ID from PlayerPrefs and applies the corresponding skin.
    /// Falls back to the default skin if the saved ID is invalid or not found.
    /// </summary>
    private void LoadAndApplySkin()
    {
        int selectedID = PlayerPrefs.GetInt(SelectedSkinPrefKey, DefaultSkinID);
        SO_Skins skinToApply = GetSkinByID(selectedID);

        // Fallback if saved ID is invalid or skin doesn't exist in the list anymore
        if (skinToApply == null)
        {
            Debug.LogWarning($"SkinManager: Saved skin ID {selectedID} not found in available skins. Falling back to default (ID {DefaultSkinID}).");
            skinToApply = GetSkinByID(DefaultSkinID);

            // If even the default skin is missing (major setup error)
            if (skinToApply == null)
            {
                 Debug.LogError($"SkinManager: Default skin with ID {DefaultSkinID} is missing from the availableSkins list! Cannot apply any skin.", this.gameObject);
                 this.enabled = false;
                 return;
            }
        }

        ApplySkin(skinToApply);
    }

    /// <summary>
    /// Applies the visual properties (Sprite, Animator Controller) of the given skin
    /// to the player GameObject.
    /// </summary>
    /// <param name="skin">The SO_Skins data to apply.</param>
    private void ApplySkin(SO_Skins skin)
    {
        if (skin == null)
        {
            Debug.LogError("SkinManager: Attempted to apply a null skin.", this.gameObject);
            return;
        }

        if (playerSpriteRenderer == null || playerAnimator == null)
        {
             Debug.LogError("SkinManager: Player components are missing, cannot apply skin.", this.gameObject);
             return;
        }

        // Apply Sprite
        if (skin.SkinSprite != null)
        {
            playerSpriteRenderer.sprite = skin.SkinSprite;
        }
        else
        {
            Debug.LogWarning($"SkinManager: Skin '{skin.SkinName}' (ID: {skin.SkinID}) is missing a SkinSprite.", skin);
        }

        // Apply Animator Controller
        if (skin.AnimatorController != null)
        {
            playerAnimator.runtimeAnimatorController = skin.AnimatorController;
        }
        else
        {
            // Decide what to do if no controller: Use a default? Disable animator? Log warning?
            // playerAnimator.runtimeAnimatorController = null; // Or assign a default controller
            Debug.LogWarning($"SkinManager: Skin '{skin.SkinName}' (ID: {skin.SkinID}) is missing an AnimatorController.", skin);
        }

        CurrentSkin = skin; // Update the currently applied skin reference
        Debug.Log($"SkinManager: Applied skin '{CurrentSkin.SkinName}' (ID: {CurrentSkin.SkinID}).");
    }

    /// <summary>
    /// Selects a skin by its ID, saves the selection to PlayerPrefs,
    /// and immediately applies it to the player.
    /// (This would typically be called from a UI button).
    /// </summary>
    /// <param name="skinID">The ID of the skin to select and apply.</param>
    public void SelectAndApplySkin(int skinID)
    {
        SO_Skins selectedSkin = GetSkinByID(skinID);

        if (selectedSkin != null)
        {
            PlayerPrefs.SetInt(SelectedSkinPrefKey, skinID);
            PlayerPrefs.Save(); // Make sure to save the changes
            ApplySkin(selectedSkin);
        }
        else
        {
            Debug.LogWarning($"SkinManager: Attempted to select non-existent skin with ID {skinID}.");
        }
    }

    /// <summary>
    /// Finds and returns the SO_Skins object with the matching ID from the availableSkins list.
    /// </summary>
    /// <param name="skinID">The ID to search for.</param>
    /// <returns>The SO_Skins object if found, otherwise null.</returns>
    public SO_Skins GetSkinByID(int skinID)
    {
        // Using LINQ for a concise search
        return availableSkins.FirstOrDefault(skin => skin.SkinID == skinID);
    }

    /// <summary>
    /// Returns the list of all available skins (e.g., for populating a UI).
    /// </summary>
    public List<SO_Skins> GetAllSkins()
    {
        return availableSkins;
    }
}
