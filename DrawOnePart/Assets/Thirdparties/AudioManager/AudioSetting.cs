using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AudioSetting : MonoBehaviour
{
    Slider m_Slider;
    [SerializeField] bool isControlSfx;
    [SerializeField] bool isControlBgm;

    private void Awake()
    {
        m_Slider = GetComponent<Slider>();
        float sliderVal = 0f;
        if (isControlSfx)
        {
            sliderVal = AudioManager.SfxVolume;
            m_Slider.value = sliderVal;
            m_Slider.onValueChanged.AddListener(SetSfxValue);
        }
        if (isControlBgm)
        {
            sliderVal = AudioManager.BgmVolume;
            m_Slider.value = sliderVal;
            m_Slider.onValueChanged.AddListener(SetBgmValue);
        }
    }

    public void SetSfxValue(float value)
    {
        AudioManager.SfxVolume = value;
    }

    public void SetBgmValue(float value)
    {
        AudioManager.BgmVolume = value;
    }
}
