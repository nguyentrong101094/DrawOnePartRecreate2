using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Threading.Tasks;

public class MainController : Controller
{
    public const string MAIN_SCENE_NAME = "Main";

    public override string SceneName()
    {
        return MAIN_SCENE_NAME;
    }

    [SerializeField] Slider loadProgress;
    float loadingValue;
    bool firebaseCallbackSuccess = false;
    bool donePreloadStagePicture = false;
    bool? doneLoadAds = null;
    bool? showOpenAds = null;

    public float LoadingValue
    {
        get => loadingValue; set
        {
            loadingValue = value;
            loadProgress.value = loadingValue;
        }
    }

    IEnumerator Start()
    {
        Manager.LoadingSceneName = LoadingController.LOADING_SCENE_NAME;
        Manager.ShieldColor = new Color(0f, 0f, 0f, 0.7f);

        if (FirebaseManager.FirebaseReady) OnFirebaseReady(this, true);
        else FirebaseManager.handleOnReady += OnFirebaseReady;

        FirebaseRemoteConfigHelper.CheckAndHandleFetchConfig(SetTimeBetweenInterAds);

        //Skip waiting for firebase ready. Load ads immediately
        //FirebaseRemoteConfigHelper.CheckAndHandleFetchConfig(CheckShowOpenAds);
        CheckShowOpenAds(this, FirebaseManager.FirebaseReady);

        StartCoroutine(CoLoadingBarIncrement());

        var initAddressable = Addressables.InitializeAsync();
        while (!initAddressable.IsDone) yield return null;
        float loadingStep = 1f / 20f;
        loadingValue += loadingStep * 10;

        //FirebaseRemoteConfigHelper.CheckAndHandleFetchConfig(PreloadStagePictureAsync); //no AB test gesture this version
        PreloadStagePictureAsync(this, false);

        //PreloadStagePictureAsync();
        while (!donePreloadStagePicture) yield return null;
        loadingValue += loadingStep * 5;

        var delay = new WaitForSeconds(0.01f);

        float timerFirebaseTimeout = 2f;
        while (timerFirebaseTimeout > 0f && !firebaseCallbackSuccess)
        {
            yield return delay;
            timerFirebaseTimeout -= 0.01f;
        }
        if (!firebaseCallbackSuccess)
        {
            //firebase ready callback timeout, remove listener
            FirebaseManager.handleOnReady -= OnFirebaseReady;
        }

        /*while (!showOpenAds.HasValue) //wait for checking show open ads from firebase
        {
            yield return delay;
        }
        Debug.Log($"show open ads {showOpenAds}");
        if (showOpenAds.HasValue && showOpenAds.Value) //load open ads
        {
            AdsManager.Instance.RequestInterstitialNoShow(AdPlacementType.Inter_Splash, OnFinishLoadAds, false);
        }
        else //preload ingame interstitial
        {
            AdsManager.Instance.RequestInterstitialNoShow(AdPlacementType.Interstitial, null, false);
            doneLoadAds = false; //skip code that show open ads
        }*/

        loadingValue += loadingStep * 4;

        while (!doneLoadAds.HasValue) //wait for open ads load
        {
            yield return delay;
        }

        LoadingValue = 1f;

        if (doneLoadAds.HasValue && doneLoadAds.Value)
        {
            AdsManager.Instance.ShowInterstitial(AdPlacementType.Inter_Splash);
        }

        if (User.HasReachedMaxStage())
        {
            CancelNotification();
            Manager.Load(StageSelectController.STAGESELECT_SCENE_NAME);
            //Manager.Load(StageComingSoonController.STAGECOMINGSOON_SCENE_NAME);
        }
        else
        {
            ScheduleNotification();
            Manager.Load(GameDrawController.GAMEDRAW_SCENE_NAME);
        }
    }

    IEnumerator CoLoadingBarIncrement()
    {
        while (loadProgress.value < 0.9f)
        {
            while (loadProgress.value < loadingValue)
            {
                loadProgress.value += 0.004f;
                yield return null;
            }
            loadProgress.value += 0.0002f;
            yield return null;
        }
    }

    async void PreloadStagePictureAsync(object sender, bool success)
    {
        //if (success)
        //    GameDatabase.GetRemoteConfig();
        StageData stageData = GameDatabase.Service.GetStageData(User.GetCurrentStage());
        if (stageData == null)
        {
            Debug.Log($"stage data {User.GetCurrentStage()} not found");
            stageData = GameDatabase.Service.GetStageData(1);
        }
        string pictureName = GameDatabase.Service.GetPictureData(stageData.picture_name).name;
        //await AddressableImage.GetSpriteFromAtlasAsync(pictureName);
        bool loadComplete = false;
        StageResLoader.Instance.GetSpriteFromAtlasAsync(pictureName, (spr) =>
        {
            loadComplete = true;
        });
        while (!loadComplete)
        {
            await Task.Delay(50);
        }
        donePreloadStagePicture = true;
    }

    void OnFirebaseReady(object sender, bool isReady)
    {
        FirebaseManager.LogEvent("Splash_Show");
        FirebaseManager.LogScreenView("Splash");
        firebaseCallbackSuccess = true;
    }

    static void SetTimeBetweenInterAds(object sender, bool firebaseSuccess)
    {
        if (firebaseSuccess)
        {
            AdsManager.TIME_BETWEEN_ADS = FirebaseRemoteConfigHelper.GetFloat(Const.RMCF_TIME_BETWEEN_ADS, AdsManager.TIME_BETWEEN_ADS);
        }
    }

    void CheckShowOpenAds(object sender, bool firebaseSuccess)
    {
        int openTime = PlayerPrefs.GetInt(Const.PREF_OPEN_COUNT, 0);
        if (firebaseSuccess)
        {
            if (openTime == 0)
            {
                //showOpenAds = FirebaseRemoteConfigHelper.GetBool(Const.RMCF_SHOW_ADS_FIRST_OPEN, false);
                showOpenAds = false; //skip checking remote config
            }
            else
            {
                showOpenAds = true;
            }
            //showOpenAds = FirebaseRemoteConfigHelper.GetBool(Const.RMCF_SHOW_ADS_FIRST_OPEN, false);
        }
        else
        {
            showOpenAds = (openTime != 0);
        }

        Debug.Log($"show open ads {showOpenAds}");
        if (showOpenAds.HasValue && showOpenAds.Value) //load open ads
        {
            AdsManager.Instance.RequestInterstitialNoShow(AdPlacementType.Inter_Splash, OnFinishLoadAds, false);
        }
        else //preload ingame interstitial
        {
            AdsManager.Instance.RequestInterstitialNoShow(AdPlacementType.Interstitial, null, false);
            doneLoadAds = false; //skip code that show open ads
        }

        openTime++; PlayerPrefs.SetInt(Const.PREF_OPEN_COUNT, openTime);
        FirebaseManager.CheckWaitForReady((object senderFb, bool success) =>
        {
            FirebaseManager.LogEvent("Splash_OpenApp", "open_app_count", openTime.ToString());
        });
    }

    void OnFinishLoadAds(bool success)
    {
        Debug.Log($"main inter ads load {success}");
        doneLoadAds = success;
    }

    void ScheduleNotification()
    {
        if (FirebaseRemoteConfigHelper.GetBool(Const.RMCF_NOTI_DAY_2, true))
        {
            NotificationHelper.ScheduleNotification("noti_remindplay_2_title".LC(), "noti_remindplay_2_message".LC(),
            GetTimeFromToday(2, 19, 30), Const.NOTI_REPEAT_ID_DAY_2);
        }
        if (FirebaseRemoteConfigHelper.GetBool(Const.RMCF_NOTI_DAY_4, true))
        {
            NotificationHelper.ScheduleNotification("noti_remindplay_4_title".LC(), "noti_remindplay_4_message".LC(),
            GetTimeFromToday(4, 19, 30), Const.NOTI_REPEAT_ID_DAY_4);
        }
        if (FirebaseRemoteConfigHelper.GetBool(Const.RMCF_NOTI_DAY_6, true))
        {
            NotificationHelper.ScheduleNotification("noti_remindplay_6_title".LC(), "noti_remindplay_6_message".LC(),
            GetTimeFromToday(6, 19, 30), Const.NOTI_REPEAT_ID_DAY_6);
        }
    }

    void CancelNotification()
    {
        NotificationHelper.Cancel(Const.NOTI_REPEAT_ID_DAY_2);
        NotificationHelper.Cancel(Const.NOTI_REPEAT_ID_DAY_4);
        NotificationHelper.Cancel(Const.NOTI_REPEAT_ID_DAY_6);
    }

    DateTime GetTimeFromToday(int day, int hour, int minute)
    {
        DateTime remindTime = DateTime.Today + new TimeSpan(day, hour, minute, 0);
        return remindTime;
    }

    DateTime GetTimeFromNow(int day, int hour, int minute)
    {
        DateTime remindTime = DateTime.Now + new TimeSpan(day, hour, minute, 0);
        return remindTime;
    }

    public override void OnKeyBack()
    {
    }
}