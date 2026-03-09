using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameVersion : MonoBehaviour
{
    public TextMeshProUGUI versionText;

    void Start()
    {
        versionText.text = "Version " + Application.version;
    }
}
