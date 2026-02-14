using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagDatabase : MonoBehaviour
{
    [System.Serializable]
    public class FlagEntry
    {
        public string countryCode;   // "tr", "de", "us"
        public Sprite flagSprite;
    }

    public List<FlagEntry> flags = new List<FlagEntry>();

    [Header("Fallback")]
    public Sprite defaultFlag; //  OYUN SÝMGESÝ

    private Dictionary<string, Sprite> lookup;

    void Awake()
    {
        lookup = new Dictionary<string, Sprite>();
        HashSet<string> usedCodes = new HashSet<string>();

        foreach (var f in flags)
        {
            if (string.IsNullOrEmpty(f.countryCode) || f.flagSprite == null)
                continue;

            string key = f.countryCode.ToLower();

            // Duplicate country code kontrolü
            if (!usedCodes.Add(key))
            {
                Debug.LogWarning($"Duplicate country code detected: '{key}'", this);
                continue; // ikinciyi ekleme
            }

            lookup.Add(key, f.flagSprite);
        }
    }

    public Sprite GetFlag(string code)
    {
        if (string.IsNullOrEmpty(code))
            return defaultFlag;

        code = code.ToLower();

        if (lookup.ContainsKey(code))
            return lookup[code];

        //  BULUNAMAZSA
        return defaultFlag;
    }
}