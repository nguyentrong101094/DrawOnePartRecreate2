using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgObject : MonoBehaviour
{
    MgAnimBase[] m_AnimList;

    private void Awake()
    {
        m_AnimList = GetComponents<MgAnimBase>();
    }

    public void Show(bool immediately = false)
    {
        for (int i = 0; i < m_AnimList.Length; i++)
        {
            m_AnimList[i].Show(immediately);
        }
    }

    public void Hide(bool immediately = false)
    {
        for (int i = 0; i < m_AnimList.Length; i++)
        {
            m_AnimList[i].Hide(immediately);
        }
    }

    public void ToggleShow(bool value) { if (value) Show(); else Hide(); }

    public MgAnimBase GetAnimBase()
    {
        if (m_AnimList.Length > 0) return m_AnimList[0];
        else return null;
    }
}
