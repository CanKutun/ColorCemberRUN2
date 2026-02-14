using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayfabLogin : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Login();
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request,
            result =>
            {
                Debug.Log("Login OK");
                ForceUpdateCountry();
            },
            error =>
            {
                Debug.LogError(error.GenerateErrorReport());
            }
        );
    }

    void ForceUpdateCountry()
    {
        string countryCode = GetCountryCode();

        PlayFabClientAPI.UpdateUserData(
            new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    { "country", countryCode }
                },
                Permission = UserDataPermission.Public
            },
            success =>
            {
                Debug.Log("Country updated PUBLIC: " + countryCode);
            },
            error =>
            {
                Debug.LogError("Country update failed: " + error.GenerateErrorReport());
            }
        );
    }

    string GetCountryCode()
    {
        try
        {
            return RegionInfo.CurrentRegion.TwoLetterISORegionName.ToLower();
        }
        catch
        {
            return "xx";
        }
    }

}