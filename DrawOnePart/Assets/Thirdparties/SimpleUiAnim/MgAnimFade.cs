using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class MgAnimFade : MgAnimBase
{
    [SerializeField] Ease easeType;
    CanvasGroup m_CanvasGroup;
    protected override void Awake()
    {
        base.Awake();
        m_CanvasGroup = GetComponent<CanvasGroup>();
    }

    public override void Show(bool immediately = false)
    {
        base.Show(immediately);
        if (!immediately)
        {
            m_CanvasGroup.DOFade(1f, AnimTime).SetEase(easeType).SetUpdate(true);
        }
        else
        {
            m_CanvasGroup.alpha = 1f;
        }
    }

    public override void Hide(bool immediately = false)
    {
        base.Hide();
        if (!immediately)
        {
            m_CanvasGroup.DOFade(0f, AnimTime).SetEase(easeType).SetUpdate(true);
        }
        else
        {
            m_CanvasGroup.alpha = 0f;
        }
    }
}
