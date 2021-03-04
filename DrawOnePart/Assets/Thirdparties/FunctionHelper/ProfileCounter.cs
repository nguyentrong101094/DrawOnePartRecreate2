using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ProfileCounter : MonoBehaviour
{
    static PerformanceCounter cpuCounter;
    static PerformanceCounter ramCounter;

    private void Awake()
    {
        cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        ramCounter = new PerformanceCounter("Memory", "Available MBytes");
    }

    public static void LogMemoryUsage()
    {
        UnityEngine.Debug.Log(string.Format("> cpu: {0}, ram: {1}", getCurrentCpuUsage(), getAvailableRAM()));
    }

    public static string getCurrentCpuUsage()
    {
        cpuCounter.NextValue();
        System.Threading.Thread.Sleep(1000); // wait a second to get a valid reading
        var usage = cpuCounter.NextValue();

        return usage + "%";
    }

    public static string getAvailableRAM()
    {
        return ramCounter.NextValue() + "MB";
    }

    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 60;
        style.normal.textColor = new Color(0.0f, 1.0f, 0f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}
