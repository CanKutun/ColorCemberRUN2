using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public bool isPausedExternally = false; // Dışarıdan kontrol için
    public FireflyManager fireflyManager;
    public Light sun;
    public float cycleDuration = 60f;

    [Header("Renkler")]
    public Color dayColor = new Color(1f, 0.95f, 0.8f);
    public Color nightColor = new Color(0.1f, 0.1f, 0.25f);

    [Header("Parlaklık")]
    public float dayIntensity = 1.2f;
    public float nightIntensity = 0.2f;

    [Header("Ambient")]
    public Color dayAmbient = new Color(0.7f, 0.7f, 0.8f);
    public Color nightAmbient = new Color(0.05f, 0.05f, 0.1f);

    [Header("Ekran Üstü Gece Filtresi")]
    public Image nightOverlay;
    public float maxOverlayAlpha = 0.35f;

    [Header("Ateş Böceği Parlaklığı")]
    public ParticleSystem atesSag;
    public ParticleSystem atesSol;

    [Header("Ateş Böceği Sesi")]
    public AudioSource fireflyAudio;
    public float minFireflyVolume = 0f;   // gündüz
    public float maxFireflyVolume = 0.5f; // gece

    public float minFireflyIntensity = 0f;   // gündüz
    public float maxFireflyIntensity = 2.2f; // gece

    private Material fireflyMatSag;
    private Material fireflyMatSol;

    private float time;


    void Start()
    {
        if (atesSag != null)
        {
            var r = atesSag.GetComponent<ParticleSystemRenderer>();
            if (r != null)
                fireflyMatSag = r.material;
        }

        if (atesSol != null)
        {
            var r = atesSol.GetComponent<ParticleSystemRenderer>();
            if (r != null)
                fireflyMatSol = r.material;
        }
    }


    void Update()
    {
        if (isPausedExternally) return; // Dışarıdan durdurulmuşsa çalışmaz
        if (sun == null) return;

        time += Time.deltaTime;
        float half = cycleDuration / 2f;
        float t = Mathf.PingPong(time / half, 1f); // 0 = gündüz, 1 = gece
        float fireflyIntensity = Mathf.Lerp(
    minFireflyIntensity,
    maxFireflyIntensity,
    Mathf.SmoothStep(0.25f, 1f, t)
     );
        if (fireflyAudio != null)
        {
            float v = Mathf.Lerp(
                minFireflyVolume,
                maxFireflyVolume,
                fireflyIntensity / maxFireflyIntensity
        );
            fireflyAudio.volume = v;
        }

        if (fireflyMatSag != null)
            fireflyMatSag.SetColor("_EmissionColor", Color.white * fireflyIntensity);

        if (fireflyMatSol != null)
            fireflyMatSol.SetColor("_EmissionColor", Color.white * fireflyIntensity);

        sun.color = Color.Lerp(dayColor, nightColor, t);
        sun.intensity = Mathf.Lerp(dayIntensity, nightIntensity, t);
        RenderSettings.ambientLight = Color.Lerp(dayAmbient, nightAmbient, t);

        float angle = Mathf.Lerp(50f, -10f, t);
        sun.transform.rotation = Quaternion.Euler(angle, 0f, 0f);

        if (RenderSettings.skybox != null)
        {
            float exposure = Mathf.Lerp(1.1f, 0.3f, t);
            RenderSettings.skybox.SetFloat("_Exposure", exposure);

            Color dayTint = Color.white;
            Color nightTint = new Color(0.12f, 0.18f, 0.35f);
            RenderSettings.skybox.SetColor("_Tint", Color.Lerp(dayTint, nightTint, t));
        }

        if (nightOverlay != null)
        {
            float a = Mathf.Lerp(0f, maxOverlayAlpha, t);
            Color c = nightOverlay.color;
            c.a = a;
            nightOverlay.color = c;
        }

        //  TEK SATIR – Ateş böceğine bilgi gönder
        if (fireflyManager != null)
            fireflyManager.currentNightValue = t;
    }
}
