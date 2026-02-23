using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateButton : MonoBehaviour
{
    public void OpenStore()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.kutuns.colorcemberrun");
    }
}