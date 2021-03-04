using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageResult
{
    public int stageID;
    public float timePlayed;

    public StageResult(int stageID, float timePlayed)
    {
        this.stageID = stageID;
        this.timePlayed = timePlayed;
    }
}
