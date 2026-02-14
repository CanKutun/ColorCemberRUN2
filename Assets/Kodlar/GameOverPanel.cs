using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    public TextMeshProUGUI highScoreValueText;

    void OnEnable()
    {
        int hs = PlayerPrefs.GetInt("HighScore", 0);
        highScoreValueText.text = hs.ToString();
    }
}