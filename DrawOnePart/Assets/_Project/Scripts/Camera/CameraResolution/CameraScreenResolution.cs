using UnityEngine;
using System.Collections;

public class CameraScreenResolution : MonoBehaviour
{
    [SerializeField] Vector2 referenceAspectRatio = new Vector2(9, 16);
    [SerializeField] float referenceOrthographicSize = 6.4f;
    public bool maintainWidth = true;
    [SerializeField] bool onlyScaleThinnerScreen;

    [Range(-1, 1)]
    public int adaptPosition;
    float defaultWidth;
    float defaultHeight;
    Vector3 CameraPos;

    public enum CamState { still, shaking };
    CamState camState = CamState.still;
    [SerializeField] float shakeMargin = 1f;
    [SerializeField] float shakeTime = 1f;
    float timerShake;
    Vector3 shakeDirection;

    [SerializeField] Camera m_Camera;
    [SerializeField] bool updatePerFrame;

    float ReferenceAspectRatio { get => referenceAspectRatio.x / referenceAspectRatio.y; }

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (m_Camera == null)
        {
            m_Camera = Camera.main;
        }
        CameraPos = m_Camera.transform.position;

        defaultHeight = referenceOrthographicSize;
        defaultWidth = referenceOrthographicSize * (ReferenceAspectRatio); // m_Camera.aspect;
        UpdatePosAndSize();
    }

    void Start()
    {
        //.Log(string.Format("width: {0}, height: {1}", defaultWidth, defaultHeight));
        //.Log("adapt: " + adaptPosition * (defaultWidth - m_Camera.orthographicSize * m_Camera.aspect));
    }

    public void ShakeCam(Vector3 initialDirection)
    {
        camState = CamState.shaking;
        shakeDirection = initialDirection;
    }

    public float GetScaleRatio()
    {
        return ReferenceAspectRatio / m_Camera.aspect;
    }

    public float GetAdaptDelta()
    {
        return adaptPosition * (defaultHeight - defaultWidth / m_Camera.aspect);
    }

    float GetOrthographicSize()
    {
        return defaultWidth / m_Camera.aspect;
    }

    public void UpdatePosAndSize()
    {
        if (maintainWidth)
        {
            if (onlyScaleThinnerScreen && m_Camera.aspect > ReferenceAspectRatio)
            {
                m_Camera.orthographicSize = referenceOrthographicSize;
            }
            else
                m_Camera.orthographicSize = GetOrthographicSize();
            //CameraPos.y was added in case camera in case camera's y is not in 0
            m_Camera.transform.position = new Vector3(CameraPos.x, CameraPos.y + adaptPosition * (defaultHeight - m_Camera.orthographicSize), CameraPos.z);
        }
        else
        {
            //CameraPos.x was added in case camera in case camera's x is not in 0
            m_Camera.transform.position = new Vector3(CameraPos.x + adaptPosition * (defaultWidth - m_Camera.orthographicSize * m_Camera.aspect), CameraPos.y, CameraPos.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCam();
    }

    public void UpdateCam()
    {
        switch (camState)
        {
            case CamState.shaking:
                timerShake += Time.deltaTime;
                float t_shakeValue = shakeMargin * (shakeTime - timerShake) * Mathf.Sin(Mathf.PI * (10f * timerShake + 0.5f));
                //.Log(t_shakeValue);
                Vector3 defaultCamPos = new Vector3(CameraPos.x, CameraPos.y + adaptPosition * (defaultHeight - m_Camera.orthographicSize), CameraPos.z); //base on still camera
                transform.position = defaultCamPos + shakeDirection * t_shakeValue; // * UnityEngine.Random.value;
                if (timerShake >= shakeTime)
                {
                    timerShake = 0f;
                    camState = CamState.still;
                }
                break;
            case CamState.still:
                if (!updatePerFrame) return;
                UpdatePosAndSize();
                break;
        }
    }
}