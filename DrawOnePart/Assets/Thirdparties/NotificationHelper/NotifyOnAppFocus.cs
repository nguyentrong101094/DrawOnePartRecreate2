using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;

public class NotifyOnAppFocus : MonoBehaviour
{

    public static NotifyOnAppFocus instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        CheckOpen();
    }

    public static void CancelGameReminder()
    {
        //NotificationHelper.Cancel(NOTI_UNFINISH_GAME_REMINDER);
    }

#if UNITY_ANDROID
    public static AndroidNotificationIntentData CheckOpen()
    {
        var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
        if (notificationIntentData != null)
        {
            switch (notificationIntentData.Id)
            {
                case Const.NOTI_REPEAT_ID_DAY_2:
                    FirebaseManager.LogEvent("Noti_Open_Day_2", "open_time", $"{DateTime.Now.Hour}:{DateTime.Now.Minute}");
                    break;
                case Const.NOTI_REPEAT_ID_DAY_4:
                    FirebaseManager.LogEvent("Noti_Open_Day_4", "open_time", $"{DateTime.Now.Hour}:{DateTime.Now.Minute}");
                    break;
                case Const.NOTI_REPEAT_ID_DAY_6:
                    FirebaseManager.LogEvent("Noti_Open_Day_6", "open_time", $"{DateTime.Now.Hour}:{DateTime.Now.Minute}");
                    break;
            }
            NotificationHelper.CancelDisplayed(notificationIntentData.Id);
        }
        return notificationIntentData;
    }
#else
    public static void CheckOpen() { Debug.Log("Dummy Check notification open"); }
#endif

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            /*if (User.data.GetCurrentSave() != null)
            {
                if (User.data.GetCurrentSave().is_completed)
                {
                    NotificationHelper.ScheduleNotification(Const.NOTI_TITLE, "Complete your challenge to receive x2 diamond", DateTime.Now + TimeSpan.FromHours(3), Const.NOTI_UNFINISH_GAME_REMINDER);
                }
                else //if (NotificationHelper.GetStatus(Const.NOTI_AFTER_UNFINISH_PLAY) == NotificationStatus.Scheduled)
                {
                    CancelGameReminder();
                }
            }*/
        }
        else
        {
            CheckOpen();
        }
    }
}