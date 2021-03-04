using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class MgAnimScale : MgAnimBase
{
    [FormerlySerializedAs("easeType")]
    [SerializeField] Ease showEaseType;
    [SerializeField] Ease hideEaseType;
    protected override void Awake()
    {
        base.Awake();
        transform.localScale = Vector2.zero;
    }

    public override void Show(bool immediately = false)
    {
        base.Show(immediately);
        if (!immediately)
        {
            transform.DOScale(1f, AnimTime).SetEase(showEaseType);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    public override void Hide(bool immediately = false)
    {
        base.Hide();
        if (!immediately)
        {
            transform.DOScale(0f, AnimTime).SetEase(hideEaseType);
        }
        else
        {
            transform.localScale = Vector3.zero;
        }
    }
}
