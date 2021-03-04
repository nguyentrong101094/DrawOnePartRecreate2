using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenjinWrapper : MonoBehaviour
{
    //static BaseTenjin InstanceTenjin => Tenjin.getInstance("XVG3NEWKYLFJ9X7XGYRZ1JF32YPXJ1GG");
    void Start()
    {
        TenjinConnect();
        DontDestroyOnLoad(gameObject);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            TenjinConnect();
        }
    }

    public void TenjinConnect()
    {
        // Sends install/open event to Tenjin
        //InstanceTenjin.Connect();
    }

    public static void LogEvent(string name)
    {
        //InstanceTenjin.SendEvent(name);
    }
}
