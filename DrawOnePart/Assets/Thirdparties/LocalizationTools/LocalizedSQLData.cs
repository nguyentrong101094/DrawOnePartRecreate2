using SQLite4Unity3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedSQLData
{
    [PrimaryKey]
    public string id { get; set; }
    public string vietnamese { get; set; }
    public string english { get; set; }
    public string arabic { get; set; }
    public string french { get; set; }
    public string dutch { get; set; }
    public string german { get; set; }
    public string japanese { get; set; }
    public string korean { get; set; }
    public string portuguese { get; set; }
}

public static class LocalizeSQL
{
    public const string PREF_LANGUAGE = "PP_KEY_LANGUAGE";

    public static string GetString(string id, string language)
    {
        string ret = string.Empty;

        LocalizedSQLData localizeData = GameDatabase.Service.GetLocalize(id);
        if (localizeData == null)
        {
            Debug.LogError(string.Format("Localize missing ID {0}", id));
            return id;
        }
        switch (language)
        {
            case SupportedLanguage.Vietnamese:
                ret = localizeData.vietnamese;
                break;
            case SupportedLanguage.Arabic:
                ret = localizeData.arabic;
                break;
            case SupportedLanguage.Dutch:
                ret = localizeData.dutch;
                break;
            case SupportedLanguage.French:
                ret = localizeData.french;
                break;
            case SupportedLanguage.German:
                ret = localizeData.german;
                break;
            case SupportedLanguage.Japanese:
                ret = localizeData.japanese;
                break;
            case SupportedLanguage.Korean:
                ret = localizeData.korean;
                break;
            case SupportedLanguage.Portuguese:
                ret = localizeData.portuguese;
                break;
            default:
                ret = localizeData.english;
                break;
        }
        ret = ret.Replace("\\n", "\n");
        if (string.IsNullOrEmpty(ret))
        {
            Debug.LogWarning($"Localize language is empty. ID: {id}, language: {language}");
            ret = id;
        }
        return ret;
    }

    public static string GetString(string id)
    {
        //return id;
        return GetString(id, CurrentLanguage());
    }

    public static string LC(this string id) { return GetString(id); }

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
                case SystemLanguage.Arabic:
                    language = SupportedLanguage.Arabic;
                    break;
                case SystemLanguage.Dutch:
                    language = SupportedLanguage.Dutch;
                    break;
                case SystemLanguage.French:
                    language = SupportedLanguage.French;
                    break;
                case SystemLanguage.German:
                    language = SupportedLanguage.German;
                    break;
                case SystemLanguage.Japanese:
                    language = SupportedLanguage.Japanese;
                    break;
                case SystemLanguage.Vietnamese:
                    language = SupportedLanguage.Vietnamese;
                    break;
                case SystemLanguage.Korean:
                    language = SupportedLanguage.Korean;
                    break;
                case SystemLanguage.Portuguese:
                    language = SupportedLanguage.Portuguese;
                    break;
                default:
                    language = SupportedLanguage.English;
                    break;
            }
            SetLanguage(language);
        }

        return language;
    }

    /*public static int CurrentLanguageID()
    {
        int language = PlayerPrefs.GetInt(PREF_LANGUAGE, -1);
        if (language == -1)
        {
            language = (int)Application.systemLanguage;
        }
        return language;
    }

    static void SetCurrentLanguageID(int id) { PlayerPrefs.SetInt(PREF_LANGUAGE, id); PlayerPrefs.Save(); }*/
}