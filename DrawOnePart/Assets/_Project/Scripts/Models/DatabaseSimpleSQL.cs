using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseSimpleSQL : MonoBehaviour
{
    // reference to our database manager object in the scene
    public SimpleSQL.SimpleSQLManager dbManager;

    static DatabaseSimpleSQL instance;
    public static DatabaseSimpleSQL Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gO = Resources.Load<GameObject>("DatabaseSimpleSQL");
                instance = Instantiate(gO).GetComponent<DatabaseSimpleSQL>();
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public StageData GetStageData(int id)
    {
        var picData = dbManager.QueryFirstRecord<StageData>(out bool recordExists, $"SELECT * FROM StageData WHERE id={id}");
        if (recordExists)
        {
            return picData;
        }
        else
        {
            Debug.LogError($"StageData not found {id}");
            return null;
        }
    }

    public PictureData GetPictureData(string pictureName)
    {
        var picData = dbManager.QueryFirstRecord<PictureData>(out bool recordExists, $"SELECT * FROM PictureData WHERE name='{pictureName}'");
        if (recordExists)
        {
            return picData;
        }
        else
        {
            Debug.LogError($"PictureData not found {pictureName}");
            return null;
        }
    }

    public List<DrawDotData> GetDrawDots(int stageID)
    {
        return dbManager.Query<DrawDotData>($"SELECT * FROM DrawDotData WHERE stage_id={stageID}");
    }

    public void UpdateData(object obj)
    {
        dbManager.UpdateTable(obj);
    }
}
