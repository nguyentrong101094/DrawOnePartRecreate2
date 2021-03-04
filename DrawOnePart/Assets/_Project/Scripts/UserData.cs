using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public string version;
    public int hintAcquired = 1;

    internal void Init()
    {
    }
}

public static class User
{
    public delegate void CallbackOnCurrencyChange(int amount);
    public static CallbackOnCurrencyChange callbackOnHintAcquire;
    public static UserData data;

    static User()
    {
        LitJsonHelper.Initialize();
        Load();
    }

    static void Load()
    {
        var textData = PlayerPrefs.GetString("USER_DATA", string.Empty);

        if (!string.IsNullOrEmpty(textData))
        {
            data = JsonMapper.ToObject<UserData>(textData);
        }
        else
        {
            data = new UserData();
            data.Init();

            Save();
        }
    }

    public static void Save()
    {
        data.version = Const.BUILD_VERSION_CODE.ToString();
        var textData = DataToString();

        PlayerPrefs.SetString("USER_DATA", textData);
        PlayerPrefs.Save();
    }

    public static string DataToString() { return JsonMapper.ToJson(data); }

    public static int GetCurrentStage()
    {
        return PlayerPrefs.GetInt(Const.PROGRESS_CURRENT_STAGE, 1);
    }

    public static void SetCurrentStage(int stage)
    {
        PlayerPrefs.SetInt(Const.PROGRESS_CURRENT_STAGE, stage);
    }

    public static void OnStageComplete(int stage)
    {
        SetCurrentStage(stage);
        if (stage >= GetLastUnlockedStage())
        {
            PlayerPrefs.SetInt(Const.PROGRESS_UNLOCKED_STAGE, stage + 1);
        }
    }

    public static int GetLastUnlockedStage()
    {
        return PlayerPrefs.GetInt(Const.PROGRESS_UNLOCKED_STAGE, 1);
    }

    public static int GetContinueStage() //get stage to jump to when use continue button
    {
        return Mathf.Min(GetLastUnlockedStage(), Const.MAX_STAGE);
    }

    public static bool HasReachedMaxStage()
    {
        return GetCurrentStage() > Const.MAX_STAGE;
    }

    public static void AddHint(int number)
    {
        data.hintAcquired += number;
        callbackOnHintAcquire?.Invoke(data.hintAcquired);
        Save();
    }
}
