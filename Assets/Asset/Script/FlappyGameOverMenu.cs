using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlappyGameOverMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _flappyScore;
    [SerializeField] private TextMeshProUGUI _flappyHighScore;
    [SerializeField] private Image _flappyMedal;

    public void UpdateGameOverScoreUI(string score, string highScore, Sprite medal) {
        _flappyScore.text = score;
        _flappyHighScore.text = highScore;

        if (medal != null) {
            _flappyMedal.gameObject.SetActive(true);
            _flappyMedal.sprite = medal;
        } else {
            _flappyMedal.gameObject.SetActive(false);
        }
    }
}
