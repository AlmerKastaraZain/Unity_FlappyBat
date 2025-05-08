// d:\Unity\FlappyBird\Assets\Asset\Script\LevelTextUI.cs
using UnityEngine;
using TMPro; // Required for TextMeshProUGUI

[RequireComponent(typeof(TextMeshProUGUI))]
public class LevelTextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelTextComponent;

    private void Awake()
    {
        // Get the component if not assigned in Inspector
        if (levelTextComponent == null)
        {
            levelTextComponent = GetComponent<TextMeshProUGUI>();
        }

        // Error check remains useful
        if (levelTextComponent == null)
        {
            Debug.LogError("LevelTextUI: No TextMeshProUGUI component found or assigned!", this.gameObject);
            this.enabled = false; // Disable script if no text component
            return;
        }
    }

    // Subscribe to the event when this component becomes active/enabled
    private void OnEnable()
    {
        // Ensure we have the component before subscribing
        if (levelTextComponent == null) return;

        // Subscribe to level changes
        LevelManager.OnLevelGained += HandleLevelGained;

        // --- Optional: Update text if re-enabled after initial start ---
        // If the UI might be disabled and re-enabled *after* the game has started,
        // you might want to update the text here too, but check instance existence.
        if (LevelManager.instance != null)
        {
             UpdateLevelText(LevelManager.instance.CurrentLevel);
        }
    }

    // Set the initial level text *after* all Awakes are done
    private void Start()
    {
        if (levelTextComponent == null) return; // Already checked in Awake, but safe

        // Check if LevelManager instance exists now (it should in Start)
        if (LevelManager.instance != null)
        {
            UpdateLevelText(LevelManager.instance.CurrentLevel);
        }
        else
        {
            // If it's STILL null here, there's a bigger problem (LevelManager missing?)
            Debug.LogError("LevelTextUI: LevelManager instance STILL not found in Start! Is LevelManager in the scene and active?", this.gameObject);
            levelTextComponent.text = "Level: ?";
        }
    }


    // IMPORTANT: Unsubscribe when the component is disabled or destroyed
    private void OnDisable()
    {
        // Unsubscribe from level changes
        LevelManager.OnLevelGained -= HandleLevelGained;
    }

    // This method is automatically called when LevelManager invokes the OnLevelGained event
    private void HandleLevelGained(int newLevel)
    {
        UpdateLevelText(newLevel);
    }

    // Helper method to format and update the TextMeshPro text
    private void UpdateLevelText(int level)
    {
        if (levelTextComponent != null)
        {
            levelTextComponent.text = "Level: " + level.ToString();
        }
    }
}
