using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;

public class ReceiveItemController : PopupController
{
    public const string RECEIVEITEM_SCENE_NAME = "ReceiveItem";

    public override string SceneName()
    {
        return RECEIVEITEM_SCENE_NAME;
    }
}