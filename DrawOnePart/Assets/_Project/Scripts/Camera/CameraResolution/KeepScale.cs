using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepScale : MonoBehaviour
{
    [SerializeField] CameraScreenResolution cameraScreenResolution;
    private void Start()
    {
        //Debug.Log(cameraScreenResolution.GetScaleRatio());
        //Debug.Log(cameraScreenResolution.GetAdaptDelta());
        transform.localScale = transform.localScale * cameraScreenResolution.GetScaleRatio();
        transform.position = new Vector3(transform.position.x, (transform.position.y + cameraScreenResolution.GetAdaptDelta()));
    }
}
