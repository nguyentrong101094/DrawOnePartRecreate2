using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRelativeToScreen : MonoBehaviour
{
    [Tooltip("Relative ratio to the bottom left corner of screen")]
    [SerializeField] Vector3 relativePosRatio;

    void Start()
    {
        Vector3 screenPoint = new Vector3(relativePosRatio.x * Screen.width, relativePosRatio.y * Screen.height, relativePosRatio.z);
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
        transform.position = new Vector3(worldPoint.x, worldPoint.y, transform.position.z);
    }
}
