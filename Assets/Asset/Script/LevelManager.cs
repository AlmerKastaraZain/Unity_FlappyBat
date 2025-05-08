// d:\Unity\FlappyBird\Assets\Asset\Script\LevelManager.cs
using System; // Required for Action
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    // --- Event ---
    // Action that broadcasts the new level number when the level increases.
    // Making it static allows easier subscription from anywhere, but requires careful management if LevelManager persists across scenes.
    public static event Action<int> OnLevelGained;

    [Header("Speed Configuration")]
    [SerializeField] private float basePipeSpeed = 0.65f; // Starting speed for pipes
    [SerializeField] private float baseGroundSpeed = 1.0f; // Starting speed for ground

    [Tooltip("For every X points scored, the speed increases.")]
    [SerializeField] private int scoreThresholdForSpeedIncrease = 10; // e.g., increase speed every 10 points

    [Tooltip("How much the pipe speed increases per threshold reached.")]
    [SerializeField] private float pipeSpeedIncrement = 0.1f;

    [Tooltip("How much the ground speed increases per threshold reached.")]
    [SerializeField] private float groundSpeedIncrement = 0.15f; // Ground might speed up slightly faster

    [SerializeField] private float maxPipeSpeed = 2.0f;     // Maximum speed limit for pipes
    [SerializeField] private float maxGroundSpeed = 3.0f;   // Maximum speed limit for ground

    [Header("Level Configuration")] // New Header for Level settings
    [Tooltip("How many points are required to advance to the next level.")]
    [SerializeField] private int scorePerLevel = 25; // e.g., Level 2 starts at 25 points, Level 3 at 50, etc.

    // Public properties to provide the current calculated speed and level
    public float CurrentPipeSpeed { get; private set; }
    public float CurrentGroundSpeed { get; private set; }
    public int CurrentLevel { get; private set; }

    // --- Private Fields ---
    private int _previousLevel = 0; // To track when the level actually changes

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }

        // Initialize speeds and level to base values
        CurrentPipeSpeed = basePipeSpeed;
        CurrentGroundSpeed = baseGroundSpeed;
        CurrentLevel = 1; // Start at level 1
        _previousLevel = CurrentLevel; // Initialize previous level
    }

    private void Start()
    {
        // Ensure thresholds are valid
        if (scoreThresholdForSpeedIncrease <= 0)
        {
            Debug.LogWarning("scoreThresholdForSpeedIncrease must be greater than 0. Defaulting to 10.");
            scoreThresholdForSpeedIncrease = 10;
        }
        if (scorePerLevel <= 0)
        {
            Debug.LogWarning("scorePerLevel must be greater than 0. Defaulting to 25.");
            scorePerLevel = 25;
        }

        // Initial calculation in case the game starts with a score > 0
        UpdateStatsBasedOnScore();
    }


    private void Update()
    {
        // Continuously check and update stats based on the current score
        UpdateStatsBasedOnScore();
    }

    private void UpdateStatsBasedOnScore()
    {
        if (GameManager.instance == null)
        {
            // Debug.LogWarning("GameManager instance not found in LevelManager. Cannot update stats."); // Reduce log spam
            return; // Cannot get score if GameManager doesn't exist
        }

        int currentScore = GameManager.instance.GetScore();

        // --- Calculate Level ---
        int calculatedLevel = 1 + (currentScore / scorePerLevel); // Calculate potential new level

        // --- Check for Level Change and Invoke Event ---
        if (calculatedLevel > _previousLevel) // Check if the level has actually increased
        {
            CurrentLevel = calculatedLevel; // Update the public property
            Debug.Log($"Level Up! Reached Level: {CurrentLevel}"); // Log the level up
            OnLevelGained?.Invoke(CurrentLevel); // Invoke the event, passing the new level
            _previousLevel = CurrentLevel; // Update the tracking variable
        }
        // Ensure CurrentLevel is at least 1, even if calculation somehow results in less (e.g., negative score)
        // This check might be redundant given the calculation `1 + (score / scorePerLevel)` but adds safety.
        else if (calculatedLevel < 1)
        {
             CurrentLevel = 1;
             // No level change event needed if it drops below 1 (shouldn't happen with positive scores)
             if (_previousLevel != 1) _previousLevel = 1; // Reset tracker if needed
        }
        // If the calculated level is the same or lower (but not below 1), just ensure CurrentLevel reflects it
        // This handles cases where score might decrease (if that's possible in your game)
        // or just stays the same.
        else if (calculatedLevel != CurrentLevel)
        {
             CurrentLevel = calculatedLevel;
             _previousLevel = CurrentLevel; // Keep tracker updated even on level decrease
        }


        // --- Calculate Speed ---
        int speedThresholdsReached = currentScore / scoreThresholdForSpeedIncrease;
        float targetPipeSpeed = basePipeSpeed + (speedThresholdsReached * pipeSpeedIncrement);
        float targetGroundSpeed = baseGroundSpeed + (speedThresholdsReached * groundSpeedIncrement);
        CurrentPipeSpeed = Mathf.Min(targetPipeSpeed, maxPipeSpeed);
        CurrentGroundSpeed = Mathf.Min(targetGroundSpeed, maxGroundSpeed);
    }

    // Optional: A public getter specifically for the level if needed elsewhere (though the property works too)
    // public int GetLevel()
    // {
    //     return CurrentLevel;
    // }

    // --- Cleanup ---
    // Optional: Unsubscribe static events if the LevelManager might be destroyed and recreated
    // Or if subscribers might be destroyed before the LevelManager
    // private void OnDestroy()
    // {
    //     // If you have static subscribers that don't clean up themselves,
    //     // you might need to clear the event invocation list, but this is complex.
    //     // Usually, subscribers handle their own unsubscription in OnDisable/OnDestroy.
    //     // Example: OnLevelGained = null; // Be careful with this if other instances might exist.
    // }
}
