using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public float rotateSpeed = -10f;
    public Vector3 axisRotate = new Vector3(0f, 0f, 1f);
    public Space relativeSpace;

    void Update()
    {
        transform.Rotate(axisRotate, rotateSpeed * Time.deltaTime, relativeSpace);
    }
}
