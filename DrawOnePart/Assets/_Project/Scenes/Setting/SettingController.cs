using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;

public class SettingController : Controller
{
    public const string SETTING_SCENE_NAME = "Setting";

    public override string SceneName()
    {
        return SETTING_SCENE_NAME;
    }

    public void OnPolicyBtn()
    {
        Manager.Add(PrivacyPolicyController.PRIVACYPOLICY_SCENE_NAME);
    }

    public void OnFeedbackBtn()
    {
        SendMail.Send("Draw Now! Feedback", "");
    }

    public void OnAboutUsBtn()
    {
        Manager.Add(AboutUsController.ABOUTUS_SCENE_NAME);
    }

    public void OnHomeBtn()
    {
        Manager.Load(StageSelectController.STAGESELECT_SCENE_NAME);
    }

    public override void OnKeyBack()
    {
        base.OnKeyBack();
        PlayerPrefs.Save();
    }
}