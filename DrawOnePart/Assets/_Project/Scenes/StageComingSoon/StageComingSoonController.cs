using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;

public class StageComingSoonController : Controller
{
    public const string STAGECOMINGSOON_SCENE_NAME = "StageComingSoon";

    public override string SceneName()
    {
        return STAGECOMINGSOON_SCENE_NAME;
    }

    public void ReplayGame()
    {
        Manager.Load(StageSelectController.STAGESELECT_SCENE_NAME);
        /*Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.YES_NO, "Do you want to start over again from level 1?",
            () => {
                FirebaseManager.LogEvent("StageComing_ReplayConfirm");
                Manager.Load(GameDrawController.GAMEDRAW_SCENE_NAME); }));*/
    }

    public override void OnKeyBack()
    {
    }
}