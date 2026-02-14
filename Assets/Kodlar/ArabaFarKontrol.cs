using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArabaFarKontrol : MonoBehaviour
{
    public SpriteRenderer[] fars;

    [Header("Blink Ayarları")]
    public float blinkInterval = 0.3f;

    public Color sari = new Color(1f, 0.9f, 0.5f, 1f);
    public Color beyaz = Color.white;

    private float timer;
    private bool state;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= blinkInterval)
        {
            timer = 0f;
            state = !state;

            foreach (var sr in fars)
            {
                if (sr)
                    sr.color = state ? sari : beyaz;
            }
        }
    }
}