using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowLoop : MonoBehaviour
{
    public ParticleSystem ps;
    public float rainTime = 30f;
    public float waitTime = 10f;

    void Start()
    {
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        while (true)
        {
            ps.Play();
            yield return new WaitForSeconds(rainTime);

            ps.Stop();
            yield return new WaitForSeconds(waitTime);
        }
    }
}