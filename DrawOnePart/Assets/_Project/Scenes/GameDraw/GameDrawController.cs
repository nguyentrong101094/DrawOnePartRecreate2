using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;
using System.Linq;
using GestureRecognizer;
using TMPro;
using UnityEngine.UI;
using Gront;
using System;

public class GameSceneData
{
    public int stageID = -1;
    public string pictureID;

    public GameSceneData(int stageID)
    {
        this.stageID = stageID;
    }

    public GameSceneData(string pictureID)
    {
        this.pictureID = pictureID;
    }
}

[Flags]
public enum GameState { Unset = 0, Playing = 1, Won = 2, Pause = 4 }

public class GameDrawController : Controller
{
    public const string GAMEDRAW_SCENE_NAME = "GameDraw";

    public override string SceneName()
    {
        return GAMEDRAW_SCENE_NAME;
    }

    [SerializeField] DrawDotStageSetup drawDotStageSetup;
    public GestureRecognizerInGame gestureRecognizer;
    [SerializeField] PlayerFingerDraw playerFingerDraw;
    [SerializeField] Image pictureDrawing;
    public StagePictureControl levelPicture;
    [SerializeField] HintDrawDisplay hintDrawDisplay;
    [SerializeField] TMP_Text debugText;
    StageData stageData;
    PictureData currentPictureData;

    [SerializeField] GameObject showHintButton;
    [SerializeField] GameObject nextStageBtn;
    [SerializeField] GameObject[] hideOnWin;
    [SerializeField] GameObject removeAdsBtn;
    [SerializeField] TMP_Text levelNumberText;
    [SerializeField] GameObject fxOnWin;
    [SerializeField] GameObject noInternetBlock;

    [SerializeField] TutorialData drawTutorialData;
    [SerializeField] DragGuide dragGuide;
    bool isShowingTutorial;

    GameState gameState = GameState.Playing;

    float timePlayed;
    List<float> gesturesScores; //collect every gestures by player in this stage to get average score

    GameSceneData sceneData;

    bool needUnloadAtlasOnWin = true; //set to false if next stage use the same atlas as this stage
    bool usedHint; //if user used a hint, they won't have to use hint to see it again
    bool usedSkip;

    float timerCheckInternet;
    float internetCheckInterval = 0.1f;
    bool hasLogEventNoInternetWhilePlay = false;
    bool hasLogEventTurnOnInternetWhilePlay = false;

    public static GameDrawController instance;

    public override void OnActive(object data)
    {
        base.OnActive(data);
        string pictureName;
        if (data != null)
        {
            sceneData = data as GameSceneData;
        }
        else
        {
            int stageToLoad = User.GetContinueStage();
            //User.SaveStageProgress(1);
            sceneData = new GameSceneData(stageToLoad);
        }

        if (sceneData.stageID < 0)
        {
            Debug.Log("Stage ID < 0, loading stage using picture name instead");
            pictureName = sceneData.pictureID;
#if SQLITE4UNITY
            stageData = GameDatabase.Service.GetStageDataFromPicture(pictureName);
            if (stageData == null)
            {
                stageData = GameDatabase.Service.GetStageData(1);
            }
#else
            not implemented
            stageData = DatabaseSimpleSQL.Instance.GetStageData(1);
#endif
        }
        else
        {
#if SQLITE4UNITY
            stageData = GameDatabase.Service.GetStageData(sceneData.stageID);
# else
            stageData = DatabaseSimpleSQL.Instance.GetStageData(sceneData.stageID);
#endif
            pictureName = stageData.picture_name;
        }

#if SQLITE4UNITY
        currentPictureData = GameDatabase.Service.GetPictureData(pictureName);
#else
        pictureData = DatabaseSimpleSQL.Instance.GetPictureData(pictureName);
#endif
        DebugManager.CurrentStageName = $"{pictureName}--{stageData.id}";
        FirebaseManager.SetCustomKey(Const.CRASHLYTIC_LOG_PICTURE, DebugManager.CurrentStageName);
        Debug.Log(pictureName);

        //V3
        if (!currentPictureData.use_v4)
        {
            gestureRecognizer.Setup(currentPictureData);
            GestureRecognizerInGame.OnRecognition += OnGestureRecognition;
        }
        else
        {
            drawDotStageSetup.onGestureLibraryLoaded += OnGestureLibraryLoaded;
            drawDotStageSetup.Setup(currentPictureData);
            playerFingerDraw.onBeginDraw += drawDotStageSetup.OnDrawBegin;
            playerFingerDraw.onUpdateDraw += drawDotStageSetup.CheckDrawCorrect;
            playerFingerDraw.onClearDraw += drawDotStageSetup.JudgeScore;
            drawDotStageSetup.onDrawScore += OnDrawRecognition;
        }

        levelPicture.Setup(pictureName);

        levelNumberText.text = $"Level {stageData.id}";
        AudioManager.Instance.PlayBgm(Const.BGM_GAME);
        gesturesScores = new List<float>();
        if (stageData.IsTracked())
        {
            FirebaseManager.LogEvent("Game_Start", "stage_number", stageData.id.ToString());
        }
        User.SetCurrentStage(stageData.id);
    }

    public override void OnShown()
    {
        base.OnShown();
        AdsManager.Instance.ShowBanner(AdPlacementType.Banner);

        if (stageData.id < Const.MAX_STAGE)
        {
            //check if next stage use same atlas as current stage
            StageData nextStageData = GameDatabase.Service.GetStageData(stageData.id + 1);
            string nextPictureName = GameDatabase.Service.GetPictureData(nextStageData.picture_name).name;
            string nextStageAtlasName = AddressableImage.GetAtlasAddressFromPictureName(nextPictureName);
            string currentStageAtlasName = AddressableImage.GetAtlasAddressFromPictureName(currentPictureData.name);
            if (string.Equals(nextStageAtlasName, currentStageAtlasName))
            {
                Debug.Log("next stage use same atlas, skip unloading on win");
                needUnloadAtlasOnWin = false;
            }
            else //Preload next stage atlas
            {
                StageResLoader.Instance.GetSpriteFromAtlasAsync(nextPictureName, null);
                //_ = AddressableImage.GetSpriteFromAtlasAsync(nextPictureName);
            }
        }

        if (InternetCheck.InternetReachable())
        {
            FirebaseManager.LogEvent("Game_Internet_Reachable", "stage_number", stageData.id);
        }
        else
        {
            FirebaseManager.LogEvent("Game_Internet_NotReachable", "stage_number", stageData.id);
        }
    }

    private void Awake()
    {
        instance = this;
        if (IAPProcessor.CheckNoAds()) { removeAdsBtn.SetActive(false); }
        //TenjinWrapper.LogEvent("Game_Start");
        FirebaseManager.SetUserProperties("VersionCode", Const.BUILD_VERSION_CODE.ToString());
    }

    void OnGestureLibraryLoaded(object sender, GestureLibrary gestureLibrary)
    {
        if (stageData.id <= 3 && !TutorialManager.HasSeenTutorial(drawTutorialData))
        {
            List<Vector2> gesturePoints = gestureLibrary.Library[0].Points;
            Vector3[] points = new Vector3[gesturePoints.Count];
            for (int i = 0; i < gesturePoints.Count; i++)
            {
                points[i] = gesturePoints[i];
            }
            dragGuide.FollowPath(points);
            isShowingTutorial = true;
        }
    }

    void OnGestureRecognition(Result r)
    {
        SetMessage($"Gesture: <color=#990000>{r.Name}</color>, score:{r.Score}");
        if (gameState == GameState.Playing)
        {
            gesturesScores.Add(r.Score);
            if (r.Score >= currentPictureData.ScoreRequired)
            {
                Win();
            }
        }
    }

    void OnDrawRecognition(float score)
    {
        SetMessage($"Score:{score}/{currentPictureData.ScoreRequired}");
        if (gameState == GameState.Playing && score >= currentPictureData.ScoreRequired)
        {
            Win();
        }
    }

    void Win()
    {
        if (gameState != GameState.Won)
        {
            levelPicture.OnCorrect();
            StartCoroutine(OnWin());
        }

        if (isShowingTutorial)
        {
            dragGuide.HideDragGuide();
        }
    }


    /// <summary>
    /// Shows a message at the bottom of the screen
    /// </summary>
    /// <param name="text">Text to show</param>
    public void SetMessage(string text)
    {
        debugText.text = text;
        Debug.Log(text);
    }

    IEnumerator OnWin()
    {
        gameState = GameState.Won;
        showHintButton.SetActive(false);
        nextStageBtn.SetActive(true);
        foreach (var item in hideOnWin)
        {
            item.SetActive(false);
        }

        if (sceneData.stageID >= 0)
        {
            User.OnStageComplete(stageData.id);
        }

        //playerFingerDraw.paintHitScreen.enabled = false;
        playerFingerDraw.paintDrawRenderer.isEnabled = false;
        fxOnWin.SetActive(true);

        if (PlayerPrefs.GetInt(Const.PREF_VIBRATION, Const.PREF_VIBRATION_DEFAULT) == 1)
        {
            Debug.Log("Vibrating");
            Vibration.VibratePop();
        }

        AudioManager.Instance.PlaySfx("correct_buzzer_sound_effect");
        LogStageResult();

        yield return new WaitForSeconds(1.2f);

        //Update: don't show result scene
        /*if (!usedSkip && !stageData.IsRateStage() && AdsManager.instance.HasEnoughTimeBetweenInterstitial())
        {
            AdsManager.Instance.RequestInterstitialNoShow(AdPlacementType.Interstitial, (bool success) =>
            {
                if (success) AdsManager.Instance.ShowInterstitial(AdPlacementType.Interstitial, (bool successShow) =>
                {
                    //after showing complete level interstitial, preload next ads
                    Debug.Log("GameDrawController: Showed complete level interstitial, preloading next ads");
                    AdsManager.Instance.RequestInterstitialNoShow(AdPlacementType.Interstitial, null, false);
                });
                GoToResultScene(success);
            });
        }
        else { GoToResultScene(false); }*/
    }

    void LogStageResult()
    {

        //Log events
        FirebaseManager.SetUserProperties("LastStageCompleted", stageData.id.ToString());
        FirebaseManager.SetUserProperties("StageUnlocked", User.GetLastUnlockedStage().ToString());
        if (stageData.IsTracked())
        {
            FirebaseManager.LogEvent("Game_Victory", "stage_number", stageData.id.ToString());
        }
        FirebaseManager.LogEvent("Game_Victory", "stage_name", currentPictureData.name);
        FirebaseManager.LogEvent("Game_Victory", "play_time", timePlayed);

        //log individual stage score
        if (stageData.IsTracked())
        {
            FirebaseManager.LogEvent($"Game_Victory_Stage{stageData.id}", "play_time", timePlayed);
            FirebaseManager.LogEvent($"Game_Victory_Stage{stageData.id}", "try_number", gesturesScores.Count);
            if (gesturesScores.Count > 0)
            {
                float totalScore = 0f;
                for (int i = 0; i < gesturesScores.Count; i++)
                {
                    totalScore += gesturesScores[i];
                }
                FirebaseManager.LogEvent($"Game_Victory_Stage{stageData.id}", "avg_accuracy", totalScore / gesturesScores.Count);
            }
        }
    }

    void GoToResultScene(bool interAdsWasShown)
    {
        StageResult stageResult = new StageResult(stageData.id, timePlayed);
        Manager.Add(ResultController.RESULT_SCENE_NAME, new ResultController.SceneData(stageResult, stageData, interAdsWasShown, needUnloadAtlasOnWin));
    }

    public void EditStage()
    {
        Manager.Load(LevelMakerController.LEVELMAKER_SCENE_NAME, currentPictureData.name);
    }

    public void OnNextStageButton()
    {
        if (stageData.IsRateStage())
        {
            Manager.Add(RatePopupController.RATEPOPUP_SCENE_NAME,
                new PopupData(PopupType.OK, "Thank you for your feedback!",
            ConfirmGoToNextStage));
            FirebaseManager.LogEvent($"Rate_ShowStage_{stageData.id}");
        }
        else if (!usedSkip && AdsManager.instance.HasEnoughTimeBetweenInterstitial() && stageData.id < 3) //don't show ads if used skip or not enough time passed between interstitial
        {
            AdsManager.Instance.RequestInterstitialNoShow(AdPlacementType.Interstitial, (bool success) =>
            {
                if (success) AdsManager.Instance.ShowInterstitial(AdPlacementType.Interstitial, (bool successShow) =>
                {
                    //after showing complete level interstitial, preload next ads
                    Debug.Log("GameDrawController: Showed complete level interstitial, preloading next ads");
                    AdsManager.Instance.RequestInterstitialNoShow(AdPlacementType.Interstitial, null, false);
                });
                ConfirmGoToNextStage();
            });
        }
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
            /*Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.YES_NO, "Max stage reached, play from beginning?",
                () =>
                {
                    Manager.Load(GameDrawController.GAMEDRAW_SCENE_NAME);
                }));*/
            Manager.Add(StageComingSoonController.STAGECOMINGSOON_SCENE_NAME);
            //Manager.Load(StageSelectController.STAGESELECT_SCENE_NAME);
        }
    }

    public void OnSkipBtn()
    {
        //hintDrawDisplay.ShowHint();
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Manager.Add(PopupWithImageController.POPUP_SCENE_NAME, new PopupWithImageData(PopupType.OK, Const.MSG_NO_INTERNET).SetImagePath(Const.DIALOG_ICON_NOINTERNET_PATH));
        }
        else
        {
            AdsManager.Reward((bool success) =>
            {
                if (success)
                {
                    usedSkip = true;
                    Win();
                    FirebaseManager.LogEvent("Game_RewardSkip_Success", "stage_number", stageData.id.ToString());
                }
            }, AdPlacementType.Reward_Skip);
            //Manager.Add(PopupWithImageController.POPUP_SCENE_NAME, new PopupWithImageData(PopupType.OK, "No ads available. Please try again later!").SetImagePath(Const.DIALOG_ICON_OPPS_PATH));
        }
        FirebaseManager.LogEvent("Game_RewardSkip_BtnClick", "stage_number", stageData.id.ToString());
        //Win();
    }

    public void OnHintBtn()
    {
        if (gameState != GameState.Playing) return;
        TogglePause(true);
        if (User.data.hintAcquired > 0 || usedHint)
        {
            if (!usedHint)
            {
                usedHint = true;
                User.AddHint(-1);
            }
            Manager.Add(HintDialogController.HINTDIALOG_SCENE_NAME, currentPictureData.name, null, Unpause);
            FirebaseManager.LogEvent("Game_HintShow_Success", "stage_number", stageData.id);
            FirebaseManager.LogEvent("Game_HintShow_Success", "stage_name", stageData.picture_name);
        }
        else
        {
            Manager.Add(GetHintController.GETHINT_SCENE_NAME, null, null, Unpause);
            FirebaseManager.LogEvent("Game_HintBtnClick_NoHintLeft", "stage_number", stageData.id);
            FirebaseManager.LogEvent("Game_HintBtnClick_NoHintLeft", "stage_name", stageData.picture_name);
        }
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
            FirebaseManager.LogEvent("Game_RemoveAds_Success");
        }
        else
        {
            Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.OK, $"There was a problem processing {productID}, please contact the developer."));
        }
    }

    public void OnSettingBtn()
    {
        if (gameState == GameState.Playing)
        {
            TogglePause(true);
            Manager.Add(SettingController.SETTING_SCENE_NAME, null, null, Unpause);
        }
    }

    void Unpause() => TogglePause(false);

    public void TogglePause(bool value)
    {
        if (value)
        {
            SubtractGameState(GameState.Playing);
        }
        else
        {
            AddGameState(GameState.Playing);
        }
    }

    public void SetGameState(GameState state) { gameState = state; }

    //add a flag
    public void AddGameState(GameState state)
    {
        gameState = gameState | state;
        //Debug.Log($"{gameState} {(int)gameState}");
    }

    //subtract a flag from gamestate
    public void SubtractGameState(GameState state) { gameState = gameState ^ state; }

    public void OnHomeBtn()
    {
        //TogglePause(true);
        Manager.Load(StageSelectController.STAGESELECT_SCENE_NAME);
    }

    void ToggleNoInternetBlock(bool showBlock)
    {
        if (noInternetBlock.activeSelf != showBlock)
        {
            noInternetBlock.SetActive(showBlock);
            TogglePause(showBlock);
        }
    }

    private void Update()
    {
        if (gameState == GameState.Playing)
        {
            timePlayed += Time.deltaTime;
        }

        if (timerCheckInternet < internetCheckInterval)
        {
            timerCheckInternet += Time.unscaledDeltaTime;
        }
        else
        {
            //block game play if configured to require internet and has no internet
            ToggleNoInternetBlock(!InternetCheck.InternetReachableOptional());

            bool noInternet = !InternetCheck.InternetReachable();
            if (noInternet && !hasLogEventNoInternetWhilePlay)
            {
                FirebaseManager.LogEvent("Game_Internet_TurnedOffWhilePlay", "stage_number", stageData.id);
                hasLogEventNoInternetWhilePlay = true;
            }
            if (!noInternet && !hasLogEventTurnOnInternetWhilePlay && hasLogEventNoInternetWhilePlay)
            {
                FirebaseManager.LogEvent("Game_Internet_TurnedOnWhilePlay", "stage_number", stageData.id);
                hasLogEventNoInternetWhilePlay = true;
            }
            timerCheckInternet = 0f;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.H))
        {
            hintDrawDisplay.ShowHint();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            DebugRetry();
        }
#endif
    }

    private void OnDestroy()
    {
        GestureRecognizerInGame.OnRecognition -= OnGestureRecognition;
    }

    public override void OnHidden()
    {
        base.OnHidden();
        //moved unloading to result scene
        if (needUnloadAtlasOnWin)
            ResourcesPreloader.Unload(StageResLoader.GetAtlasAddressFromPictureName(currentPictureData.name));
        //AddressablePreloader.Unload(AddressableImage.GetAtlasAddressFromPictureName(currentPictureData.name));
        Resources.UnloadUnusedAssets();
    }

    public override void OnKeyBack()
    {
    }

    public void DebugRetry()
    {
        Manager.Load(SceneName(), new GameSceneData(currentPictureData.name));
    }

    public void DebugModeToggle()
    {
        DebugManager.forceDebugMode = !DebugManager.forceDebugMode;
        Debug.Log($"Force debug mode: {DebugManager.forceDebugMode}");
        DebugRetry();
    }

    public static void DebugStageList()
    {
        Manager.Load(StageListThumbnailController.STAGELISTTHUMBNAIL_SCENE_NAME);
    }
}