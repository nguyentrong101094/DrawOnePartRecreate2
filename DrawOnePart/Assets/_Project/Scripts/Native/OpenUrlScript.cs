using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenUrlScript : MonoBehaviour
{
    [Multiline]
    [Tooltip("If you use OpenUrlMyStore, this field won't be used")]
    [SerializeField] protected string url = "https://www.facebook.com/hatchthepokeggofficial";
    [Multiline]
    [SerializeField] protected string appUrl = "fb://hatchthepokeggofficial/";
    protected bool leftApp = false;
    protected bool clicked = false;

    protected virtual void Start()
    {

    }

    protected IEnumerator CoOpenUrl()
    {
        Application.OpenURL(appUrl);
        yield return new WaitForSecondsRealtime(1f);
        if (!leftApp)
        {
            Application.OpenURL(url);
        }
        leftApp = false;
        clicked = false;
    }

    public void OpenUrl()
    {
        clicked = true;
        if (string.IsNullOrEmpty(appUrl))
        {
            Application.OpenURL(url);
        }
        else
        {
            StartCoroutine(CoOpenUrl());
            /*float startTime;
            startTime = Time.timeSinceLevelLoad;


            //open the facebook app
            Application.OpenURL(appUrl);

            if (Time.timeSinceLevelLoad - startTime <= 1f)
            {
                //fail. Open safari.
                Application.OpenURL(url);
            }*/
        }
    }

    protected void OnApplicationFocus(bool pauseStatus)
    {
        if (clicked)
        {
            leftApp = pauseStatus;
#if UNITY_EDITOR
            leftApp = false;
#endif
        }
    }
}
