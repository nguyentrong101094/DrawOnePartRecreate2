using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    static bool isShowing; //is a tutorial showing?
    public static TutorialProgress data;
    public static Dictionary<string, int> cacheHasSeenTutorial = new Dictionary<string, int>();

    static TutorialManager()
    {
        Load();
    }

    private static void Load()
    {
        var textData = PlayerPrefs.GetString("TUTORIAL_PROGRESS", string.Empty);

        if (!string.IsNullOrEmpty(textData))
        {
            data = JsonMapper.ToObject<TutorialProgress>(textData);
        }
        else
        {
            ResetData();
        }
    }

    public static void ResetData()
    {
        data = new TutorialProgress();
        data.Init();
        cacheHasSeenTutorial.Clear();
        activeTutorials.Clear();
        Save();
    }

    public static void Save()
    {
        var textData = JsonMapper.ToJson(data);

        PlayerPrefs.SetString("TUTORIAL_PROGRESS", textData);
        PlayerPrefs.Save();
    }

    public static bool HasSeenTutorial(TutorialData tutData)
    {
        return HasSeenTutorial(tutData.Id, tutData.repeatTimes);
    }

    public static bool HasSeenTutorial(string id, int seeTime = 1)
    {
        if (!cacheHasSeenTutorial.ContainsKey(id))
        {
            int value = 0;
            if (data.finishedTutorials.ContainsKey(id)) value = data.finishedTutorials[id];
            cacheHasSeenTutorial.Add(id, value);
        }

        return cacheHasSeenTutorial[id] >= seeTime;
    }

    public static bool CanShowTutorial(TutorialData tutData)
    {
        if (isShowing)
        {
            Debug.Log("A tutorial is already in place");
            return false;
        }
        bool seenAllRequireTut = true;
        for (int i = 0; i < tutData.requireTutorials.Count; i++)
        {
            if (!HasSeenTutorial(tutData.requireTutorials[i]))
            {
                seenAllRequireTut = false;
                break;
            }
        }
        if (!seenAllRequireTut)
        {
            //("has not seen all required tut");
            return false;
        }

        if (!HasSeenTutorial(tutData))
        {
            return true;
        }
        else return false;
    }

    public static void CompleteTutorial(TutorialData tutData, int seeTime = 1)
    {
        if (!HasSeenTutorial(tutData))
        {
            if (!data.finishedTutorials.ContainsKey(tutData.Id))
                data.finishedTutorials.Add(tutData.Id, 0);
            data.finishedTutorials[tutData.Id] += 1;
            if (!cacheHasSeenTutorial.ContainsKey(tutData.Id))
            {
                cacheHasSeenTutorial.Add(tutData.Id, 0);
            }
            cacheHasSeenTutorial[tutData.Id] = data.finishedTutorials[tutData.Id];
            TutorialManager.activeTutorials.Remove(tutData.Id);

            if (tutData.saveOnDone) TutorialManager.Save();
            isShowing = false;
            FirebaseManager.LogEvent($"Tutorial_Done_{tutData.Id}");
        }
    }

    public static void OnTutorialEndUnexpected()
    {
        isShowing = false;
    }

    public static Dictionary<string, TutorialBaseScript> activeTutorials = new Dictionary<string, TutorialBaseScript>(); //store list of active tutorial to iterate through

    public static TutorialBaseScript ShowTutorial(TutorialItem caller)
    {
        TutorialBaseScript m_Tut = ShowTutorial(caller.m_Data, caller.transform);
        return m_Tut;
    }

    public static TutorialBaseScript ShowTutorial(TutorialData m_Data, Transform parent)
    {
        TutorialBaseScript m_Tut;
        isShowing = true;
        if (activeTutorials.ContainsKey(m_Data.Id) && activeTutorials[m_Data.Id] != null)
        {
            return activeTutorials[m_Data.Id];
            //.LogError("A tutorial like this already exist, this should not happen");
        }
        else
        {
            m_Tut = Instantiate(m_Data.tutObject, parent);
            if (activeTutorials.ContainsKey(m_Data.Id))
                activeTutorials[m_Data.Id] = m_Tut;
            else
                activeTutorials.Add(m_Data.Id, m_Tut);
        }
        return m_Tut;
    }

    public static bool CheckShowing(TutorialData tutData)
    {
        if (activeTutorials.ContainsKey(tutData.Id) && activeTutorials[tutData.Id] != null)
            return true;
        else
            return false;
    }

    public enum ActionType { DragGamePiece = 1, BoosterBomb = 2, BoosterRotate = 3, BoosterThunder = 5 }
    public delegate bool CheckActionPossibleDelegate(ActionType actionType);
}
