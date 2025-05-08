// d:\Unity\FlappyBird\Assets\Asset\GameObject\Skins\SO_Skins.cs
using UnityEngine;

/// <summary>
/// ScriptableObject defining the properties of a single player skin.
/// Create instances via Assets -> Create -> Game Data -> Skin
/// </summary>
[CreateAssetMenu(fileName = "New Skin", menuName = "Game Data/Skin")]
public class SO_Skins : ScriptableObject // Inherit from ScriptableObject, NOT MonoBehaviour
{
    [Header("Skin Identification")]

    [Tooltip("Unique identifier for this skin (used internally, e.g., for saving selection). Should be unique across all skins.")]
    [SerializeField] private int skinID;

    [Tooltip("Display name and description of the skin shown to the player.")]
    [SerializeField] private string skinName = "Default Skin";
    [SerializeField] private string skinDesc = "A new skin";

    [Header("Visuals")]

    [Tooltip("The main sprite used for the player character with this skin.")]
    [SerializeField] private Sprite skinSprite;

    [Tooltip("Optional: A smaller icon representation for UI elements like selection menus.")]
    [SerializeField] private Sprite uiIcon; // Optional, can be the same as skinSprite if needed

    // --- NEW: Animation Section ---
    [Header("Animation")]

    [Tooltip("The Animator Controller that defines the animations for this skin (e.g., flapping, idle). Assign the controller asset here.")]
    [SerializeField] private RuntimeAnimatorController animatorController;
    // --- End NEW ---


    [Header("Unlock Conditions (Example)")]

    [Tooltip("Is this the default skin available from the start?")]
    [SerializeField] private bool isDefaultSkin = false;

    [Tooltip("Score required to unlock this skin (0 if unlocked by default or other means).")]
    [SerializeField] private int scoreToUnlock = 0;

    // --- Public Accessors (Properties) ---
    // Provide read-only access to the data from other scripts

    public int SkinID => skinID;
    public string SkinName => skinName;
    public string SkinDesc => skinDesc;
    public Sprite SkinSprite => skinSprite;
    public Sprite UiIcon => uiIcon != null ? uiIcon : skinSprite; // Fallback to main sprite if no specific icon

    // --- NEW: Public Accessor for Animator Controller ---
    public RuntimeAnimatorController AnimatorController => animatorController;
    // --- End NEW ---

    public bool IsDefaultSkin => isDefaultSkin;
    public int ScoreToUnlock => scoreToUnlock;

}
