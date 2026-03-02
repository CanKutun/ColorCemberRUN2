using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class donmek : MonoBehaviour
{

    string isim;


    float deger = 150.0f;


    void Start()
    {
        isim = gameObject.tag;    
    }

    
    void Update()
    {
        if (isim == "miknatis")
        {
            transform.Rotate(0f, deger * Time.deltaTime, 0f, Space.World);
        }

        if (isim == "altin")
        {
            transform.Rotate(0f,deger*Time.deltaTime, 0f, Space.World);
        }

    }
}
