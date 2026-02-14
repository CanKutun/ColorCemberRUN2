using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class yonetici : MonoBehaviour
{
    int yedekPuan;
    int yedekFaz;
    int yedekSonrakiFaz;
    public int puan;
    public int highScore;

    public int faz = 1;
    public int sonrakiFazSkoru = 1000;


    public TextMeshProUGUI score_value;
    public TextMeshProUGUI highScore_value;

    public GameObject altin;
    public GameObject miknatis;

    public GameObject kutuk;
    public GameObject tas;
    public GameObject araba;

    [Header("Faz Engelleri")]
    public GameObject[] faz1Engeller;
    public GameObject[] faz2Engeller;
    public GameObject[] faz3Engeller;
    public GameObject[] faz4Engeller;

    List<GameObject> altinlar;
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
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScore_value.text = highScore.ToString();
        altinlar = new List<GameObject>();
        digerleri = new List<GameObject>();
        cocuk = GameObject.Find("cocuk").transform;

        uretme(altin, 10, altinlar);

        uretme(miknatis, 3, digerleri);
        uretme(kutuk, 3, digerleri);
        uretme(tas, 3, digerleri);
        uretme(araba, 3, digerleri);

        InvokeRepeating("altin_uret", 0.0f, 1.0f);
        InvokeRepeating("engel_uret", 1.5f, 3.0f);

        score_value.text = puan.ToString();

           FazEngelleriniGuncelle();
    }


    public void puan_arttir(int deger)
    {
        puan += deger;
        score_value.text = puan.ToString();

        var hedef = FindObjectOfType<HedefSayac>();

        if (hedef != null)
        {
           
            hedef.PuanEkle(puan);
        }
        
        int hs = PlayerPrefs.GetInt("HighScore", 0);

        if (puan > hs)
        {
            PlayerPrefs.SetInt("HighScore", puan);
            PlayerPrefs.Save();

            PlayFabHighScoreManager.Instance.SendHighScore(puan);

            // Eğer high score text'in varsa
            if (highScore_value != null)
                highScore_value.text = puan.ToString();
        }

        // FAZ KONTROLÜ
        if (puan >= sonrakiFazSkoru)
        {
            faz++;
            sonrakiFazSkoru += 1000;
            FazDegisti();

            FazEngelleriniGuncelle();
        }
    }

    void engel_uret()
    {
        int rast = Random.Range(0, digerleri.Count);

        if (digerleri[rast].activeSelf == false)
        {
            digerleri[rast].SetActive(true);

            //  Eğer bu nesne arabaysa, üzerindeki ArabaKorna'ya cocuk'u ata
            ArabaKorna korna = digerleri[rast].GetComponent<ArabaKorna>();
            if (korna != null)
            {
                korna.cocuk = cocuk;
            }

            int rastgele = Random.Range(0, 2);

            if (rastgele == 0)
            {
                digerleri[rast].transform.position = new Vector3(-0.5f, -3.0f, cocuk.position.z + 10.0f);
            }

            if (rastgele == 1)
            {
                digerleri[rast].transform.position = new Vector3(-3.0f, -3.0f, cocuk.position.z + 10.0f);
            }

            if (digerleri[rast].tag == "miknatis")
            {
                if (cocuk.gameObject.GetComponent<karakter_kontrol>().miknatis_alindi == true)
                {
                    digerleri[rast].SetActive(false);
                }
            }

            if (digerleri[rast].name != "miknatis(Clone)")
            {
                Vector3 eskiPos = digerleri[rast].transform.position;

                eskiPos.y = -3.6f;

                digerleri[rast].transform.position = eskiPos;
            }

        }
        else
        {
            foreach (GameObject nesne in digerleri)
            {
                if (nesne.activeSelf == false)
                {
                    nesne.SetActive(true);

                    //  Eğer bu nesne arabaysa, üzerindeki ArabaKorna'ya cocuk'u ata
                    ArabaKorna korna = nesne.GetComponent<ArabaKorna>();
                    if (korna != null)
                    {
                        korna.cocuk = cocuk;
                    }

                    int rastgele_2 = Random.Range(0, 2);

                    if (rastgele_2 == 0)
                    {
                        nesne.transform.position = new Vector3(-0.5f, -3.0f, cocuk.position.z + 10.0f);
                    }

                    if (rastgele_2 == 1)
                    {
                        nesne.transform.position = new Vector3(-3.0f, -3.0f, cocuk.position.z + 10.0f);
                    }

                    if (nesne.tag == "miknatis")
                    {
                        if (cocuk.gameObject.GetComponent<karakter_kontrol>().miknatis_alindi == true)
                        {
                            nesne.SetActive(false);
                        }
                    }

                    if (digerleri[rast].name != "miknatis(Clone)")
                    {
                        Vector3 eskiPos = digerleri[rast].transform.position;

                        eskiPos.y = -3.6f;

                        digerleri[rast].transform.position = eskiPos;
                    }

                    return;
                }
            }
        }
    }

    public void tekrar_oyna()
    {
        if (AdsManager.Instance == null)
        {
            RestartGame();
            return;
        }

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
        }
        else
        {
            TryInterstitialOrContinue();
        }
    }


    void TryInterstitialOrContinue()
    {
        //  GEÇİŞ VARSA  DEVAM
        if (AdsManager.Instance.IsInterstitialReady())
        {
            AdsManager.Instance.ShowInterstitial(() =>
            {
                YedektenDon(); // kaldığın yerden devam
            });
        }
        else
        {
            //  REKLAM YOK  SIFIRDAN
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
        Debug.Log("Yeni faz: " + faz);

        SkyManager sky = FindObjectOfType<SkyManager>();
        if (sky != null)
            sky.FazDegisti(faz);

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
    }

    void FazEngelleriniGuncelle()
    {
        // Eski engelleri kapat
        foreach (GameObject g in digerleri)
            g.SetActive(false);

        digerleri.Clear();

        GameObject[] secilen = faz1Engeller;

        switch (faz)
        {
            case 1: secilen = faz1Engeller; break;
            case 2: secilen = faz2Engeller; break;
            case 3: secilen = faz3Engeller; break;
            case 4: secilen = faz4Engeller; break;
        }

        foreach (GameObject prefab in secilen)
        {
            uretme(prefab, 3, digerleri);
        }

        Debug.Log("Faz engelleri güncellendi: " + faz);
    }

    void YedektenDon()
    {
        Debug.Log("YEDEKTEN DON CALISTI");
        puan = yedekPuan;
        faz = yedekFaz;
        sonrakiFazSkoru = yedekSonrakiFaz;

        score_value.text = puan.ToString();

        SkyManager sky = FindObjectOfType<SkyManager>();
        if (sky != null)
            sky.FazDegisti(faz);
        FazEngelleriniGuncelle();

        oyun_bitti_paneli.SetActive(false);
        StartCoroutine(GecikmeliDevam());
    }

    IEnumerator GecikmeliDevam()
    {
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
    }

}