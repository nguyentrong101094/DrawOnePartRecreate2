using GestureRecognizer;
using Gront;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureDebugger : MonoBehaviour
{
    [SerializeField] LineRenderer gestureByUserRenderer;
    [SerializeField] LineRenderer gestureLibraryRenderer;

    private void Start()
    {
        if (DebugManager.IsDebugMode())
        {
            GameDrawController.instance.gestureRecognizer.onUserDrawGesture += RenderUserGesture;
            GameDrawController.instance.gestureRecognizer.onGestureRecognized += RenderLibraryGesture;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void RenderUserGesture(object sender, Gesture gesture)
    {
        RenderGesture(gesture, gestureByUserRenderer);
    }

    void RenderLibraryGesture(object sender, Gesture gesture)
    {
        RenderGesture(gesture, gestureLibraryRenderer);
    }

    void RenderGesture(Gesture gesture, LineRenderer renderer)
    {
        if (gesture != null)
        {
            Vector3[] points = new Vector3[gesture.Points.Count];
            for (int i = 0; i < gesture.Points.Count; i++)
            {
                if (float.IsNaN(gesture.Points[i].x) || float.IsNaN(gesture.Points[i].y))
                {
                    Debug.LogError("NaN point");
                    return;
                }
                points[i] = gesture.Points[i] * 0.01f;
            }
            renderer.positionCount = points.Length;
            renderer.SetPositions(points);
        }
    }
}
