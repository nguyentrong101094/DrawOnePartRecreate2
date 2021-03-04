using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;
using UnityEngine.UI;

public class PopupWithImageData : PopupData
{
    public string imagePath;
    public PopupWithImageData(PopupType type, string text, Manager.Callback onOk = null, Manager.Callback onCancel = null) : base(type, text, onOk, onCancel)
    {
    }

    public PopupWithImageData SetImagePath(string path) { imagePath = path; return this; }
}

public class PopupWithImageController : PopupController
{
    public new const string POPUP_SCENE_NAME = "PopupWithImage";

    public override string SceneName()
    {
        return POPUP_SCENE_NAME;
    }

    [SerializeField] Image icon;

    public override void OnActive(object data)
    {
        base.OnActive(data);
        PopupWithImageData popupImageData = data as PopupWithImageData;
        icon.sprite = Resources.Load<Sprite>(popupImageData.imagePath);
        icon.SetNativeSize();
    }
}