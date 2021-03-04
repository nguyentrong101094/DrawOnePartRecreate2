using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for perspective camera
public class StageCamera : MonoBehaviour
{
    [SerializeField] Vector2 referenceResolution = new Vector2(720f, 1280f);
    public void SetPixelPerfect()
    {
        float pixelMult = 1f; // scaling factor, assumes 100ppu unity default, and scales up to my desired 3 pixel squares.

        var camera = GetComponent<Camera>();
        var camFrustWidthShouldBe = referenceResolution.y / 100f;
        //var frustrumInnerAngles = (180f - camera.fieldOfView) / 2f * Mathf.PI / 180f;
        var frustrumInnerAngles = (camera.fieldOfView) / 2f * Mathf.PI / 180f;
        var newCamDist = (camFrustWidthShouldBe / 2) / Mathf.Tan(frustrumInnerAngles);
        Debug.Log(Screen.currentResolution.height);
        Debug.Log(Screen.height);
        Debug.Log(camera.fieldOfView);
        Debug.Log(newCamDist);
        transform.position = new Vector3(0, 0, -newCamDist / pixelMult);
    }
}
