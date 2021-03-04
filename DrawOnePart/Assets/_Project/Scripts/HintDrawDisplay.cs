using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestureRecognizer;

public class HintDrawDisplay : MonoBehaviour
{
    [SerializeField] GameObject drawDot;

    public void ShowHint()
    {
        List<Gesture> gestures = GameDrawController.instance.gestureRecognizer.LoadedGestureLibrary.Library;
        for (int i = 0; i < gestures[0].Points.Count; i++)
        {
            Debug.Log(gestures[0].Points[i]);
            GameObject dot = Instantiate(drawDot, GameDrawController.instance.levelPicture.pictureUICanvas.transform);
            RectTransform rect = dot.transform as RectTransform;
            rect.anchoredPosition = gestures[0].Points[i];
        }
    }
}
