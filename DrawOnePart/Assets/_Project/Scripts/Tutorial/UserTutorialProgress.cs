using UnityEngine;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System;

public class TutorialProgress
{
    public Dictionary<string, int> finishedTutorials = new Dictionary<string, int>();

    public void Init()
    {
    }

    /*public bool HasSeenTutorial(string id)
    {
        bool hasSeen = finishedTutorials.Contains(id);
        return hasSeen;
    }

    public void DoneTutorial(TutorialData data)
    {
        if (!HasSeenTutorial(data.id))
        {
            finishedTutorials.Add(data.id);
            if (data.saveOnDone) TutorialManager.Save();
        }
    }*/
}
