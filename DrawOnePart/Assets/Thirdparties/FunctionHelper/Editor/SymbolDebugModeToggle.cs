using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SymbolDebugModeToggle
{
    const string debugModeSymbol = "DEBUG_MODE";

    [MenuItem("Tools/Debug Mode/Add Symbol")]
    public static void AddDebugModeSymbol()
    {
        string currentSymbol = GetCurrentSymbol();

        if (currentSymbol.Contains(debugModeSymbol))
        {
            Debug.Log("Already has debug mode symbol.");
            return;
        }
        currentSymbol = currentSymbol + ";DEBUG_MODE";
        SetSymbol(currentSymbol);
    }

    [MenuItem("Tools/Debug Mode/Remove Symbol")]
    public static void RemoveDebugModeSymbol()
    {
        string currentSymbol = GetCurrentSymbol();

        if (!currentSymbol.Contains(debugModeSymbol))
        {
            Debug.Log("Already removed debug mode symbol.");
            return;
        }

        currentSymbol = currentSymbol.Replace("DEBUG_MODE", "");
        SetSymbol(currentSymbol);
    }

    static string GetCurrentSymbol()
    {
        string currentSymbol;
#if UNITY_ANDROID
        currentSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
#elif UNITY_STANDALONE
        currentSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
#endif
        return currentSymbol;
    }

    static void SetSymbol(string currentSymbol)
    {
#if UNITY_ANDROID
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, currentSymbol);
#elif UNITY_STANDALONE
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, currentSymbol);
#endif
    }
}
