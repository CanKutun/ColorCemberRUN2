using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SkyManager : MonoBehaviour
{
    public Material faz1Sky;
    public Material faz2Sky;
    public Material faz3Sky;
    public Material faz4Sky;

    public GameObject rain;
    public GameObject snow;

    public GameObject[] faz1Objs;
    public GameObject[] faz2Objs;
    public GameObject[] faz3Objs;
    public GameObject[] faz4Objs;

    void TumFazlariKapat()
    {
        GameObject[][] tumFazlar = { faz1Objs, faz2Objs, faz3Objs, faz4Objs };

        foreach (var faz in tumFazlar)
        {
            foreach (var obj in faz)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }

    void FazAc(GameObject[] aktifFaz)
    {
        foreach (var obj in aktifFaz)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    int aktifFaz = 1;
    public void FazDegisti(int faz)
    {
        Debug.Log("SKY FAZ: " + faz);
        // 1-4 arasýnda döndür
        aktifFaz = ((faz - 1) % 4) + 1;

        FazUygula();
    }

    void Update()
    {
        
    }

    void FazUygula()
    {
        rain.SetActive(false);
        snow.SetActive(false);

        foreach (var o in faz1Objs) o.SetActive(false);
        foreach (var o in faz2Objs) o.SetActive(false);
        foreach (var o in faz3Objs) o.SetActive(false);
        foreach (var o in faz4Objs) o.SetActive(false);

        switch (aktifFaz)
        {
            case 1:
                RenderSettings.skybox = faz1Sky;
                foreach (var o in faz1Objs) o.SetActive(true);
                break;

            case 2:
                RenderSettings.skybox = faz2Sky;
                foreach (var o in faz2Objs) o.SetActive(true);
                break;

            case 3:
                RenderSettings.skybox = faz3Sky;
                rain.SetActive(true);
                foreach (var o in faz3Objs) o.SetActive(true);
                break;

            case 4:
                RenderSettings.skybox = faz4Sky;
                snow.SetActive(true);
                foreach (var o in faz4Objs) o.SetActive(true);
                break;
        }
    }
}