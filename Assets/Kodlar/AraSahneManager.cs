using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Components;
public class AraSahneManager : MonoBehaviour
{

    public TMP_Text countdownText;
    public TMP_Text actionText;
    public LocalizeStringEvent actionTextLocalize;
    public string gameSceneName = "Oyun"; // OYUN SAHNENÝN ADI

    void Start()
    {
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
       // 3
        countdownText.text = "3";
        actionTextLocalize.StringReference.SetReference("UI_Text", "TXT_ACTION_3");
        yield return new WaitForSeconds(1f);

        // 2
        countdownText.text = "2";
        actionTextLocalize.StringReference.SetReference("UI_Text", "TXT_ACTION_2");
        yield return new WaitForSeconds(1f);

        // 1
        countdownText.text = "1";
        actionTextLocalize.StringReference.SetReference("UI_Text", "TXT_ACTION_1");
        yield return new WaitForSeconds(1f);

        
        SceneManager.LoadScene(gameSceneName);
        yield break;
    }
}