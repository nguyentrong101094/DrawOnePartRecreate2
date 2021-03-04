using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RateSlider : MonoBehaviour
{
    [SerializeField] List<GameObject> stars;
    [SerializeField] Slider m_Slider;

    private void Start()
    {
        OnSliderChange(m_Slider.value);
    }

    public void OnSliderChange(float value)
    {
        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].SetActive(i <= (int)value - 1);
        }
    }
}
