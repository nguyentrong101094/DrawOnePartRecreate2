using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameDatabase : MonoBehaviour
{
    public static string databaseName = "simpledata.bytes";

    public static DataService Service
    {
        get
        {
            if(Instance.service == null)
            {
                GetRemoteConfig();
            }
            return Instance.service;
        }
    }
    DataService service;

    public static GameDatabase Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gO = Resources.Load<GameObject>("Database");
                instance = Instantiate(gO).GetComponent<GameDatabase>();
            }
            return instance;
        }
    }

    static GameDatabase instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        //service = new DataService(databaseName);
    }

    public static DataService GetDataServiceInEditMode()
    {
        if (Application.isPlaying)
            Debug.LogError("Do not call this in play mode");
        return new DataService(databaseName);
    }

    public static void GetRemoteConfig()
    {
        bool useV4 = FirebaseRemoteConfigHelper.GetBool(Const.RMCF_USE_DATABASE_V4, true);
        if (useV4)
        {
            databaseName = "simpledata.bytes";
        }
        else
        {
            databaseName = "simpledatav3.bytes";
        }
        Instance.service = new DataService(databaseName);
        Debug.Log($"Use database v4: {useV4}");
    }

    private void ToConsole(IEnumerable<DrawDotData> people)
    {
        foreach (var person in people)
        {
            ToConsole(person.ToString());
        }
    }

    private void ToConsole(string msg)
    {
        //DebugText.text += System.Environment.NewLine + msg;
        Debug.Log(msg);
    }

    private void OnApplicationQuit()
    {
        service.CloseConnection();
    }
}