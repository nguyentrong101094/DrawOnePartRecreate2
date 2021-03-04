using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;

public class HintDialogController : Controller
{
    public const string HINTDIALOG_SCENE_NAME = "HintDialog";

    public override string SceneName()
    {
        return HINTDIALOG_SCENE_NAME;
    }

    [SerializeField] StagePictureControl stagePicture;

    public override void OnActive(object data)
    {
        base.OnActive(data);
        string pictureName = data as string;
        stagePicture.Setup(pictureName, false);
        stagePicture.OnCorrect();
    }
}