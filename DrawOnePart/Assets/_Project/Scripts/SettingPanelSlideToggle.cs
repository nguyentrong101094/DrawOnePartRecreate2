using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SettingPanelSlideToggle : MonoBehaviour
{
    [SerializeField] MgObject mgObject;
    bool isAnimating;
    bool isShow;

    public void Toggle()
    {
        if (isAnimating) return;
        isShow = !isShow;
        Toggle(isShow);
    }

    void Toggle(bool value)
    {
        if (value)
        {
            mgObject.gameObject.SetActive(true);
            StartCoroutine(CoShow());
        }
        else
        {
            StartCoroutine(CoHide());
        }
    }

    IEnumerator CoShow()
    {
        isAnimating = true;
        mgObject.Show();
        yield return new WaitForSecondsRealtime(mgObject.GetAnimBase().AnimTime);
        isAnimating = false;
    }

    IEnumerator CoHide()
    {
        isAnimating = true;
        mgObject.Hide();
        yield return new WaitForSecondsRealtime(mgObject.GetAnimBase().AnimTime);
        mgObject.gameObject.SetActive(false);
        isAnimating = false;
    }

    private void Reset()
    {
        mgObject = GetComponent<MgObject>();
    }
}
