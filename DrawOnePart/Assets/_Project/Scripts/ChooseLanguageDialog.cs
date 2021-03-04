using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChooseLanguageDialog : MonoBehaviour
{
    [SerializeField] TMP_Text textCurrentLanguage;

    public void NextLanguage()
    {
        string language = LocalizeSQL.CurrentLanguage();
        switch (language)
        {
            case SupportedLanguage.Vietnamese:
                language = SupportedLanguage.Arabic;
                break;
            case SupportedLanguage.Arabic:
                language = SupportedLanguage.Dutch;
                break;
            case SupportedLanguage.Dutch:
                language = SupportedLanguage.French;
                break;
            case SupportedLanguage.French:
                language = SupportedLanguage.German;
                break;
            case SupportedLanguage.German:
                language = SupportedLanguage.Japanese;
                break;
            case SupportedLanguage.Japanese:
                language = SupportedLanguage.Korean;
                break;
            case SupportedLanguage.Korean:
                language = SupportedLanguage.Portuguese;
                break;
            case SupportedLanguage.Portuguese:
                language = SupportedLanguage.English;
                break;
            default:
                language = SupportedLanguage.Vietnamese;
                break;
        }
        LocalizeSQL.SetLanguage(language);
    }

    public void SetTextDisplayCurrentLanguage()
    {
        textCurrentLanguage.text = LocalizeSQL.CurrentLanguage();
    }
}
