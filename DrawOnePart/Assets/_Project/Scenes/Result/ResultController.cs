using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;
using TMPro;

public class ResultController : Controller
{
    public class SceneData
    {
        public StageResult stageResult;
        public StageData stageData;
        public bool interAdsWasShown; //check if interstitial ads was showing before going to result scene
        public bool needUnloadAtlasOnWin;

        public SceneData(StageResult stageResult, StageData stageData, bool interAdsWasShown, bool needUnloadAtlasOnWin)
        {
            this.stageResult = stageResult;
            this.stageData = stageData;
            this.interAdsWasShown = interAdsWasShown;
            this.needUnloadAtlasOnWin = needUnloadAtlasOnWin;
        }
    }
    public const string RESULT_SCENE_NAME = "Result";

    public override string SceneName()
    {
        return RESULT_SCENE_NAME;
    }

    [SerializeField] StagePictureControl stagePicture;
    [SerializeField] GameObject removeAdsBtn;
    [SerializeField] GameObject moreHintBtn;
    [SerializeField] MgObject[] showAfterDelay; //show next stage button after delaying some seconds
    [SerializeField] TMP_Text iqText;
    [SerializeField] AudioClip winSound;
    SceneData sceneData;
    StageData stageData { get => sceneData.stageData; }
    float bgmVolumeToRestore;

    bool needUnloadAtlasOnWin = false;

    IEnumerator Start()
    {
        bgmVolumeToRestore = AudioManager.BgmVolume;
        AudioManager.BgmVolume = 0f;
        AudioManager.Instance.PlaySfx(winSound.name);
        yield return new WaitForSecondsRealtime(winSound.length);
        while (AudioManager.BgmVolume < bgmVolumeToRestore)
        {
            AudioManager.BgmVolume += 0.05f;
            yield return null;
        }
        RestoreBgm();
    }

    public override void OnActive(object data)
    {
        base.OnActive(data);
        if (data != null) sceneData = data as SceneData;
        else
        {
            //sceneData = new SceneData(new StageResult(1, 5f), GameDatabase.Service.GetStageData(1), true);
        }

        stagePicture.Setup(sceneData.stageData.picture_name, false);
        stagePicture.OnCorrect();

        bool showRemoveAds = false;
        if (!IAPProcessor.CheckNoAds() && sceneData.interAdsWasShown)
        {
            showRemoveAds = true;
        }
        removeAdsBtn.SetActive(showRemoveAds);
        moreHintBtn.SetActive(!showRemoveAds);
        StartCoroutine(CoIncreaseIQNumber());
    }

    public override void OnShown()
    {
        base.OnShown();
        StartCoroutine(CoShowNextStageBtn());
    }

    IEnumerator CoIncreaseIQNumber()
    {
        int iqCounter = 0;
        int iqValue;
        float time = sceneData.stageResult.timePlayed;
        iqValue = (int)(75f + Mathf.Pow(1.5f, 10 - time / 5) + UnityEngine.Random.value * 5f);
        while (iqCounter < iqValue)
        {
            iqText.text = $"IQ: {iqCounter}";
            yield return null;
            iqCounter += iqValue / 70;
        }
        iqText.text = $"IQ: {iqValue}";
    }

    IEnumerator CoShowNextStageBtn()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        foreach (var item in showAfterDelay)
        {
            item.Show();
        }
    }

    public void Retry()
    {
        sceneData.needUnloadAtlasOnWin = false;
        Manager.Load(GameDrawController.GAMEDRAW_SCENE_NAME, new GameSceneData(stageData.id));
    }

    public void NextStage()
    {
        if (stageData.IsRateStage())
        {
            Manager.Add(RatePopupController.RATEPOPUP_SCENE_NAME,
                new PopupData(PopupType.OK, "Thank you for your feedback!",
            ConfirmGoToNextStage));
            FirebaseManager.LogEvent($"Rate_ShowStage_{stageData.id}");
        }
        /*else if (InternetCheck.CheckInternetOptional())
        {
            Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.OK, Const.MSG_NO_INTERNET));
        }*/
        else ConfirmGoToNextStage();
    }

    void ConfirmGoToNextStage()
    {
        if (stageData.id < Const.MAX_STAGE)
        {
            Manager.Load(GameDrawController.GAMEDRAW_SCENE_NAME, new GameSceneData(stageData.id + 1));
        }
        else
        {
            Manager.Add(StageComingSoonController.STAGECOMINGSOON_SCENE_NAME);
            //Manager.Add(StageComingSoonController.STAGECOMINGSOON_SCENE_NAME);
        }
    }

    public void RewardGetMoreHint()
    {
        FirebaseManager.LogEvent("Result_RewardGetHint_BtnClick");
        AdsManager.Reward((bool success) =>
        {
            if (success)
            {
                User.AddHint(1);
                moreHintBtn.SetActive(false);
                Manager.Add(ReceiveItemController.RECEIVEITEM_SCENE_NAME, new PopupData(PopupType.OK, "+1", "You received a hint!"));
            }
        }, AdPlacementType.Reward_GetMoreHint);
    }

    public void OnRemoveAdsButton()
    {
        FirebaseManager.LogEvent("Result_RemoveAds_BtnClick");
        Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.YES_NO,
            "Buying [No Ads] will remove the ads at the bottom and the ads shown between levels. However, you still can watch ads to skip levels.",
            () =>
            {
                InAppPurchaseHelper.instance.BuyProduct(IAPProcessor.remove_ads, OnPurchaseComplete);
                FirebaseManager.LogEvent("Result_RemoveAds_DialogConfirm");
            }, () =>
            {
                FirebaseManager.LogEvent("Result_RemoveAds_Cancel");
            }
            ));
    }

    void OnPurchaseComplete(bool success, UnityEngine.Purchasing.PurchaseProcessingResult result, string productID)
    {
        if (success)
        {
            Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.OK, "Ads removed successful"));
            AdsManager.Instance.HideBanner();
            removeAdsBtn.SetActive(false);
            FirebaseManager.LogEvent("Result_RemoveAds_Success");
        }
        else
        {
            Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.OK, $"There was a problem processing {productID}, please contact the developer."));
        }
    }

    void RestoreBgm()
    {
        AudioManager.BgmVolume = bgmVolumeToRestore;
    }

    public override void OnKeyBack()
    {
    }

    private void OnDestroy()
    {
        RestoreBgm();
        if (sceneData.needUnloadAtlasOnWin)
            AddressablePreloader.Unload(AddressableImage.GetAtlasAddressFromPictureName(sceneData.stageData.picture_name));
        Resources.UnloadUnusedAssets();
    }
}