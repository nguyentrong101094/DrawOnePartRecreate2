using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectItem : MonoBehaviour
{
    [SerializeField] Image frameImage;
    [SerializeField] Image completedMark;
    [SerializeField] Image lockMark;
    [SerializeField] Sprite completedFrameSprite;
    [SerializeField] Sprite unlockedFrameSprite;
    [SerializeField] TMP_Text textStageNumber;
    int stageID;
    bool isUnlocked;
    bool isCompleted;
    public System.EventHandler<int> onClick;

    public void Setup(int id)
    {
        stageID = id;
        isCompleted = stageID <= User.GetLastUnlockedStage() - 1;
        isUnlocked = stageID <= User.GetLastUnlockedStage();
        textStageNumber.text = $"Level {stageID}";
        if (isUnlocked)
        {
            lockMark.gameObject.SetActive(false);
            if (isCompleted)
            {
                frameImage.sprite = completedFrameSprite;
                //completedMark.gameObject.SetActive(true);
            }
            else
            {
                frameImage.sprite = unlockedFrameSprite;
            }
        }
        else
        {
            //textStageNumber.gameObject.SetActive(false);
        }
    }

    public void OnClick()
    {
        if (isUnlocked)
            onClick?.Invoke(this, stageID);
        else
        {
            SS.View.Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.OK, "You have to complete previous levels to unlock this."));
        }
        FirebaseManager.LogEvent("StageSelect_StageClick", "stage_number", stageID);
    }
}
