using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;
using UnityEngine.UI;

public class RatePopupController : Controller
{
    public const string RATEPOPUP_SCENE_NAME = "RatePopup";

    public override string SceneName()
    {
        return RATEPOPUP_SCENE_NAME;
    }

    [SerializeField] Slider rateSlider;
    [SerializeField] Button rateButton;
    PopupData onRateFinish;
    //bool showFeedback = false;

    public override void OnActive(object data)
    {
        base.OnActive(data);
        onRateFinish = data as PopupData;
    }

    public void OnSliderChange(float value)
    {
        //rateButton.interactable = true;
    }

    public void OnStarButton(int id)
    {
        rateSlider.value = id;
    }

    public void OnRateButton()
    {
        if (rateSlider.value == 0)
        {
            Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.OK, "Please tap on a star to rate this game."));
        }
        else if (rateSlider.value < 5)
        {
            //showFeedback = true;
            OnGivingReview();
        }
        else
        {
            GetComponent<OpenUrlMyStore>().OpenUrl();
            OnGivingReview();
        }
    }

    void OnGivingReview()
    {
        FirebaseManager.LogEvent($"Rate_Rated_{rateSlider.value}");
        PlayerPrefs.SetInt(Const.PREF_HAS_REVIEWED, 1);
        Manager.Close();
    }

    public override void OnHidden()
    {
        base.OnHidden();
        if (PlayerPrefs.GetInt(Const.PREF_HAS_REVIEWED, 0) == 1)
        {
            Manager.Add(PopupController.POPUP_SCENE_NAME, onRateFinish);
        }
        else
        {
            onRateFinish.onOk?.Invoke();
        }
    }
}