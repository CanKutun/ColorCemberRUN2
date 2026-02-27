using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Microsoft.Win32.SafeHandles;
using UnityEngine.Localization.Settings;

public class yonetici : MonoBehaviour
{
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

    public int puan;
    public int highScore;


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

    float sonrakiMiknatisZamani = 0f;

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
        cocukBaslangicPos = cocuk.position;

        uretme(altin, 10, altinlar);
        uretme(miknatis, 3, miknatislar);

        InvokeRepeating("altin_uret", 0.0f, 1.0f);
        InvokeRepeating("miknatis_uret", 5.0f, 12.0f);

        score_value.text = puan.ToString();

        if (hedefText != null)
            hedefText.text = sonrakiFazSkoru.ToString();

        FazEngelleriniGuncelle(); // engel spawn buradan başlar

        yol1BaslangicPos = yol1.transform.position;
        yol2BaslangicPos = yol2.transform.position;

        Debug.Log("HighScore e bagli obje " + highScore_value.gameObject.name);
        Debug.Log("LABEL OBJ INSTANCE ID: " + GameObject.Find("highscore_label").GetInstanceID());
        Debug.Log("VALUE OBJ INSTANCE ID: " + highScore_value.gameObject.GetInstanceID());

       
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
        foreach (GameObject altin in altinlar)
        {
            if (altin.activeSelf == false)
            {
                altin.SetActive(true);

                int rastgele = Random.Range(0, 2);

                if (rastgele == 0)
                {
                    altin.transform.position = new Vector3(-0.5f, -3.0f, cocuk.position.z + 10.0f);
                }

                if (rastgele == 1)
                {
                    altin.transform.position = new Vector3(-3.0f, -3.0f, cocuk.position.z + 10.0f);
                }

                return;
            }
        }
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
        Debug.Log("GERÇEK FAZ = " + faz);
        Debug.Log("Yeni faz: " + faz);
        Debug.Log("Aktif Faz: " + faz);

        SkyManager sky = FindObjectOfType<SkyManager>();
        if (sky != null)
        {
            int gosterilecekFaz = Mathf.Min(faz, maxFaz);
            sky.FazDegisti(gosterilecekFaz);
        }

        if (faz == 2)
        {
            CancelInvoke("engel_uret");
            InvokeRepeating("engel_uret", 1f, 2.5f);
        }
        else if (faz == 3)
        {
            CancelInvoke("engel_uret");
            InvokeRepeating("engel_uret", 1f, 2f);
        }

       

        FazEngelleriniGuncelle();
    }

    void FazEngelleriniGuncelle()
    {
        CancelInvoke("engel_uret");

        foreach (GameObject g in digerleri)
            g.SetActive(false);

        digerleri.Clear();

        
        int aktifFazNo = faz;

        if (faz > maxFaz)
            aktifFazNo = maxFaz;


        switch (aktifFazNo)
        {
            case 1: aktifFaz = faz1Engeller; break;
            case 2: aktifFaz = faz2Engeller; break;
            case 3: aktifFaz = faz3Engeller; break;
            case 4: aktifFaz = faz4Engeller; break;
            default: aktifFaz = faz4Engeller; break;
        }

        if (aktifFaz == null || aktifFaz.Length == 0)
        {
            Debug.LogWarning("Faz " + faz + " boş!");
            return;
        }

        foreach (GameObject prefab in aktifFaz)
            uretme(prefab, 3, digerleri);

        InvokeRepeating("engel_uret", 1.5f, 3.0f);
    }

    void YedektenDon()
    {
        Debug.Log("YEDEKTEN DÖN ÇALIŞTI");
        CancelInvoke("engel_uret");
        CancelInvoke("altin_uret");
        CancelInvoke("miknatis_uret");

        StopAllCoroutines();

        foreach (GameObject g in digerleri)
        {
            g.SetActive(false);
        }

        puan = yedekPuan;
        faz = yedekFaz;
        sonrakiFazSkoru = yedekSonrakiFaz;

        score_value.text = puan.ToString();

        SkyManager sky = FindObjectOfType<SkyManager>();
        if (sky != null)
            sky.FazDegisti(faz);

       

        yol1.transform.position = yol1BaslangicPos;
        yol2.transform.position = yol2BaslangicPos;

        StartCoroutine(SafeResume());

        FazEngelleriniGuncelle();   // engel spawn başlar


        //  BUNLARI EKLE
        InvokeRepeating("altin_uret", 0.0f, 1.0f);
        InvokeRepeating("miknatis_uret", 5.0f, 12.0f);


        oyun_bitti_paneli.SetActive(false);

        highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (highScore_value != null)
            highScore_value.text = highScore.ToString();

        Time.timeScale = 1f;


        var label = GameObject.Find("highscore_label")
    .GetComponent<TextMeshProUGUI>();

        if (LocalizationSettings.SelectedLocale.Identifier.Code == "tr")
            label.text = "REKOR:";
        else
            label.text = "HIGH SCORE:";

        


        Debug.Log("LABEL AFTER RESUME: " + label.text);

    }

    IEnumerator GecikmeliDevam()
    {
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
    }

    void miknatis_uret()
    {
        if (Time.time < sonrakiMiknatisZamani)
            return;

        sonrakiMiknatisZamani = Time.time + Random.Range(8f, 14f);

        foreach (GameObject m in miknatislar)
        {
            if (!m.activeSelf)
            {
                m.SetActive(true);
                m.transform.position = new Vector3(-3f, -3f, cocuk.position.z + 12f);
                return;
            }
        }
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

}