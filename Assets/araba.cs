using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class araba : MonoBehaviour
{

    public float deger=1f;


    

    void Update()
    {
        transform.Translate(
    0f,
    0f,
    deger * GameSpeedManager.SpeedMultiplier * Time.deltaTime
);
    }
}
