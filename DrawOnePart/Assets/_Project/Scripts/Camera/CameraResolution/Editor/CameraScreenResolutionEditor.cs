using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraScreenResolution))]
public class CameraScreenResolutionEditor : Editor
{
    CameraScreenResolution m_Target;

    private void Awake()
    {
        m_Target = target as CameraScreenResolution;
        m_Target.Init();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Update Ratio"))
        {
            m_Target.UpdatePosAndSize();
        }

        if (GUILayout.Button("Harlem Shake!"))
        {
            m_Target.ShakeCam(Random.insideUnitCircle.normalized);
        }
    }
}
