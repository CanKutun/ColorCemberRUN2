using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class InternetChecker : MonoBehaviour
{
    public GameObject internetPanel;
    public TextMeshProUGUI internetText;

    void Start()
    {
        internetPanel.SetActive(true);   // baþta panel açýk

        if (Application.systemLanguage == SystemLanguage.Turkish)
            internetText.text = "Ýnternet baðlantýsý gerekli\r\nLütfen baðlantýnýzý kontrol edin";
        else
            internetText.text = "Internet connection required\r\nPlease check your connection";

        StartCoroutine(CheckInternet());
    }

    IEnumerator CheckInternet()
    {
        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Get("https://clients3.google.com/generate_204");
            request.timeout = 5;

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                internetPanel.SetActive(true);
            else
                internetPanel.SetActive(false);

            yield return new WaitForSeconds(2f);
        }
    }
}