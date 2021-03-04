using SQLite4Unity3d;
using UnityEngine;
//#if !UNITY_EDITOR
using System.Collections;
using System.IO;
//#endif
using System.Collections.Generic;
using System;

public class DataService
{

    private SQLiteConnection _connection;

    public DataService(string DatabaseName)
    {
        string currentDatabaseVersion = PlayerPrefs.GetString(Const.PREF_EXIST_DATABASE_VERSION_CODE, "");
        bool requireUpdateDatabase = !currentDatabaseVersion.Equals(Application.version);
        Debug.Log($"database:{currentDatabaseVersion} app:{Application.version} update:{requireUpdateDatabase}");
#if UNITY_EDITOR
        var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (requireUpdateDatabase || !File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->
#if UNITY_ANDROID
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath, true);

#endif
            PlayerPrefs.SetString(Const.PREF_EXIST_DATABASE_VERSION_CODE, Application.version);
            PlayerPrefs.Save();
            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);

    }

    public PictureData AddPictureData(string id)
    {
        var existData = GetPictureData(id);
        if (existData != null)
        {
            Debug.LogWarning($"PictureData {id} already exists");
            return existData;
        }
        existData = new PictureData() { name = id };
        _connection.Insert(existData);
        return existData;
    }

    public IEnumerable<DrawDotData> GetDrawDots(int stage_id)
    {
        return _connection.Table<DrawDotData>().Where(x => x.stage_id == stage_id);
    }

    public IEnumerable<StageData> GetAllStageData()
    {
        return _connection.Table<StageData>();
    }

    public IEnumerable<PictureData> GetAllPictureData()
    {
        return _connection.Table<PictureData>();
    }

    public StageData GetStageData(int levelID)
    {
        return _connection.Table<StageData>().Where(x => x.id == levelID).FirstOrDefault();
    }

    public StageData GetStageDataFromPicture(string pictureName)
    {
        var stage = _connection.Find<StageData>(x => x.picture_name == pictureName);
        if (stage == null) { Debug.LogError($"{pictureName} stage data not found"); }
        return stage;
    }

    public PictureData GetPictureData(string id)
    {
        return _connection.Table<PictureData>().Where(x => x.name == id).FirstOrDefault();
    }

    public LocalizedSQLData GetLocalize(string id)
    {
        return _connection.Table<LocalizedSQLData>().Where(x => x.id == id).FirstOrDefault();
    }

    public void UpdateData(object obj)
    {
        _connection.Update(obj);
    }

    public void CloseConnection()
    {
        Debug.Log("Closing connection");
        _connection.Close();
        GC.Collect();
    }

    /*
        public IEnumerable<Person> GetPersons()
        {
            return _connection.Table<Person>();
        }

        public IEnumerable<Person> GetPersonsNamedRoberto()
        {
            return _connection.Table<Person>().Where(x => x.Name == "Roberto");
        }

        public Person GetJohnny()
        {
            return _connection.Table<Person>().Where(x => x.Name == "Johnny").FirstOrDefault();
        }

        public Person CreatePerson()
        {
            var p = new Person
            {
                Name = "Johnny",
                Surname = "Mnemonic",
                Age = 21
            };
            _connection.Insert(p);
            return p;
        }*/
}