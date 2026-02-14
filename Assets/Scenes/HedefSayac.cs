using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HedefSayac : MonoBehaviour
{
    public TMP_Text hedefText;

    public int hedef = 50;     // ilk hedef
    public int hedefAdim = 50; // kaç kaç artacak

    void Start()
    {
        GuncelleYazi();
    }

    public void PuanEkle(int toplamSkor)
    {
        // skor hedefi geçtiyse yeni hedefe geç
        while (toplamSkor >= hedef)
        {
            HedefTamamlandi();
            hedef += hedefAdim;
        }

        GuncelleYazi();
    }

    void GuncelleYazi()
    {
        hedefText.text = hedef.ToString();
    }

    void HedefTamamlandi()
    {
        Debug.Log("HEDEF BİTTİ → yeni hedef: " + hedef);

        var y = FindObjectOfType<yonetici>();
        if (y != null)
        {
            y.faz++;
            y.FazDegisti();
        }
    }
}