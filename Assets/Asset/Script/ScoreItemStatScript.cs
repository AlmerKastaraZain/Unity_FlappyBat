using TMPro;
using UnityEngine;

public class ScoreItemStatScript : MonoBehaviour
{
    /// <summary>
    /// Sets the text of the first two child TextMeshProUGUI components.
    /// Assumes the first child (index 0) is for Score and the second child (index 1) is for Name.
    /// </summary>
    /// <param name="playerName">The name (alias) to display on the second child.</param>
    /// <param name="playerScore">The score to display on the first child.</param>
    private TextMeshProUGUI _scoreText;
    private TextMeshProUGUI _nameText;
    private TextMeshProUGUI _idText;

    public void Awake()
    {
        // --- Access Child Components ---
        // Get the Transform of the first child (index 0)
        Transform scoreChild = transform.GetChild(0);
        // Get the Transform of the second child (index 1)
        Transform nameChild = transform.GetChild(1);
        Transform idChild = transform.GetChild(2);

        // Attempt to get the TextMeshProUGUI component from the first child
        _scoreText = scoreChild?.GetComponent<TextMeshProUGUI>();
        // Attempt to get the TextMeshProUGUI component from the second child
        _nameText = nameChild?.GetComponent<TextMeshProUGUI>();
        _idText = idChild?.GetComponent<TextMeshProUGUI>();
    }
    public void SetStats(string playerName, int playerScore, int id)
    {
        // --- Validate Child Count ---
        if (transform.childCount < 3)
        {
            Debug.LogError($"ScoreItemStatScript on '{gameObject.name}' requires at least 2 child objects to function.", this.gameObject);
            return; // Exit if the required children aren't present
        }

        // --- Update Score Text ---
        if (_scoreText != null)
        {
            // Convert the integer score to a string for display
            _scoreText.text = playerScore.ToString();
        }
        else
        {
            // Log an error if the first child doesn't have the required component
            Debug.LogError($"First child (scoreText) of '{gameObject.name}' is missing a TextMeshProUGUI component for the name.");
        }

        // --- Update Name Text ---
        if (_nameText != null)
        {
            // Assign the player name string directly
            _nameText.text = playerName;
        }
        else
        {
            // Log an error if the second child doesn't have the required component
            Debug.LogError($"Second child (nameText) of '{gameObject.name}' is missing a TextMeshProUGUI component for the name.");
        }

                // --- Update ID Text ---
        if (_idText != null)
        {
            // Convert the integer ID to a string for display
            _idText.text = id.ToString();
        }
        else
        {
            // Log error if component wasn't found in Awake
            Debug.LogError($"Third child (idText) '{gameObject.name}' is missing a TextMeshProUGUI component for the name.");
        }
    }

}
