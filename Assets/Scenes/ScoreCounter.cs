using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
   
    public int anlikSkor = 0;

    public TMP_Text sayacText;

    void Update()
    {
        
    }

    public void SkorEkle(int miktar)
    {
        anlikSkor += miktar;
    }
}