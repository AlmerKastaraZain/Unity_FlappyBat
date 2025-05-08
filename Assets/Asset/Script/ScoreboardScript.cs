using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScoreboardScript : MonoBehaviour
{
    [Header("UI Setup")]
    [Tooltip("Assign the Prefab for a single score entry UI element here.")]
    [SerializeField] GameObject _scoreItemStatPrefab;
    [Tooltip("Assign the parent Transform where score items will be instantiated (e.g., a panel with a Layout Group).")]
    [SerializeField] private Transform scoreListContainer;
    [SerializeField] RectTransform rectTransform;
    string saveFilePath;

    public void Awake()
    {

        saveFilePath = Application.persistentDataPath + "/ScoreData.json";
        InitializeList();
    }


// Call this method when you want to display/refresh the leaderboard
    public void InitializeList()
    {
        // --- Pre-checks ---
        if (_scoreItemStatPrefab == null)
        {
            Debug.LogError("Score Item Stat Prefab is not assigned in the ScoreboardScript inspector!", this.gameObject);
            return;
        }
        if (scoreListContainer == null)
        {
            Debug.LogError("Score List Container is not assigned in the ScoreboardScript inspector!", this.gameObject);
            return;
        }

        // --- Clear existing items ---
        // Destroy previous entries before adding new ones to prevent duplicates
        // Iterate backwards when removing items from a collection being iterated
        for (int i = scoreListContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(scoreListContainer.GetChild(i).gameObject);
        }
        // Alternative (often works fine in Unity for Transforms):
        // foreach (Transform child in scoreListContainer)
        // {
        //     Destroy(child.gameObject);
        // }

        // --- Fetch and Sort Scores ---
        List<ScoreData> scores = FetchScores();

        if (scores == null || scores.Count == 0)
        {
            Debug.Log("No scores found to display on the leaderboard.");
            // TODO: NO SCORE TEXT
            return;
        }

        // Sort scores: Highest score first is typical for leaderboards
        scores.Sort((a, b) => b.Score.CompareTo(a.Score));

        // --- Populate List ---
        Debug.Log($"Populating leaderboard with {scores.Count} scores...");
        foreach (ScoreData data in scores)
        {
            // Instantiate the prefab as a child of the designated container
            GameObject scoreEntryInstance = Instantiate(_scoreItemStatPrefab, scoreListContainer);

            // Get the ScoreItemStatScript component from the instantiated object
            ScoreItemStatScript itemScript = scoreEntryInstance.GetComponent<ScoreItemStatScript>();

            if (itemScript != null)
            {
                // Convert the char[] alias to a string using the helper property
                string alias = data.AliasString;

                // Call the SetStats function with the fetched data
                itemScript.SetStats(alias, data.Score, data.Id);
            }
            else
            {
                // Log an error if the prefab is missing the required script
                Debug.LogError($"Instantiated Score Item Prefab ('{_scoreItemStatPrefab.name}') is missing the ScoreItemStatScript component.", scoreEntryInstance);
                // Optionally destroy the problematic instance: Destroy(scoreEntryInstance);
            }


            // Get the current scale
            Vector3 currentScale = this.transform.localScale;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + (10 + 100));
            // Set the new height (only changing the y-axis)
        }
    }

    /// <summary>
    /// Reads the ScoreData.json file from the persistent data path,
    /// parses it, and returns the list of ScoreData objects.
    /// Returns an empty list if the file doesn't exist, is empty, or cannot be parsed.
    /// </summary>
    /// <returns>A List of ScoreData objects, or an empty List on failure.</returns>
     public List<ScoreData> FetchScores()
    {
        // Ensure the file path is set (should be by Awake)
        if (string.IsNullOrEmpty(saveFilePath))
        {
            // Recalculate if Awake hasn't run or path was lost
            saveFilePath = Path.Combine(Application.persistentDataPath, "ScoreData.json");
            Debug.LogWarning("Save file path was not set, recalculating in FetchScores.");
        }

        // Check if the file exists
        if (File.Exists(saveFilePath))
        {
            try
            {
                // Read the entire JSON file content
                string jsonContent = File.ReadAllText(saveFilePath);

                // Check if the file content is empty or just whitespace
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    Debug.LogWarning($"Score file found at '{saveFilePath}' but it is empty.");
                    return new List<ScoreData>(); // Return empty list
                }

                // Parse the JSON string into our wrapper object
                ScoreDataList loadedData = JsonUtility.FromJson<ScoreDataList>(jsonContent);

                // Check if parsing was successful and the inner list exists
                if (loadedData != null && loadedData.scoreDataList != null)
                {
                    Debug.Log($"Successfully loaded {loadedData.scoreDataList.Count} scores from '{saveFilePath}'.");
                    // Return the actual list of scores
                    return loadedData.scoreDataList;
                }
                else
                {
                    // Parsing failed or the structure didn't match
                    Debug.LogError($"Failed to parse score data from JSON file at '{saveFilePath}'. Check JSON structure and ScoreDataList class definition.");
                    return new List<ScoreData>(); // Return empty list on parsing error
                }
            }
            catch (Exception e)
            {
                // Catch potential errors during file reading or parsing
                Debug.LogError($"Error reading or parsing score file at '{saveFilePath}': {e.Message}");
                return new List<ScoreData>(); // Return empty list on exception
            }
        }
        else
        {
            // File does not exist
            Debug.LogWarning($"Score file not found at '{saveFilePath}'. Returning empty list.");
            return new List<ScoreData>(); // Return empty list if file doesn't exist
        }
    }
}
