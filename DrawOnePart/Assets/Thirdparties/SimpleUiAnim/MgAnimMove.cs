using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MgAnimMove : MgAnimBase
{
    [SerializeField] Ease easeType;
    [SerializeField] Vector2 startPos;
    [SerializeField] Vector2 endPos;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Show(bool immediately = false)
    {
        base.Show(immediately);
        m_RectTransform.anchoredPosition = startPos;
        if (!immediately)
        {
            m_RectTransform.DOAnchorPos(endPos, AnimTime).SetEase(easeType).SetUpdate(true);
        }
        else
        {
            m_RectTransform.DOAnchorPos(endPos, 0f).SetUpdate(true);
        }
    }

    public override void Hide(bool immediately = false)
    {
        base.Hide();
        if (!immediately)
        {
            m_RectTransform.DOAnchorPos(startPos, AnimTime).SetEase(easeType).SetUpdate(true);
        }
        else
        {
            m_RectTransform.DOAnchorPos(startPos, 0f).SetUpdate(true);
        }
    }
}
