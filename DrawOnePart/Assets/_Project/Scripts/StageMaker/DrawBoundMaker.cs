using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBoundMaker : MonoBehaviour
{
    [SerializeField] RectTransform gestureLimitRectBounds;
    RectTransform gestureBoundParent;
    public bool isSettingBound;
    Vector2 fingerBeginPos;
    PictureData pictureData;
    bool isRotateBound; //if true, using mouse will result in rotating the bound

    private void Start()
    {
        gestureBoundParent = gestureLimitRectBounds.parent as RectTransform;
        LeanTouch.OnFingerDown += OnFingerDown;
        LeanTouch.OnFingerUpdate += OnFingerUpdate;
        LeanTouch.OnFingerUp += OnFingerUp;
    }

    void OnFingerDown(LeanFinger finger)
    {
        if (isSettingBound && !finger.IsOverGui)
        {
            if (isRotateBound)
            {
                RotateRectBoundToMouse(finger.ScreenPosition);
            }
            else
            {
                //RectTransformUtility.ScreenPointToLocalPointInRectangle(gestureBoundParent, finger.ScreenPosition, LevelMakerController.instance.Camera, out Vector2 localpoint);
                Vector2 localpoint = GetMousePosInCanvas(finger.ScreenPosition);
                fingerBeginPos = finger.ScreenPosition;
                gestureLimitRectBounds.anchoredPosition = new Vector2(localpoint.x, localpoint.y);
                gestureLimitRectBounds.sizeDelta = new Vector2(0f, 0f);
            }
        }
    }
    void OnFingerUpdate(LeanFinger finger)
    {
        if (isSettingBound && !finger.IsOverGui)
        {
            if (isRotateBound)
            {
                Vector2 localpoint = GetMousePosInCanvas(finger.ScreenPosition);
                Debug.DrawLine(gestureLimitRectBounds.anchoredPosition, localpoint, Color.green);
                Debug.DrawLine(Vector3.zero, localpoint, Color.red);
                Debug.DrawLine(Vector3.zero, gestureLimitRectBounds.anchoredPosition, Color.red);
                RotateRectBoundToMouse(localpoint);
            }
            else
            {
                Vector2 fingerMoveDelta = finger.ScreenPosition - fingerBeginPos;
                float canvasScale = LevelMakerController.instance.Canvas.scaleFactor;
                gestureLimitRectBounds.sizeDelta = new Vector2(fingerMoveDelta.x, -fingerMoveDelta.y) / canvasScale;
            }
        }
    }
    void OnFingerUp(LeanFinger finger)
    {
        if (isSettingBound && pictureData != null)
        {
            SetBoundData();
        }
    }

    public void Setup(PictureData data)
    {
        pictureData = data;
        SetupGestureBound(data, gestureLimitRectBounds);
    }

    public static void SetupGestureBound(PictureData data, RectTransform rectTransform)
    {
        rectTransform.anchoredPosition = new Vector2(data.bound_x, data.bound_y);
        rectTransform.sizeDelta = new Vector2(data.bound_width, data.bound_height);
        rectTransform.eulerAngles = new Vector3(0f, 0f, data.bound_angle_z);
    }

    void SetBoundData()
    {
        pictureData.bound_x = gestureLimitRectBounds.anchoredPosition.x;
        pictureData.bound_y = gestureLimitRectBounds.anchoredPosition.y;
        pictureData.bound_width = gestureLimitRectBounds.sizeDelta.x;
        pictureData.bound_height = gestureLimitRectBounds.sizeDelta.y;
        pictureData.bound_angle_z = gestureLimitRectBounds.eulerAngles.z;
    }

    void RotateRectBoundToMouse(Vector2 mousePos)
    {
        Vector2 fromRectToMouse = mousePos - gestureLimitRectBounds.anchoredPosition;
        float angle = Vector2.SignedAngle(Vector2.down, fromRectToMouse);
        gestureLimitRectBounds.eulerAngles = new Vector3(0f, 0f, angle);
    }

    Vector2 GetMousePosInCanvas(Vector2 fingerPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(gestureBoundParent, fingerPos, LevelMakerController.instance.Camera, out Vector2 localpoint);
        return localpoint;
    }

    public void SaveBound()
    {
        SetBoundData();
#if SQLITE4UNITY
        GameDatabase.Service.UpdateData(pictureData);
#else
        DatabaseSimpleSQL.Instance.UpdateData(pictureData);
#endif
        Debug.Log("Bound data saved");
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isRotateBound = true;
        }
        else
        {
            isRotateBound = false;
        }
    }

    private void OnDestroy()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
        LeanTouch.OnFingerUpdate -= OnFingerUpdate;
        LeanTouch.OnFingerUp -= OnFingerUp;
    }
}
