using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenshotHelper : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static void CaptureScreenshot()
    {
        DateTime time = DateTime.Now;
        string path = $"ss_{time.ToFileTimeUtc()}.png";

/*#if UNITY_EDITOR
        System.IO.Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}/Screenshots");
#else
        System.IO.Directory.CreateDirectory($"{Application.persistentDataPath}/Space Shooter_Data/Screenshots");
#endif*/
        ScreenCapture.CaptureScreenshot(path);

#if UNITY_EDITOR
        Debug.Log($"Screenshot success {Directory.GetCurrentDirectory()}/{path}");
#else
        Debug.Log($"Screenshot success {Application.persistentDataPath}/{path}");
#endif
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CaptureScreenshot();
        }
    }
}
