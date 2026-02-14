using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class CountryFlag
{
    public string code;   // "tr", "us", "de" gibi
    public Sprite sprite;
}

public class CountryFlagDatabase : MonoBehaviour
{
    public List<CountryFlag> flags = new List<CountryFlag>();
    public Sprite defaultFlag;

    Dictionary<string, Sprite> lookup;

    void Awake()
    {
        lookup = new Dictionary<string, Sprite>();
        foreach (var f in flags)
        {
            if (!string.IsNullOrEmpty(f.code) && f.sprite != null)
                lookup[f.code.ToLower()] = f.sprite;
        }
    }

    public Sprite GetFlag(string code)
    {
        if (string.IsNullOrEmpty(code))
            return defaultFlag;

        code = code.ToLower();
        if (lookup.TryGetValue(code, out var sp))
            return sp;

        return defaultFlag;
    }
}