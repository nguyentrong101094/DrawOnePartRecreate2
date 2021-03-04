using Lean.Touch;
using PaintIn3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//since PaintHitScreen handled drawing, this class will handle firing events
public class PlayerFingerDraw : MonoBehaviour
{
    public PaintHitScreen paintHitScreen;
    public GestureLineRenderer paintDrawRenderer;
    [SerializeField] Transform drawDetectCollider;
    //[SerializeField] UnityEvent onFingerUpEvent;
    public event System.EventHandler onClearDraw;
    public event System.EventHandler<Vector3> onBeginDraw;
    public event System.EventHandler<Vector3> onUpdateDraw;

    public float checkRadius;
    Collider[] collidersInRadius;

    [SerializeField] AudioSource drawAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        LeanTouch.OnFingerDown += OnFingerDown;
        LeanTouch.OnFingerUpdate += OnFingerUpdate;
        LeanTouch.OnFingerUp += OnFingerUp;
        paintHitScreen.onFingerHitUpdate += DetectDraw;
        //onClearDraw += paintHitScreen.ClearAll;
        onClearDraw += paintDrawRenderer.ClearGesture;
    }

    Vector3 GetTouchWorldPosition(LeanFinger finger)
    {
        return finger.GetWorldPosition(Mathf.Abs(GameDrawController.instance.Camera.transform.position.z), GameDrawController.instance.Camera);
    }

    void OnFingerDown(LeanFinger finger)
    {
        if (!finger.StartedOverGui)
        {
            if (drawAudioSource.time == 0f)
                drawAudioSource.Play();
            else drawAudioSource.UnPause();
            onBeginDraw?.Invoke(this, GetTouchWorldPosition(finger));
        }
    }

    void OnFingerUpdate(LeanFinger finger)
    {
        if (!finger.StartedOverGui)
        {
            if (finger.ScreenDelta.sqrMagnitude > 0.1f && !drawAudioSource.isPlaying)
            {
                drawAudioSource.Play();
            }
            onUpdateDraw?.Invoke(this, GetTouchWorldPosition(finger));
        }
    }

    void OnFingerUp(LeanFinger finger)
    {
        StartCoroutine(CoClearAll());
        drawAudioSource.Pause();
    }

    void DetectDraw(object sender, Vector3 pos)
    {
        drawDetectCollider.position = pos;
        /*Physics.OverlapSphereNonAlloc(pos, checkRadius, collidersInRadius);
        for (int i = 0; i < collidersInRadius.Length; i++)
        {
            collidersInRadius[i].GetComponent<DrawDot>();
        }*/
    }

    IEnumerator CoClearAll()
    {
        yield return 0;
        onClearDraw?.Invoke(this, null);
    }


    private void OnDestroy()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
        LeanTouch.OnFingerUpdate -= OnFingerUpdate;
        LeanTouch.OnFingerUp -= OnFingerUp;
    }
}
