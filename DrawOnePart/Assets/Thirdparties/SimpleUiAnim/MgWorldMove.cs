using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgWorldMove : MgAnimBase
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
        transform.position = startPos;
        if (!immediately)
        {
            transform.DOLocalMove(endPos, AnimTime).SetEase(easeType).SetUpdate(true);
        }
        else
        {
            transform.DOLocalMove(endPos, 0f).SetUpdate(true);
        }
    }

    public override void Hide(bool immediately = false)
    {
        base.Hide();
        if (!immediately)
        {
            transform.DOLocalMove(startPos, AnimTime).SetEase(easeType).SetUpdate(true);
        }
        else
        {
            transform.DOLocalMove(startPos, 0f).SetUpdate(true);
        }
    }
}
