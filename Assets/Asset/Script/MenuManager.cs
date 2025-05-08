using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    [SerializeField] private GameObject _Leaderboard;
    [SerializeField] private GameObject _MainMenu;
    [SerializeField] private GameObject _CharacterSelection;
    [SerializeField] private GameObject _Credit;
    [SerializeField] private GameObject _Settings;

    public void StartGame() 
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GoToCharacterSelection() {
        _Leaderboard.SetActive(false);
        _MainMenu.SetActive(false);
        _CharacterSelection.SetActive(true);
    }

    public void GoToCredit() {
        _Credit.SetActive(true);
        _MainMenu.SetActive(false);
    }

    public void GoToMainMenu() {
        _Leaderboard.SetActive(false);
        _MainMenu.SetActive(true);
        _CharacterSelection.SetActive(false);
        _Credit.SetActive(false);
        _Settings.SetActive(false);
    }

    public void GoToLeaderboard() {
        _Leaderboard.SetActive(true);
        _MainMenu.SetActive(false);
        _CharacterSelection.SetActive(false);
    }

    public void GoToSettings() {
        _Settings.SetActive(true);
        _MainMenu.SetActive(false);
    }
}
