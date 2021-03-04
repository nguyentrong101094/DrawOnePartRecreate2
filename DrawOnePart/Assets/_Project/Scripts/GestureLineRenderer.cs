using GestureRecognizer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureLineRenderer : MonoBehaviour
{
    [SerializeField] LineRenderer gestureRenderer;

    /// <summary>
    /// Disable or enable gesture recognition
    /// </summary>
    public bool isEnabled = true;

    // The position of the point on the screen.
    Vector3 virtualKeyPosition = Vector2.zero;

    // A new point.
    Vector2 point;

    // Vertex count of the line renderer.
    int vertexCount = 0;

    /// <summary>
    /// List of points that form the gesture. 
    /// </summary>
    List<Vector2> points = new List<Vector2>();

    /// <summary>
    /// A new point will be placed if it is this further than the last point.
    /// </summary>
	public float distanceBetweenPoints = 10f;

    private void Awake()
    {
        gestureRenderer.positionCount = 0;
    }

    /// <summary>
    /// Register this point only if the point list is empty or current point
    /// is far enough than the last point. This ensures that the gesture looks
    /// good on the screen. Moreover, it is good to not overpopulate the screen
    /// with so much points.
    /// </summary>
    void RegisterPoint()
    {
        point = new Vector2(virtualKeyPosition.x, -virtualKeyPosition.y);

        if (points.Count == 0 || (points.Count > 0 && Vector2.Distance(point, points[points.Count - 1]) > distanceBetweenPoints)
            && points.Count < 9999) //limit recording point to prevent crash
        {
            points.Add(point);

            gestureRenderer.positionCount = ++vertexCount;
            gestureRenderer.SetPosition(vertexCount - 1, Utility.WorldCoordinateForGesturePoint(virtualKeyPosition));
        }
    }

    /// <summary>
    /// Remove the gesture from the screen.
    /// </summary>
    public void ClearGesture(object sender = null, System.EventArgs args = null)
    {
        points.Clear();
        gestureRenderer.positionCount = 0;
        vertexCount = 0;
    }


    void Update()
    {

        // Track user input if GestureRecognition is enabled.
        if (isEnabled)
        {

            // If it is a touch device, get the touch position
            // if it is not, get the mouse position
            if (Utility.IsTouchDevice())
            {
                if (Input.touchCount > 0)
                {
                    virtualKeyPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
                }
            }

            //if (RectTransformUtility.RectangleContainsScreenPoint(drawArea, virtualKeyPosition, Camera.main))
            //{

            if (Input.GetMouseButtonDown(0))
            {
                ClearGesture();
            }

            // It is not necessary to track the touch from this point on,
            // because it is already registered, and GetMouseButton event 
            // also fires on touch devices
            if (Input.GetMouseButton(0))
            {

                point = new Vector2(virtualKeyPosition.x, -virtualKeyPosition.y);

                // Register this point only if the point list is empty or current point
                // is far enough than the last point. This ensures that the gesture looks
                // good on the screen. Moreover, it is good to not overpopulate the screen
                // with so much points.
                if (points.Count == 0 ||
                    (points.Count > 0 && Vector2.Distance(point, points[points.Count - 1]) > distanceBetweenPoints))
                {
                    points.Add(point);

                    gestureRenderer.positionCount = ++vertexCount;
                    gestureRenderer.SetPosition(vertexCount - 1, Utility.WorldCoordinateForGesturePoint(virtualKeyPosition));
                }

            }
            //}
        }

    }
}
