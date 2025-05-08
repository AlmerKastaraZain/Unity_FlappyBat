using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlappyPauseMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _flappyScore;
    [SerializeField] private TextMeshProUGUI _flappyHighScore;
    [SerializeField] private FlappyGameOverMenu _flappyGameOverMenu;
    [SerializeField] private Image _flappyMedal;

    // Medal
    [SerializeField] private Sprite _bronzeMedal;
    [SerializeField] private Sprite _ironMedal;
    [SerializeField] private Sprite _goldMedal;

    public void UpdateUI(string score, string highScore, string medal) {
        _flappyScore.text = score;
        _flappyHighScore.text = highScore;

        switch (medal)
        {
            case "Bronze":
                _flappyMedal.gameObject.SetActive(true);
                _flappyMedal.sprite = _bronzeMedal;
                break;
            case "Iron":
                _flappyMedal.gameObject.SetActive(true);
                _flappyMedal.sprite = _ironMedal;
                break;
            case "Gold":
                _flappyMedal.gameObject.SetActive(true);
                _flappyMedal.sprite = _goldMedal;
                break;    
            default:
                _flappyMedal.sprite = null;
                _flappyMedal.gameObject.SetActive(false);
                break;
        }

        _flappyGameOverMenu.UpdateGameOverScoreUI(score, highScore, _flappyMedal.sprite);
    }
}
