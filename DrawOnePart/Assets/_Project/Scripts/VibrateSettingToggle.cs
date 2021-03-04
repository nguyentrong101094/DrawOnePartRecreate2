using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VibrateSettingToggle : MonoBehaviour
{
    string prefKey => Const.PREF_VIBRATION;
    int defaultValue => Const.PREF_VIBRATION_DEFAULT;
    //public enum Type { Int = 0, Float = 1, String = 2 }
    Toggle m_Toggle;

    private void Start()
    {
        m_Toggle = GetComponent<Toggle>();
        m_Toggle.isOn = PlayerPrefs.GetInt(prefKey, defaultValue) == 1;
        m_Toggle.onValueChanged.AddListener(OnValueChange);
        /*switch (type)
        {
            case Type.Int:
                m_Toggle = PlayerPrefs.GetInt(prefKey, int.Parse(defaultValue));
                break;
            case Type.Float:
                m_Toggle = PlayerPrefs.GetFloat(prefKey, float.Parse(defaultValue));
                break;
            case Type.String:
                m_Toggle = PlayerPrefs.GetString(prefKey, defaultValue);
                break;
        }*/
    }

    void OnValueChange(bool value)
    {
        PlayerPrefs.SetInt(prefKey, value ? 1 : 0);
    }
}
