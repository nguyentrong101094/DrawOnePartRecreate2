#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

[InitializeOnLoad]
class VersionIncrementor
{
    public int callbackOrder => 0;

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
    }

    private static void BuildPlayerHandler(BuildPlayerOptions options)
    {
        if (UnityEngine.Debug.isDebugBuild)
        {
            bool shouldIncrement = EditorUtility.DisplayDialog(
                "Increment Version?",
                $"Current: {PlayerSettings.bundleVersion}",
                "Yes",
                "No"
            );

            if (shouldIncrement) IncreaseBothVersions();
        }

        //ask to build addressable
        if (EditorUtility.DisplayDialog("Build with Addressables",
            "Do you want to build a clean addressables before export?",
            "Build with Addressables", "Skip"))
        {
            BuildAddressablesProcessor.PreExport();
        }
        BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
    }

    [MenuItem("Tools/Build/Increase Both Versions &v")]
    static void IncreaseBothVersions()
    {
        IncreaseBuild();
        //IncreasePlatformVersion();
    }

    [MenuItem("Tools/Build/Increase Current Build Version")]
    static void IncreaseBuild()
    {
        //IncrementVersion(new[] { 0, 0, 1 });
        IncrementVersionBeta(new[] { 0, 0, 0, 1 });
    }

    [MenuItem("Tools/Build/Increase Minor Version")]
    static void IncreaseMinor()
    {
        IncrementVersion(new[] { 0, 1, 0 });
    }

    [MenuItem("Tools/Build/Increase Major Version")]
    static void IncreaseMajor()
    {
        IncrementVersion(new[] { 1, 0, 0 });
    }

    [MenuItem("Tools/Build/Increase Platform Version")]
    static void IncreasePlatformVersion()
    {
        PlayerSettings.Android.bundleVersionCode += 1;
        PlayerSettings.iOS.buildNumber = (int.Parse(PlayerSettings.iOS.buildNumber) + 1).ToString();
    }

    static void IncrementVersion(int[] version)
    {
        string[] lines = PlayerSettings.bundleVersion.Split('.');

        for (int i = lines.Length - 1; i >= 0; i--)
        {
            bool isNumber = int.TryParse(lines[i], out int numberValue);

            if (isNumber && version.Length - 1 >= i)
            {
                /* uncomment if you want to increase minor version every 10 builds (please don't)
                 * if (i > 0 && version[i] + numberValue > 9)
                {
                    version[i - 1]++;

                    version[i] = 0;
                }
                else*/
                {
                    version[i] += numberValue;
                }
            }
        }

        PlayerSettings.bundleVersion = $"{version[0]}.{version[1]}.{version[2]}";
    }

    static void IncrementVersionBeta(int[] version)
    {
        string[] lines = PlayerSettings.bundleVersion.Split('b');

        if (lines.Length == 1)
        {
            PlayerSettings.bundleVersion = $"{PlayerSettings.bundleVersion}-b1";
        }
        else
        {
            bool isNumber = int.TryParse(lines[1], out int numberValue);
            if (isNumber)
            {
                version[3] += numberValue;
            }
            PlayerSettings.bundleVersion = $"{lines[0]}b{version[3]}";
        }
    }

    /*public void OnPreprocessBuild(BuildReport report)
    {
        if (UnityEngine.Debug.isDebugBuild)
        {
            bool shouldIncrement = EditorUtility.DisplayDialog(
                "Increment Version?",
                $"Current: {PlayerSettings.bundleVersion}",
                "Yes",
                "No"
            );

            if (shouldIncrement) IncreaseBothVersions();
        }

        //ask to build addressable

        if (EditorUtility.DisplayDialog("Build with Addressables",
            "Do you want to build a clean addressables before export?",
            "Build with Addressables", "Skip"))
        {
            BuildAddressablesProcessor.PreExport();
        }
    }*/
}

#endif