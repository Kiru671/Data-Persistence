using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HIScore : MonoBehaviour
{
    [SerializeField] private Text hiScoreText;
    
    private void Start()
    {
        UpdateText();
        MainManager.OnGameOver += UpdateText;
        MainManager.OnHIScoreChanged += UpdateText;
    }

    void UpdateText()
    {
        if (hiScoreText == null) return;
        hiScoreText.text = $"Best Score: {DataManager.getInstance.GetInt("0", "HiScore")}";
    }
}
