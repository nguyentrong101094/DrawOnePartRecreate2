using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial Data", menuName = "Tutorial Data")]
public class TutorialData : ScriptableObject
{
    [Header("If id is empty, the object's name will be used")]
    [SerializeField] private string id;
    public List<TutorialData> requireTutorials; //must see this tutorial first before this is displayed
    public TutorialBaseScript tutObject; //tut object to be created
    public int repeatTimes = 1; //how many time tutorial repeat
    [TextArea]
    public string textDialog;
    public bool saveOnDone; //this is the last tutorial of one progress, save after done

    public string Id
    {
        get
        {
            if (string.IsNullOrEmpty(id)) return name;
            else return id;
        }
        set => id = value;
    }
}
