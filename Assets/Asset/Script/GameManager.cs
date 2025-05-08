using System;
using System.Collections.Generic;
using System.IO;
// using NUnit.Framework.Interfaces; // Removed if not needed
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Keep these class definitions as they are - they seem correct for JsonUtility
[Serializable] // Add this attribute so JsonUtility knows how to handle it
public class ScoreData
{
    public int Id;
    public int Score;
    public char[] Alias = new char[4];

    // Helper property to easily get Alias as a string
    public string AliasString => new string(Alias);
}

[Serializable] // Add this attribute so JsonUtility knows how to handle it
public class ScoreDataList {
    public List<ScoreData> scoreDataList; // This list inside the wrapper should hold ScoreData
}

public enum GameState {
    GameActive,
    GameInactive
}

public class GameManager : MonoBehaviour
{
    // --- Game State Event ---
    public event Action OnGameOver;

    // --- Game State ---
    private GameState _gameState = GameState.GameActive;

    // --- Have started yet ---
    public bool HasGameStarted = false;

    public GameState getGameState() {
        return _gameState;
    }

    public static GameManager instance;
    [SerializeField] private GameObject _gameOverCanvas;
    [SerializeField] private GameObject _gameOverAliasCanvas;
    [SerializeField] private FlappyPauseMenu _flappyMenuCanvas;
    [SerializeField] private GameObject _gameOverlayCanvas;
    [SerializeField] private TextMeshProUGUI _scoreText;

    // Score
    private int score = 0;

    // Correctly typed and initialized list
    private List<ScoreData> scoreDataList = new List<ScoreData>(); 
    private ScoreData currentScoreData; // Renamed from scoreData to avoid confusion with the class name
    string saveFilePath;

    public int GetScore() { return score; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        saveFilePath = Application.persistentDataPath + "/ScoreData.json";
        
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); 
        }

        // Initialize the list (already done at declaration, but safe to ensure here too)
        scoreDataList = new List<ScoreData>();
        // Load any existing scores when the game starts
        ReadScore();

        if (HasGameStarted) Time.timeScale = 1f;
    }

    public void GameOver()
    {
        // Don't hide _gameOverCanvas immediately if it contains the alias input field logic
        // _gameOverCanvas.SetActive(false); // Keep this active if AliasScript is on it or its children
        _gameOverAliasCanvas.SetActive(true); // This seems to be the alias input screen
        _gameOverlayCanvas.SetActive(false);
        _flappyMenuCanvas.gameObject.SetActive(false);

        Time.timeScale = 0f;

        _gameState = GameState.GameInactive;

        // Update the UI elements that show score *before* alias input (if any)
        UpdatePauseMenuUI(); // Update scores shown on pause/game over menus

        OnGameOver?.Invoke();
    }

    // This function seems intended to show the final score summary *after* alias is entered
    public void GameOverCover()
    {
        _gameOverCanvas.SetActive(true); // Show the final summary screen
        _gameOverAliasCanvas.SetActive(false); // Hide the alias input screen
        _gameOverlayCanvas.SetActive(false);
        _flappyMenuCanvas.gameObject.SetActive(false); // Ensure pause menu is hidden

        _gameState = GameState.GameInactive;

        // Update the final summary screen UI (likely uses FlappyGameOverMenu)
        // Note: UpdatePauseMenuUI might have already calculated medal/highscore
        // You might need a separate method or pass data to FlappyGameOverMenu directly here
        // For now, assuming UpdatePauseMenuUI updates the necessary components shown on _gameOverCanvas
        UpdatePauseMenuUI();

        // Saving happens when the alias is submitted via AliasScript.Submit(), which calls SaveScore()
        // Calling SaveScore() here might be redundant or save incomplete data if alias wasn't set.
        // SaveScore(); // Removed from here - should be called after alias is set
    }


    public void ToggleFlappyMenu()
    {
        // This logic seems reversed or intended for a pause menu, not the alias input
        // Let's assume this is for a PAUSE menu distinct from game over
        if (_flappyMenuCanvas.gameObject.activeSelf == false)
        {
            _gameState = GameState.GameInactive;

            // Pause the game
            _gameOverlayCanvas.SetActive(false);
            _flappyMenuCanvas.gameObject.SetActive(true); // Show the PAUSE menu
            _gameOverAliasCanvas.SetActive(false); // Hide alias input if it was somehow active
            Time.timeScale = 0f;
            UpdatePauseMenuUI(); // Update scores on pause menu
        }
        else
        {
            // Resume the game
            _gameOverlayCanvas.SetActive(true);
            _flappyMenuCanvas.gameObject.SetActive(false); // Hide the PAUSE menu
            if (HasGameStarted) Time.timeScale = 1f;
            _gameState = GameState.GameActive;

            // No need to update UI when resuming usually
        }
    }

    public void UpdatePauseMenuUI() {
        string newMedal = "";
        string newHighScore = "";
        string newScore = score.ToString();

        // Medal Logic (Seems reasonable)
        if (score >= 100) newMedal = "Gold";
        else if (score >= 50) newMedal = "Iron"; // Simplified condition
        else if (score > 10) newMedal = "Bronze"; // Simplified condition
        else newMedal = "N/A"; // Or perhaps "" or null depending on FlappyPauseMenu handling

        // High Score Logic
        int currentHighScore = PlayerPrefs.GetInt("HighScore", 0); // Get current high score safely
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
            newHighScore = score.ToString(); // The new high score is the current score
        }
        else
        {
            newHighScore = currentHighScore.ToString(); // Show the existing high score
        }

        Debug.Log("Updating UI - Score: " + newScore + ", HighScore: " + newHighScore + ", Medal: " + newMedal);

        // Update the UI elements (Pause Menu and potentially Game Over Menu via FlappyPauseMenu)
        _flappyMenuCanvas.UpdateUI(newScore, newHighScore, newMedal);
    }

    public void AddScore(int scoreGained)
    {
        score += scoreGained;
        _scoreText.text = score.ToString();

        // Optional: Update high score display in real-time if needed on the game overlay
        // UpdatePauseMenuUI(); // Calling this every point might be excessive, depends on performance/design
    }

    public void RestartGame()
    {
        if (HasGameStarted) Time.timeScale = 1f; // Ensure time scale is reset before loading scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        if (HasGameStarted) Time.timeScale = 1f; // Ensure time scale is reset before loading scene
        SceneManager.LoadScene("MenuScene");
    }

    // Prepares the data object for the current game's score
    public void InitializeScoreSave(char a, char b, char c, char d) {
        currentScoreData = new ScoreData(); // Create a new object for this game session
        currentScoreData.Score = score;
        currentScoreData.Alias[0] = a;
        currentScoreData.Alias[1] = b;
        currentScoreData.Alias[2] = c;
        currentScoreData.Alias[3] = d;
        Debug.Log($"Score data initialized: Alias={new string(currentScoreData.Alias)}, Score={currentScoreData.Score}");
    }

    // Saves the *currently prepared* score data to the list and file
    public void SaveScore()
    {
        if (currentScoreData == null) {
            Debug.LogError("SaveScore called but currentScoreData is null. Was InitializeScoreSave called?");
            return;
        }

        // Ensure the list is loaded (though it should be from Awake)
        // ReadScore(); // Usually not needed here if loaded in Awake, but can be a safeguard

        // Set Id for the new ScoreData based on the current list size
        if (scoreDataList.Count > 0)
        {
            // Find the highest existing ID and add 1
            int maxId = 0;
            foreach(var data in scoreDataList) {
                if (data.Id > maxId) maxId = data.Id;
            }
            currentScoreData.Id = maxId + 1;
            // Simpler if always appending: currentScoreData.Id = scoreDataList[scoreDataList.Count - 1].Id + 1;
        }
        else
        {
            currentScoreData.Id = 1; // First entry
        }

        // Add the new score data to the list
        scoreDataList.Add(currentScoreData);
        Debug.Log($"Added score data to list. New count: {scoreDataList.Count}");


        // Create a wrapper object containing the *entire updated list*
        ScoreDataList dataToSave = new ScoreDataList { scoreDataList = this.scoreDataList };

        try
        {
            // Serialize the wrapper object to JSON
            string saveScoreData = JsonUtility.ToJson(dataToSave, true); // Use 'true' for pretty print (debugging)

            // Write the JSON string to the file, overwriting previous content
            File.WriteAllText(saveFilePath, saveScoreData);
            Debug.Log("Score data saved successfully to " + saveFilePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save score data: {e.Message}");
        }

        // Clear the current data after saving to prevent accidental re-saving
        currentScoreData = null;
    }

    // Reads the score data from the file into the scoreDataList
    public void ReadScore() {
        if (File.Exists(saveFilePath)) {
            try
            {
                string readScoreData = File.ReadAllText(saveFilePath);
                if (string.IsNullOrWhiteSpace(readScoreData))
                {
                    Debug.Log("Score file is empty. Initializing empty list.");
                    scoreDataList = new List<ScoreData>();
                    return;
                }

                ScoreDataList loadedData = JsonUtility.FromJson<ScoreDataList>(readScoreData);

                if (loadedData != null && loadedData.scoreDataList != null) {
                    // Successfully loaded data, assign it to our list
                    scoreDataList = loadedData.scoreDataList;
                    Debug.Log($"Loaded {scoreDataList.Count} scores from file.");
                } else {
                    // File exists, but JSON parsing failed or resulted in nulls
                    Debug.LogWarning("Could not parse score data from file or file content is invalid. Initializing empty list.");
                    scoreDataList = new List<ScoreData>(); // FIX 2: Initialize with the correct type
                }
            }
            catch (Exception e)
            {
                 Debug.LogError($"Failed to read or parse score data: {e.Message}. Initializing empty list.");
                 scoreDataList = new List<ScoreData>(); // Initialize empty on error
            }
        } else {
            // File doesn't exist, start with an empty list
            Debug.Log("Score file not found. Initializing empty list.");
            scoreDataList = new List<ScoreData>(); // Ensure it's initialized
        }
    }

    // Example method to potentially display scores (you'd call this from somewhere else)
    public void PrintScoresToLog() {
        Debug.Log("--- High Scores ---");
        if (scoreDataList == null || scoreDataList.Count == 0) {
            Debug.Log("No scores saved yet.");
            return;
        }

        // Optional: Sort scores descending before printing
        scoreDataList.Sort((a, b) => b.Score.CompareTo(a.Score));

        foreach (var data in scoreDataList) {
            Debug.Log($"ID: {data.Id}, Alias: {new string(data.Alias)}, Score: {data.Score}");
        }
        Debug.Log("------------------");
    }
}
