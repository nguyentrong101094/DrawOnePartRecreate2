using Gront;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDot : MonoBehaviour
{
    [SerializeField] SpriteRenderer m_SpriteRender;
    public bool isCorrect;

    public void SetCorrect()
    {
        if (DebugManager.IsDebugMode())
        {
            m_SpriteRender.color = m_SpriteRender.color.SetColorKeepAlpha(Color.green);
        }

        isCorrect = true;
    }

    public void OnReset(object sender, System.EventArgs args)
    {
        if (DebugManager.IsDebugMode())
        {
            m_SpriteRender.color = m_SpriteRender.color.SetColorKeepAlpha(Color.white);
        }

        isCorrect = false;
    }

    public void ToggleSpriteRenderer(bool value)
    {
        m_SpriteRender.transform.parent.gameObject.SetActive(value);
    }

    internal void Setup(float checkRadius)
    {
        if (DebugManager.IsDebugMode())
            m_SpriteRender.transform.parent.localScale = Vector3.one * checkRadius;
    }
}
