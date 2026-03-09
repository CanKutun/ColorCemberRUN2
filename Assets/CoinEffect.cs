using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinEffect : MonoBehaviour
{
    public float rotateSpeed = 120f;
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.05f;

    Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        // dönme
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);

        // nefes alma efekti
        float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = startScale * scale;
    }
}