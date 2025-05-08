using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AliasScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;

    public void OnValueChanged() {
        if (_inputField.text.Length == 4)
        {
            GameManager.instance.InitializeScoreSave(_inputField.text[0], _inputField.text[1], _inputField.text[2], _inputField.text[3]);
            Submit();
        }
        else if (_inputField.text.Length > 4) 
        {
            GameManager.instance.InitializeScoreSave(_inputField.text[0], _inputField.text[1], _inputField.text[2], _inputField.text[3]);
            Submit();
        }

        Debug.Log("Hello " + _inputField.text.Length + _inputField.text);
    }

    public void Submit() {
        GameManager.instance.SaveScore();

        GameManager.instance.GameOverCover();
    }
}
