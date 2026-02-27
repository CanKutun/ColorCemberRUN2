using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(AudioSource))]
public class ArabaKorna : MonoBehaviour
{
    public Transform cocuk;
    public float tetiklemeMesafesi = 4f;
    public float seritTolerance = 0.5f;
    public bool sadeceBirKez = true;

    AudioSource src;
    bool caldi;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        src.loop = false;
        src.playOnAwake = false;

        // Prefab içindeyken atanamazsa, sahnede adı "cocuk" olan nesneyi bul
        if (cocuk == null)
        {
            GameObject go = GameObject.Find("cocuk");
            if (go != null)
                cocuk = go.transform;
        }
    }

    void Update()
    {
        if (PlayerPrefs.GetInt("SFX", 1) == 0)
        {
            src.mute = true;
            if (src.isPlaying) src.Stop();
            return;
        }
        else
        {
            src.mute = false;
        }

        // SFX kapalıysa kesinlikle çalmasın
        if (SettingsAudioManager.Instance != null &&
            !SettingsAudioManager.Instance.sfxOn)
        {
            if (src.isPlaying)
                src.Stop();
            return;
        }

        if (sadeceBirKez && caldi) return;
        if (cocuk == null) return;

        float xFark = Mathf.Abs(transform.position.x - cocuk.position.x);
        if (xFark > seritTolerance) return;

        float zFark = transform.position.z - cocuk.position.z;

        if (zFark < tetiklemeMesafesi && zFark > 0)
        {
            if (!src.isPlaying)
                src.Play();

            caldi = true;
        }
    }
}