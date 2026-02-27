using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class yonetici : MonoBehaviour
{
    public float yolUzunluk = 10.0f;
    Coroutine engelRoutine;
    public TextMeshProUGUI hedefText;
    public Transform yol2;
    Vector3 yol1BaslangicPos;
    Vector3 yol2BaslangicPos;
    Vector3 cocukBaslangicPos;
    GameObject[] aktifFaz;
    int yedekPuan;
    int yedekFaz;
    int yedekSonrakiFaz;
    Dictionary<GameObject, float> yOffset = new Dictionary<GameObject, float>();

    
    Coroutine altinRoutine;
    Coroutine miknatisRoutine;

    public int puan;
    public int highScore;

    float solX = -2.9f;
    float sagX = -0.45f;


    public int faz = 1;
    public int sonrakiFazSkoru = 1000;
    public int fazArtisMiktari = 1000;

    public int maxFaz = 4;

    public Transform yol1;
    private float zeminY;

    public TextMeshProUGUI score_value;
    public TextMeshProUGUI highScore_value;

    public GameObject altin;
    public GameObject miknatis;

    float sonrakiAltinZ;
    float altinMesafe = 30f;

    float sonrakiMiknatisZ;
    float miknatisMesafe = 60f;

    float sonrakiMiknatisZamani = 0f;
    float sonrakiEngelZ = 0f;
    float minEngelMesafe = 12f;

    //  public GameObject kutuk;
    //  public GameObject tas;
    //  public GameObject araba;

    [Header("Faz Engelleri")]
    public GameObject[] faz1Engeller;
    public GameObject[] faz2Engeller;
    public GameObject[] faz3Engeller;
    public GameObject[] faz4Engeller;

    List<GameObject> altinlar;
    List<GameObject> miknatislar;
    List<GameObject> digerleri;

    Transform cocuk;

    public GameObject oyun_bitti_paneli;

    public GameObject oyunu_durdur_pnl;

    bool rewardUsed = false;

   
    void OnEnable()
    {
        rewardUsed = false;
    }


    void Start()
    {
        Collider yolCllider = yol1.GetComponent<Collider>();

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScore_value.text = highScore.ToString();

        if (LocalizationSettings.SelectedLocale.Identifier.Code == "tr")
            GameObject.Find("highscore_label")
                .GetComponent<TextMeshProUGUI>().text = "REKOR";
        else
            GameObject.Find("highscore_label")
                .GetComponent<TextMeshProUGUI>().text = "HIGH SCORE";

        altinlar = new List<GameObject>();
        miknatislar = new List<GameObject>();
        digerleri = new List<GameObject>();

        cocuk = GameObject.Find("cocuk").transform;
        zeminY = cocuk.position.y;

        // ===== BAŞLANGIÇ POZİSYONLARI =====
        cocukBaslangicPos = cocuk.position;
        yol1BaslangicPos = yol1.position;
        yol2BaslangicPos = yol2.position;

        // Karakteri biraz ileri al
        cocuk.position = new Vector3(
            cocuk.position.x,
            cocuk.position.y,
            cocuk.position.z + 1.2f
        );

        // ===== SABİT MESAFELER (DENGELİ) =====
        altinMesafe = 5f;       // dengeli seyrek
        miknatisMesafe = 20f;    // gerçekten nadir

        // İlk spawn noktaları
        sonrakiEngelZ = cocuk.position.z + 12f;
        sonrakiAltinZ = cocuk.position.z + altinMesafe;
        sonrakiMiknatisZ = cocuk.position.z + miknatisMesafe;

        // Havuz
        uretme(altin, 10, altinlar);
        uretme(miknatis, 3, miknatislar);

        score_value.text = puan.ToString();

        if (hedefText != null)
            hedefText.text = sonrakiFazSkoru.ToString();

        FazDegisti();
        engelRoutine = StartCoroutine(EngelLoop());
    }

    void Update()
    {
        if (cocuk == null) return;

        // ===== ENGEL SPAWN =====
        if (cocuk.position.z >= sonrakiEngelZ)
        {
            engel_uret();

            float mesafe = 12f;

            if (faz == 2) mesafe = 11f;
            else if (faz == 3) mesafe = 10f;
            else if (faz >= 4) mesafe = 9f;

            sonrakiEngelZ += mesafe;
        }

        // ===== ALTIN SPAWN =====
        if (cocuk.position.z >= sonrakiAltinZ)
        {
            altin_uret();

            // Faz arttıkça daha seyrek
            float dinamikMesafe = altinMesafe + (faz * 2f);
            sonrakiAltinZ += dinamikMesafe;
        }

        // ===== MIKNATIS SPAWN =====
        if (cocuk.position.z >= sonrakiMiknatisZ)
        {
            miknatis_uret();

            // Çok daha seyrek
            float dinamikMesafe = miknatisMesafe + (faz * 5f);
            sonrakiMiknatisZ += dinamikMesafe;
        }

        // ===== YOL DÖNGÜ =====
        float yolUzunluk = 10f;
        float tasimaGecikme = 2f;

        if (cocuk.position.z > yol1.position.z + yolUzunluk + tasimaGecikme)
        {
            float ileriZ = Mathf.Max(yol1.position.z, yol2.position.z) + yolUzunluk;

            yol1.position = new Vector3(
                yol1.position.x,
                yol1.position.y,
                ileriZ
            );
        }

        if (cocuk.position.z > yol2.position.z + yolUzunluk + tasimaGecikme)
        {
            float ileriZ = Mathf.Max(yol1.position.z, yol2.position.z) + yolUzunluk;

            yol2.position = new Vector3(
                yol2.position.x,
                yol2.position.y,
                ileriZ
            );
        }
    }




    public void puan_arttir(int deger)
    {
        puan += deger;
        score_value.text = puan.ToString();

      //  var hedef = FindObjectOfType<HedefSayac>();
        //if (hedef != null)
        //{
          //  hedef.PuanEkle(puan);
        //}

        int hs = PlayerPrefs.GetInt("HighScore", 0);

        if (puan > hs)
        {
            highScore = puan;
            PlayerPrefs.SetInt("HighScore", puan);
            PlayerPrefs.Save();

            PlayFabHighScoreManager.Instance.SendHighScore(puan);

            if (highScore_value != null)
                highScore_value.text = highScore.ToString();
        }

        // =========================
        // YENİ FAZ KONTROLÜ (GARANTİLİ)
        // =========================

        if (puan >= sonrakiFazSkoru)
        {
            faz++;
            sonrakiFazSkoru += fazArtisMiktari;

            FazEngelleriniGuncelle();
            FazDegisti();

            if (hedefText != null)
            {
                hedefText.text = sonrakiFazSkoru.ToString();
            }
        }

      //  int sonrakiHedef = ((puan / 1000) + 1) * 1000;
      //  hedefText.text = sonrakiHedef.ToString();

    }

    void engel_uret()
    {
        Debug.Log("Pasif engel var mı?");
        Debug.Log("Toplam: " + digerleri.Count);

        int pasifSayisi = 0;
        foreach (GameObject g in digerleri)
        {
            if (!g.activeSelf)
                pasifSayisi++;
        }

        Debug.Log("Pasif sayısı: " + pasifSayisi);

        if (digerleri.Count == 0) return;

        int rast = Random.Range(0, digerleri.Count);

        if (digerleri[rast].tag == "miknatis" && Random.Range(0, 4) != 0)
            return;

        if (digerleri[rast].activeSelf == false)
        {
            SpawnNesne(digerleri[rast]);
        }
        else
        {
            foreach (GameObject nesne in digerleri)
            {
                if (nesne.activeSelf == false)
                {
                    SpawnNesne(nesne);
                    return;
                }
            }
        }
    }

    void SpawnNesne(GameObject nesne)
    {
        nesne.SetActive(true);

        
        ArabaKorna korna = nesne.GetComponent<ArabaKorna>();
        if (korna != null)
            korna.cocuk = cocuk;

        int rastgele = Random.Range(0, 2);
        float xPos = (rastgele == 0) ? -0.5f : -3.0f;

        Vector3 pos = nesne.transform.position;

        nesne.transform.position =
            new Vector3(xPos, pos.y, cocuk.position.z + 10.0f);

        if (nesne.tag == "miknatis")
        {
            if (cocuk.GetComponent<karakter_kontrol>().miknatis_alindi)
                nesne.SetActive(false);
        }
    }

    public void tekrar_oyna()
    {
        Debug.Log("TEKRAR BASILDI");

        if (AdsManager.Instance == null)
        {
            RestartGame();
            return;
        }

        //  Rewarded varsa
        if (!rewardUsed && AdsManager.Instance.IsRewardedReady())
        {
            AdsManager.Instance.ShowRewarded(
                onReward: () =>
                {
                    rewardUsed = true;
                    YedektenDon();
                },
                onFail: TryInterstitialOrContinue
            );
            return;
        }

        //  Rewarded yoksa interstitial dene
        TryInterstitialOrContinue();
    
    }

    void TryInterstitialOrContinue()
    {
        Time.timeScale = 1f;

        if (AdsManager.Instance != null && AdsManager.Instance.IsInterstitialReady())
        {
            AdsManager.Instance.ShowInterstitial(() =>
            {
                YedektenDon();
            });
        }
        else
        {
            RestartGame();
        }
    }

    public void Anamenu()
    {
        Time.timeScale = 1f;

        if (AdsManager.Instance != null && AdsManager.Instance.IsInterstitialReady())
        {
            AdsManager.Instance.ShowInterstitial(() =>
            {
                SceneManager.LoadScene("MainMenu");
            });
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }



    public void devam_et()
    {

        oyunu_durdur_pnl.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void oyunu_durdur()
    {
        oyunu_durdur_pnl.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void oyundan_cik()
    {
        Application.Quit();
    }


    void altin_uret()
    {
        GameObject a = altinlar[Random.Range(0, altinlar.Count)];

        float ileriMesafe = 25f; // karakterden 25 birim ileri

        float spawnZ = cocuk.position.z + ileriMesafe;

        float xPoz = Random.value < 0.5f ? solX : sagX;

        a.transform.position = new Vector3(
            xPoz,
            -3f,
            spawnZ
        );

        a.SetActive(true);
    }

    void uretme(GameObject nesne, int miktar, List<GameObject> liste)
    {
        for (int i = 0; i < miktar; i++)
        {
            GameObject yeni_nesne = Instantiate(nesne);
            yeni_nesne.SetActive(false);
            liste.Add(yeni_nesne);

            // Prefabın gerçek Y offsetini kaydet
            yOffset[yeni_nesne] = yeni_nesne.transform.position.y;
        }
    }


    public void OyunuBitir()
    {
        yedekPuan = puan;
        yedekFaz = faz;
        yedekSonrakiFaz = sonrakiFazSkoru;

        oyun_bitti_paneli.SetActive(true);
         Time.timeScale = 0f;
                   
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scenes/oyun");
    }

    void TryInterstitialOrStart()
    {
        if (AdsManager.Instance != null && AdsManager.Instance.IsInterstitialReady())
        {
            AdsManager.Instance.ShowInterstitial(() =>
            {
                RestartGame();
            });
        }
        else
        {
            RestartGame();
        }
    }


  public  void FazDegisti()
  {
        int gosterilecekFaz = ((faz - 1) % maxFaz) + 1;

        SkyManager sky = FindObjectOfType<SkyManager>();
        if (sky != null)
            sky.FazDegisti(gosterilecekFaz);

        FazEngelleriniGuncelle();
    }

    void FazEngelleriniGuncelle()
    {
        foreach (GameObject g in digerleri)
            g.SetActive(false);

        digerleri.Clear();

        int aktifFazNo = ((faz - 1) % maxFaz) + 1;

        switch (aktifFazNo)
        {
            case 1: aktifFaz = faz1Engeller; break;
            case 2: aktifFaz = faz2Engeller; break;
            case 3: aktifFaz = faz3Engeller; break;
            case 4: aktifFaz = faz4Engeller; break;
            default: aktifFaz = faz4Engeller; break;
        }

        if (aktifFaz == null || aktifFaz.Length == 0)
            return;

        foreach (GameObject prefab in aktifFaz)
            uretme(prefab, 3, digerleri);
    }

    void YedektenDon()
    {
        StopAllCoroutines();

        Time.timeScale = 1f;

        Rigidbody rb = cocuk.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            cocuk.position += new Vector3(0f, 0f, 2f);
        }

        sonrakiEngelZ = cocuk.position.z + 10f;

        if (engelRoutine != null)
        {
            StopCoroutine(engelRoutine);
            engelRoutine = null;
        }

        engelRoutine = StartCoroutine(EngelLoop());

        InvokeRepeating("altin_uret", 0.0f, 1.0f);
        InvokeRepeating("miknatis_uret", 5.0f, 12.0f);

        oyun_bitti_paneli.SetActive(false);
    }


    void miknatis_uret()
    {
        GameObject a = altinlar[Random.Range(0, altinlar.Count)];

        float ileriMesafe = 40f; // karakterden 25 birim ileri

        float spawnZ = cocuk.position.z + ileriMesafe;

        float xPoz = Random.value < 0.5f ? solX : sagX;

        a.transform.position = new Vector3(
            xPoz,
           -3f,
            spawnZ
        );

        a.SetActive(true);
    }


    IEnumerator SafeResume()
    {
        // Oyun dursun
        Time.timeScale = 0f;

        Rigidbody rb = cocuk.GetComponent<Rigidbody>();

        // Karakter hareketini geçici kapat
        karakter_kontrol kk = cocuk.GetComponent<karakter_kontrol>();
        kk.enabled = false;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // Pozisyonları kesin set et
        yol1.position = yol1BaslangicPos;
        yol2.position = yol2BaslangicPos;
        cocuk.position = cocukBaslangicPos + Vector3.up * 0.1f;

        Physics.SyncTransforms();

        // 1 frame bekle
        yield return null;

        // Rigidbody aç
        rb.isKinematic = false;

        Physics.SyncTransforms();

        // 1 frame daha bekle
        yield return null;

        // Karakter kontrolü geri aç
        kk.enabled = true;

        // ŞİMDİ oyunu başlat
        Time.timeScale = 1f;

        Debug.Log("RESUME TAMAM – OYUN BASLADI");
    }

   


    IEnumerator EngelLoop()
    {
        while (true)
        {
            if (cocuk.position.z >= sonrakiEngelZ)
            {
                engel_uret();

                float bekleme = 3.5f;

                if (faz == 2) bekleme = 3.0f;
                else if (faz == 3) bekleme = 2.5f;
                else if (faz >= 4) bekleme = 2.0f;

                minEngelMesafe = bekleme * 4f;
                sonrakiEngelZ = cocuk.position.z + minEngelMesafe;
            }

            yield return null;
        }
    }
}

