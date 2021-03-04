using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class SupportedLanguage
{
    public const string Vietnamese = "Vietnamese";
    public const string English = "English";
    public const string Japanese = "Japanese";
    public const string Korean = "Korean";
    public const string Arabic = "Arabic";
    public const string Dutch = "Dutch";
    public const string Portuguese = "Portuguese";
    public const string French = "French";
    public const string German = "German";
}

public class LocalizedData
{
    public string id;
    public string vietnamese;
    public string english;
    public string japanese;
    public string korean;

    public LocalizedData()
    {
    }
}

public class Localizes
{
    public const string PREF_LANGUAGE = "PP_KEY_LANGUAGE";
    static LocalizedData[] datas;

    static Localizes()
    {
        datas = JsonMapper.ToObject<LocalizedData[]>(Resources.Load<TextAsset>("Data/LocalizedData.json").text);
    }

    public static string GetString(string id, string language)
    {
        string ret = string.Empty;
        for (int i = 0; i < datas.Length; i++)
        {
            if (string.Compare(id, datas[i].id) == 0)
            {
                switch (language)
                {
                    case SupportedLanguage.Vietnamese:
                        ret = datas[i].vietnamese.Replace("\\n", "\n");
                        break;
                    case SupportedLanguage.English:
                        ret = datas[i].english.Replace("\\n", "\n");
                        break;
                    case SupportedLanguage.Japanese:
                        ret = datas[i].japanese.Replace("\\n", "\n");
                        break;
                    case SupportedLanguage.Korean:
                        ret = datas[i].korean.Replace("\\n", "\n");
                        break;
                }
            }
        }
        //Debug.LogWarning(string.Format("Localize missing {0} {1}", language, id));
        return string.IsNullOrEmpty(ret) ? id : ret;
    }

    public static string GetString(string id)
    {
        //return id;
        return GetString(id, CurrentLanguage());
    }

    public static void SetLanguage(string language)
    {
        PlayerPrefs.SetString(PREF_LANGUAGE, language); //"PP_KEY_LANGUAGE"
        PlayerPrefs.Save();
        //.Log(CurrentLanguage());
    }

    public static string CurrentLanguage()
    {
        string language = PlayerPrefs.GetString(PREF_LANGUAGE, string.Empty);

        if (string.IsNullOrEmpty(language))
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Japanese:
                    language = SupportedLanguage.Japanese;
                    break;
                case SystemLanguage.Vietnamese:
                    language = SupportedLanguage.Vietnamese;
                    break;
                case SystemLanguage.Korean:
                    language = SupportedLanguage.Korean;
                    break;
                default:
                    language = SupportedLanguage.English;
                    break;
            }
            SetLanguage(language);
        }

        return language;
    }
}
