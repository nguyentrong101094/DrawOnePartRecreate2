using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageData
{
    public int id { get; set; }
    public string picture_name { get; set; }
    public bool IsTracked() { return id < 100 || id % 5 == 0; } //only track stage satisfy this condition to reduce number of events

    public bool IsRateStage()
    {
        return (id % 5 == 0 && PlayerPrefs.GetInt(Const.PREF_HAS_REVIEWED, 0) == 0);
    }
}
