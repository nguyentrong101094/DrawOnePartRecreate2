using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;

public class LoadingController : Controller
{
    public const string LOADING_SCENE_NAME = "Loading";

    public override string SceneName()
    {
        return LOADING_SCENE_NAME;
    }
}