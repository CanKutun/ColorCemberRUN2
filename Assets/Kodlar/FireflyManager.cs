using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyManager : MonoBehaviour
{
    public ParticleSystem fireflySag;
    public ParticleSystem fireflySol;

    [Header("Gece Ayarları")]
    [Range(0f, 1f)]
    public float nightStart = 0.45f;   // hava kararmaya başlıyor
    [Range(0f, 1f)]
    public float fullNight = 0.75f;    // tam gece

    [Header("Yoğunluk")]
    public float minRate = 0f;   // gündüz
    public float maxRate = 6f;   // tam gece

    [HideInInspector]
    public float currentNightValue = 0f;

    private ParticleSystem.EmissionModule emSag;
    private ParticleSystem.EmissionModule emSol;

    void Start()
    {
        if (fireflySag)
        {
            fireflySag.Play();
            emSag = fireflySag.emission;
        }

        if (fireflySol)
        {
            fireflySol.Play();
            emSol = fireflySol.emission;
        }

        SetRate(0f);
    }

    void Update()
    {
        float rate = 0f;

        if (currentNightValue <= nightStart)
        {
            // gündüz
            rate = 0f;
        }
        else if (currentNightValue >= fullNight)
        {
            // tam gece
            rate = maxRate;
        }
        else
        {
            //  yumuşak geçiş
            float t = Mathf.InverseLerp(nightStart, fullNight, currentNightValue);
            rate = Mathf.Lerp(minRate, maxRate, t);
        }

        SetRate(rate);
    }

    void SetRate(float rate)
    {
        if (fireflySag)
            emSag.rateOverTime = rate;

        if (fireflySol)
            emSol.rateOverTime = rate;
    }
}