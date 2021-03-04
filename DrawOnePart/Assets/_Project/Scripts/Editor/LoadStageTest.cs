using GestureRecognizer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LoadStageTest : MonoBehaviour
{
    static bool stopTest = false;
    static bool testRunning = false;
    static int errorCount = 0;
    static MonoBehaviour tempMono;
    //[MenuItem("Tools/Test Load All Stage Pictures")]
    public static async void TestLoadStagePictureAsync()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Async only work in play mode");
            return;
        }
        testRunning = true;
        Debug.Log("Test started");
        var time = DateTime.Now;
        int errorCount = 0;
        var service = GameDatabase.GetDataServiceInEditMode();
        List<StageData> stageDatas = service.GetAllStageData().ToList();
        Debug.Log($"Testing {stageDatas.Count} stages");
        for (int i = 0; i < stageDatas.Count; i++)
        {
            if (stopTest)
            {
                stopTest = false;
                break;
            }
            Debug.Log($"Testing stage {stageDatas[i].id}");
            PictureData pictureData = service.GetPictureData(stageDatas[i].picture_name);
            if (pictureData == null)
            {
                Debug.LogError($"Picture data not found {stageDatas[i].id}");
                continue;
            }
            var picture = await AddressableImage.GetSpriteFromAtlasAsync(pictureData.name);
            if (picture == null)
            {
                Debug.LogError($"{pictureData.name} load failed");
                errorCount++;
            }
            var pictureSolution = await AddressableImage.GetSpriteFromAtlasAsync(StagePictureControl.GetPictureSolutionName(pictureData.name));
            if (pictureSolution == null)
            {
                Debug.LogError($"{StagePictureControl.GetPictureSolutionName(pictureData.name)} solution load failed");
                errorCount++;
            }

            //test load Gesture
            GestureLibrary gl = new GestureLibrary(pictureData.name, true, pictureData.use_v4);
        }
        TimeSpan timePassed = DateTime.Now - time;
        Debug.Log($"Test completed in {timePassed}, error count: {errorCount}");
        testRunning = false;
    }

    [MenuItem("Tools/Test Load All Stage Pictures")]
    public static void TestLoadStagePicture()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Async only work in play mode");
            return;
        }

        GameObject tempStageLoader = new GameObject("tempStageLoader");
        DontDestroyOnLoad(tempStageLoader);
        tempMono = tempStageLoader.AddComponent<DummyCoroutineRunner>();
        tempMono.StartCoroutine(CoTestLoadStagePicture());
    }

    static IEnumerator CoTestLoadStagePicture()
    {
        testRunning = true;
        Debug.Log("Test started");
        var time = DateTime.Now;
        errorCount = 0;
        var service = GameDatabase.GetDataServiceInEditMode();
        List<StageData> stageDatas = service.GetAllStageData().ToList();
        Debug.Log($"Testing {stageDatas.Count} stages");
        for (int i = 0; i < stageDatas.Count; i++)
        {
            if (stopTest)
            {
                stopTest = false;
                break;
            }
            Debug.Log($"Testing stage {stageDatas[i].id}");
            PictureData pictureData = service.GetPictureData(stageDatas[i].picture_name);
            if (pictureData == null)
            {
                Debug.LogError($"Picture data not found {stageDatas[i].id}");
                continue;
            }

            yield return tempMono.StartCoroutine(CoTestLoadPicture(pictureData.name));
            yield return tempMono.StartCoroutine(CoTestLoadPicture(StagePictureControl.GetPictureSolutionName(pictureData.name)));

            //test load Gesture
            GestureLibrary gl = new GestureLibrary(pictureData.name, true, pictureData.use_v4);
            if (errorCount > 0)
            {
                Debug.Log("Test aborted");
                break;
            }
        }
        TimeSpan timePassed = DateTime.Now - time;
        Debug.Log($"Test completed in {timePassed}, error count: {errorCount}");
        testRunning = false;
    }

    static IEnumerator CoTestLoadPicture(string pictureName)
    {
        Sprite picture = null;
        bool coroutineIsComplete = false;
        StageResLoader.Instance.GetSpriteFromAtlasAsync(pictureName, (Sprite pic) =>
        {
            coroutineIsComplete = true;
            picture = pic;
        });
        while (!coroutineIsComplete)
        {
            yield return 0;
        }
        if (picture == null)
        {
            Debug.LogError($"{pictureName} load failed");
            errorCount++;
        }
    }

    [MenuItem("Tools/Stop Test load Picture")]
    public static void StopTesting()
    {
        if (testRunning)
        {
            stopTest = true;
            Debug.Log("Stop testing");
        }
    }

    /*[MenuItem("Tools/Rename HW in stage data")]
    public static void RenamePictureData()
    {
    }*/

    /*[MenuItem("Tools/Generate Thumbnails")]
    public static async void GenerateStageThumbnails()
    {
        testRunning = true;
        Debug.Log("Test started");
        var time = DateTime.Now;
        int errorCount = 0;
        var service = GameDatabase.GetDataServiceInEditMode();
        List<StageData> stageDatas = service.GetAllStageData().ToList();
        Debug.Log($"Testing {stageDatas.Count} stages");
        for (int i = 0; i < stageDatas.Count; i++)
        {
            if (stopTest)
            {
                stopTest = false;
                break;
            }
            Debug.Log($"Testing stage {stageDatas[i].id}");
            PictureData pictureData = service.GetPictureData(stageDatas[i].picture_name);
            if (pictureData == null)
            {
                Debug.LogError($"Picture data not found {stageDatas[i].id}");
                continue;
            }
            var picture = await AddressableImage.GetSpriteFromAtlasAsync(pictureData.name);
            if (picture == null)
            {
                Debug.LogError($"{pictureData.name} load failed");
                errorCount++;
            }
            var pictureSolution = await AddressableImage.GetSpriteFromAtlasAsync(StagePictureControl.GetPictureSolutionName(pictureData.name));
            if (pictureSolution == null)
            {
                Debug.LogError($"{StagePictureControl.GetPictureSolutionName(pictureData.name)} solution load failed");
                errorCount++;
            }
        }
        TimeSpan timePassed = DateTime.Now - time;
        Debug.Log($"Test completed in {timePassed}, error count: {errorCount}");
        testRunning = false;

        MemoryStream outputStream = new MemoryStream();
        System.Drawing.Image image = System.Drawing.Image.FromFile(originalImagePath);
        System.Drawing.Image thumbnail = image.GetThumbnailImage(thumbnailWidth, thumbnailHeight, () => false, IntPtr.Zero);
        thumbnail.Save(outputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
        return outputStream;
    }*/
}
