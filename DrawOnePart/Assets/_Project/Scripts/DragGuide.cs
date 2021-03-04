using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragGuide : MonoBehaviour
{
    [SerializeField] GameObject handGuide;
    [SerializeField] float pathSpeed = 3f;
    bool showingHandGuide;
    Coroutine currentCoDragGuide;
    float handGuideMoveTime = 1.5f;
    const int DRAG_GUIDE_DOTWEEN_ID = 69;

    Vector3[] currentPath;

    //[SerializeField] LineConnectDrag lineHint;

    public void ShowDragGuide(Vector3 beginPos, Vector3 endPos, bool circleEnd)
    {
        //show a finger dragging from piece to tile
        EnableHand();
        StopCoDragGuide();
        currentCoDragGuide = StartCoroutine(CoDragGuide(beginPos, endPos, circleEnd));
        //handGuideAnimancer.Stop();
    }

    void EnableHand()
    {
        showingHandGuide = true;
        handGuide.SetActive(true);
    }

    IEnumerator CoDragGuide(Vector3 beginPos, Vector3 endPos, bool circleEnd)
    {
        //handGuideCircleTarget.SetActive(circleEnd);
        //handGuideCircleTarget.transform.position = endPos;
        handGuide.transform.position = beginPos;
        while (showingHandGuide)
        {
            yield return new WaitForSeconds(.5f);
            handGuide.transform.DOMove(endPos, handGuideMoveTime).SetId(DRAG_GUIDE_DOTWEEN_ID);
            yield return new WaitForSeconds(handGuideMoveTime + .5f);
            handGuide.transform.position = beginPos;
        }
    }

    public void HideDragGuide()
    {
        StopCoDragGuide();
        handGuide.SetActive(false);
        //handGuideCircleTarget.SetActive(false);
    }

    void StopCoDragGuide()
    {
        if (currentCoDragGuide != null)
        {
            DOTween.Kill(DRAG_GUIDE_DOTWEEN_ID);
            StopCoroutine(currentCoDragGuide);
        }
    }

    public void FollowPath(Vector3[] points)
    {
        EnableHand();
        StopCoDragGuide();
        currentPath = points;
        transform.position = currentPath[0];
        //StartCoroutine(CoLoopPath());
        transform.DOPath(currentPath, pathSpeed).SetSpeedBased(true).SetEase(Ease.Linear).SetId(DRAG_GUIDE_DOTWEEN_ID).OnComplete(LoopPath);
    }

    void LoopPath()
    {
        if (gameObject.activeInHierarchy)
            currentCoDragGuide = StartCoroutine(CoLoopPath());
        //transform.DOPath(currentPath, 2f).SetEase(Ease.Linear).SetSpeedBased(true).SetId(DRAG_GUIDE_DOTWEEN_ID).SetDelay(1f).SetLoops(-1);
    }

    IEnumerator CoLoopPath()
    {
        yield return new WaitForSeconds(.5f);
        transform.position = currentPath[0];
        transform.DOPath(currentPath, pathSpeed).SetSpeedBased(true).SetEase(Ease.Linear).SetId(DRAG_GUIDE_DOTWEEN_ID).OnComplete(LoopPath);
    }

    //public void ShowHintLine(Vector3 beginPos, Vector3 endPos)
    //{
    //    lineHint.transform.position = beginPos;
    //    lineHint.HintToWorldPos(endPos);
    //}
}
