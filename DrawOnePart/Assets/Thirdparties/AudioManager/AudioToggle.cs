using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class AudioToggle : MonoBehaviour
{
    Toggle m_Slider;
    [SerializeField] bool isControlSfx;
    [SerializeField] bool isControlBgm;
    bool saveOnChangeValue = true;

    private void Awake()
    {
        m_Slider = GetComponent<Toggle>();
        if (isControlSfx)
        {
            m_Slider.isOn = AudioManager.SfxVolume > 0f;
            m_Slider.onValueChanged.AddListener(SetSfxValue);
        }
        if (isControlBgm)
        {
            m_Slider.isOn = AudioManager.BgmVolume > 0f;
            m_Slider.onValueChanged.AddListener(SetBgmValue);
        }
    }

    public void SetSfxValue(bool value)
    {
        AudioManager.SfxVolume = value ? 1f : 0f;
        if (saveOnChangeValue) AudioManager.Save();
    }

    public void SetBgmValue(bool value)
    {
        AudioManager.BgmVolume = value ? 1f : 0f;
        if (saveOnChangeValue) AudioManager.Save();
    }
}
