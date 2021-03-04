using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;

public class PrivacyPolicyController : Controller
{
    public const string PRIVACYPOLICY_SCENE_NAME = "PrivacyPolicy";

    public override string SceneName()
    {
        return PRIVACYPOLICY_SCENE_NAME;
    }
}