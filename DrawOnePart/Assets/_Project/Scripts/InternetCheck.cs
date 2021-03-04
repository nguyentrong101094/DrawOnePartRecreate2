using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetCheck
{
    public static bool InternetReachable()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    /// <summary>
    /// Check if phone turn on Wifi or Data connection. Can be toggled through Firebase Remote Config
    /// </summary>
    /// <returns>If configured to not require internet, always return true. If configured to require internet, return internet state</returns>
    public static bool InternetReachableOptional()
    {
        if (FirebaseRemoteConfigHelper.GetBool(Const.RMCF_REQUIRE_INTERNET, false))
            return Application.internetReachability != NetworkReachability.NotReachable;
        else return true; //skip internet check
    }
}
