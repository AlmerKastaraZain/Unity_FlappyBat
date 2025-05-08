// d:\Unity\FlappyBird\Assets\Asset\Script\CharacterSelectionScript.cs
using UnityEngine;
using UnityEngine.UI; // Required for Image and Button
using TMPro; // Required for TextMeshProUGUI
using System.Collections.Generic; // Required for List
using System.Linq; // Required for LINQ methods like FirstOrDefault

public class CharacterSelectionScript : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Image component to display the current skin's preview.")]
    [SerializeField] private Image skinDisplayImage;
    [Tooltip("Text component to display the skin's name.")]
    [SerializeField] private TextMeshProUGUI skinNameText;
    [Tooltip("Text component to display the player current Highscore.")]
    [SerializeField] private TextMeshProUGUI highScoreText; // Displays player's overall high score
    [Tooltip("Text component to display the skin's description.")]
    [SerializeField] private TextMeshProUGUI skinDescriptionText;
    [Tooltip("Text component to display the skin's unlock status or requirement.")]
    [SerializeField] private TextMeshProUGUI statusRequirementText; // Renamed from costStatusText
    [Tooltip("Button to select the next skin.")]
    [SerializeField] private Button nextButton;
    [Tooltip("Button to select the previous skin.")]
    [SerializeField] private Button previousButton;
    // [Tooltip("Button to buy/unlock the current skin.")] // REMOVED Buy Button
    // [SerializeField] private Button buyButton; // REMOVED Buy Button
    [Tooltip("Button to equip the current skin.")]
    [SerializeField] private Button equipButton;

    // Image for equipButton
    [SerializeField] private Sprite equipButtonImage_Unequipped;
    [SerializeField] private Sprite equipButtonImage_equipped;

    [Header("Skin Data")]
    private List<SO_Skins> availableSkins;
    private int currentSkinIndex = 0;
    private int currentPlayerHighScore = 0; // Cache the high score

    // PlayerPrefs keys
    // Keep this in case you want other ways to unlock skins later (e.g., rewards)
    private const string SelectedSkinPrefKey = "SelectedSkinID";

    private const string OwnedSkinPrefKeyPrefix = "SkinOwned_";
    private const string HighScorePrefKey = "HighScore"; // Key used in GameManager

    void Start()
    {
                    PlayerPrefs.SetInt("HighScore", 100);
            PlayerPrefs.Save();

        // --- Initialization ---
        if (SkinManager.instance == null)
        {
            Debug.LogError("CharacterSelectionScript: SkinManager instance not found! Ensure SkinManager is active in the scene.", this.gameObject);
            this.enabled = false; // Disable script if SkinManager is missing
            return;
        }

        availableSkins = SkinManager.instance.GetAllSkins();

        if (availableSkins == null || availableSkins.Count == 0)
        {
            Debug.LogError("CharacterSelectionScript: No skins found in SkinManager!", this.gameObject);
            this.enabled = false;
            // Optionally disable buttons here
            if (nextButton) nextButton.interactable = false;
            if (previousButton) previousButton.interactable = false;
            // if (buyButton) buyButton.interactable = false; // REMOVED
            if (equipButton) equipButton.interactable = false;
            return;
        }

        // --- Get Player High Score ---
        currentPlayerHighScore = PlayerPrefs.GetInt(HighScorePrefKey, 0);
        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {currentPlayerHighScore}";
        }
        else
        {
            Debug.LogWarning("High Score Text not assigned in CharacterSelectionScript inspector.");
        }


        // --- Find Initial Skin ---
        // Start displaying the currently equipped skin
        SO_Skins equippedSkin = SkinManager.instance.CurrentSkin;
        if (equippedSkin != null)
        {
            currentSkinIndex = availableSkins.FindIndex(skin => skin.SkinID == equippedSkin.SkinID);
            if (currentSkinIndex == -1) // If equipped skin isn't in the list somehow, default to 0
            {
                Debug.LogWarning("Currently equipped skin not found in available list. Defaulting to index 0.");
                currentSkinIndex = 0;
            }
        }
        else
        {
            // If no skin is currently equipped (e.g., first launch), default to index 0
            currentSkinIndex = 0;
        }

        // --- Add Button Listeners ---
        // Ensure buttons are assigned in the inspector
        if (nextButton) nextButton.onClick.AddListener(NextSkin);
        else Debug.LogWarning("Next Button not assigned in CharacterSelectionScript inspector.");

        if (previousButton) previousButton.onClick.AddListener(PreviousSkin);
        else Debug.LogWarning("Previous Button not assigned in CharacterSelectionScript inspector.");

        // if (buyButton) buyButton.onClick.AddListener(BuySkin); // REMOVED
        // else Debug.LogWarning("Buy Button not assigned in CharacterSelectionScript inspector."); // REMOVED

        if (equipButton) equipButton.onClick.AddListener(EquipSkin);
        else Debug.LogWarning("Equip Button not assigned in CharacterSelectionScript inspector.");


        // --- Initial Display ---
        UpdateDisplay();
    }

    /// <summary>
    /// Cycles to the next available skin in the list.
    /// </summary>
    public void NextSkin()
    {
        if (availableSkins.Count == 0) return; // Should not happen if Start checks passed

        currentSkinIndex = (currentSkinIndex + 1) % availableSkins.Count; // Wrap around using modulo
        UpdateDisplay();
    }

    /// <summary>
    /// Cycles to the previous available skin in the list.
    /// </summary>
    public void PreviousSkin()
    {
        if (availableSkins.Count == 0) return;

        currentSkinIndex--;
        if (currentSkinIndex < 0)
        {
            currentSkinIndex = availableSkins.Count - 1; // Manual wrap around for negative index
        }
        UpdateDisplay();
    }

    // REMOVED BuySkin() method

    /// <summary>
    /// Equips the currently displayed skin if it's usable (meets requirements).
    /// </summary>
    public void EquipSkin()
    {
        if (availableSkins.Count == 0) return;

        SO_Skins skinToEquip = availableSkins[currentSkinIndex];

        // Check if the skin can be used based on requirements
        if (!CanUseSkin(skinToEquip))
        {
            Debug.LogWarning($"Cannot equip skin '{skinToEquip.SkinName}' because requirements are not met (High Score: {currentPlayerHighScore}, Required: {skinToEquip.ScoreToUnlock}).");
            // Optionally show a message to the player here
            return; // Should not happen if button is correctly disabled, but good safeguard
        }

        // Use SkinManager to select and apply the skin
        SkinManager.instance.SelectAndApplySkin(skinToEquip.SkinID);
        Debug.Log($"Equipped skin '{skinToEquip.SkinName}'.");

        UpdateDisplay(); // Refresh UI to update button states (e.g., disable Equip button for this skin)
    }


    /// <summary>
    /// Updates all UI elements based on the currently selected skin index and player high score.
    /// </summary>
    private void UpdateDisplay()
    {
        if (availableSkins == null || availableSkins.Count == 0 || currentSkinIndex < 0 || currentSkinIndex >= availableSkins.Count)
        {
            Debug.LogError("Cannot update display - invalid skin data or index.");
            // Optionally clear UI elements
            if (skinDisplayImage) skinDisplayImage.sprite = null;
            if (skinNameText) skinNameText.text = "Error";
            if (skinDescriptionText) skinDescriptionText.text = "No skins available.";
            if (statusRequirementText) statusRequirementText.text = "";
            if (highScoreText) highScoreText.text = "High Score: ?"; // Update high score display on error?
            // Disable buttons
            if (nextButton) nextButton.interactable = false;
            if (previousButton) previousButton.interactable = false;
            // if (buyButton) buyButton.interactable = false; // REMOVED
            if (equipButton) equipButton.interactable = false;
            return;
        }

        // Update High Score display (in case it changes while menu is open, though unlikely)
        // currentPlayerHighScore = PlayerPrefs.GetInt(HighScorePrefKey, 0); // Re-fetch if needed
        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {currentPlayerHighScore}";
        }

        SO_Skins currentSkin = availableSkins[currentSkinIndex];

        // Update Image (Use UiIcon if available, otherwise fallback to SkinSprite)
        if (skinDisplayImage != null)
        {
            skinDisplayImage.sprite = currentSkin.UiIcon; // UiIcon property handles fallback
            skinDisplayImage.enabled = skinDisplayImage.sprite != null; // Hide image if no sprite
        }

        // Update Texts
        if (skinNameText != null) skinNameText.text = currentSkin.SkinName;
        if (skinDescriptionText != null) skinDescriptionText.text = currentSkin.SkinDesc;

        // Update Status/Requirement Text and Button States
        bool canUse = CanUseSkin(currentSkin);

        if (statusRequirementText != null)
        {
            if (canUse) // Includes default skins and those meeting high score req
            {
                statusRequirementText.text = "Unlocked"; // Or "Available"
            }
            // Removed IsOwned check here as CanUseSkin covers it implicitly via high score
            // else if (IsSkinOwned(currentSkin.SkinID)) // Check if owned via other means (kept for flexibility)
            // {
            //     statusRequirementText.text = "Owned";
            // }
            else // Not equipped, not default, and high score requirement not met
            {
                // Display requirement if not usable
                statusRequirementText.text = $"Requires High Score: {currentSkin.ScoreToUnlock}";
            }
        }

        // Update Button Interactability
        // REMOVED Buy Button Logic
        bool isEquipped = SkinManager.instance.CurrentSkin != null && SkinManager.instance.CurrentSkin.SkinID == currentSkin.SkinID;

        if (equipButton != null)
        {
            // Can equip if usable (meets requirements) BUT NOT currently equipped
            equipButton.interactable = canUse && !isEquipped;
            equipButton.GetComponent<Image>().sprite = isEquipped ? equipButtonImage_equipped :  equipButtonImage_Unequipped;
            // Show equip button only if the skin is usable
            equipButton.gameObject.SetActive(canUse);
        }

        // Enable/Disable Next/Prev based on count (though usually always enabled if > 1 skin)
        bool multipleSkins = availableSkins.Count > 1;
        if (nextButton) nextButton.interactable = multipleSkins;
        if (previousButton) previousButton.interactable = multipleSkins;
    }

    /// <summary>
    /// Checks if the player can currently use the specified skin.
    /// A skin is usable if it's the default skin OR the player's high score
    /// meets or exceeds the skin's ScoreToUnlock requirement.
    /// Also includes a check for skins explicitly marked as owned via PlayerPrefs (optional).
    /// </summary>
    /// <param name="skin">The SO_Skins data to check.</param>
    /// <returns>True if the skin can be used, false otherwise.</returns>
    private bool CanUseSkin(SO_Skins skin)
    {
        if (skin == null) return false;

        // 1. Default skins are always usable
        if (skin.IsDefaultSkin) return true;

        // 2. Check if explicitly marked as owned (e.g., via rewards - optional)
        if (IsSkinOwned(skin.SkinID)) return true;

        // 3. Check if player's high score meets the requirement
        // Use the cached high score for efficiency
        if (currentPlayerHighScore >= skin.ScoreToUnlock) return true;

        // If none of the above, the skin is not usable yet
        return false;
    }


    /// <summary>
    /// Checks PlayerPrefs to see if a skin with the given ID is marked as owned.
    /// This is kept separate in case you add other unlock methods (rewards, purchases later).
    /// Default skins are NOT automatically considered owned by this specific check,
    /// but CanUseSkin handles them separately.
    /// </summary>
    /// <param name="skinID">The ID of the skin to check.</param>
    /// <returns>True if the skin is marked as owned in PlayerPrefs, false otherwise.</returns>
    private bool IsSkinOwned(int skinID)
    {
        // Check PlayerPrefs directly
        string key = OwnedSkinPrefKeyPrefix + skinID.ToString();
        return PlayerPrefs.GetInt(key, 0) == 1; // Returns 1 if owned, 0 if not
    }

    /// <summary>
    /// Marks a skin as owned in PlayerPrefs. (Kept for potential future use)
    /// </summary>
    /// <param name="skinID">The ID of the skin to mark as owned.</param>
    private void MarkSkinAsOwned(int skinID)
    {
        string key = OwnedSkinPrefKeyPrefix + skinID.ToString();
        PlayerPrefs.SetInt(key, 1); // Set value to 1 to indicate owned
        PlayerPrefs.Save(); // Save changes immediately
        Debug.Log($"Skin ID {skinID} marked as owned in PlayerPrefs.");
    }

    // Optional: Add cleanup if needed
    // private void OnDestroy()
    // {
    //     if (nextButton) nextButton.onClick.RemoveListener(NextSkin);
    //     if (previousButton) previousButton.onClick.RemoveListener(PreviousSkin);
    //     // if (buyButton) buyButton.onClick.RemoveListener(BuySkin); // REMOVED
    //     if (equipButton) equipButton.onClick.RemoveListener(EquipSkin);
    // }
}
