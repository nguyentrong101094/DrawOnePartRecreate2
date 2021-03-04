using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenUrlMyStore : OpenUrlScript
{
    protected override void Start()
    {
        base.Start();
        appUrl = Const.PLAY_STORE_APP_URL;
        url = Const.PLAY_STORE_URL;
    }
}
