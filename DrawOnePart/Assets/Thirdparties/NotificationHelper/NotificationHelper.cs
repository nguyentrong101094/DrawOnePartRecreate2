using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;

public static class NotificationHelper
{
    public const string channelIdDefault = "general_channel";
    static bool isInit;

    static void Initialize()
    {
#if UNITY_ANDROID
        if (isInit) return;
        isInit = true;
        var channel = new AndroidNotificationChannel()
        {
            Id = channelIdDefault,
            Name = "General",
            Importance = Importance.Default,
            Description = "Generic notification"
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif
    }

#if UNITY_ANDROID
    public static void RegisterChannel(string id, string name, Importance importance, string description)
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = id,
            Name = name,
            Importance = importance,
            Description = description
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }
#endif

    public static void ScheduleNotification(string title, string text, DateTime fireTime, int notificationID = -1, string channel = channelIdDefault)
    {
#if UNITY_ANDROID
        Initialize();
        var notification = new AndroidNotification(title, text, fireTime);
        notification.SmallIcon = "small_icon";
        notification.LargeIcon = "large_icon";
        if (notificationID < 0)
        {
            AndroidNotificationCenter.SendNotification(notification, channel);
        }
        else
        {
            CheckAndSend(notification, channel, notificationID);
        }
#endif
    }

    public static void ScheduleRepeatNotification(string title, string text, DateTime fireTime, TimeSpan repeatInterval, int notificationID, string channel = channelIdDefault)
    {
#if UNITY_ANDROID
        Initialize();
        AndroidNotification notification = new AndroidNotification(title, text, fireTime, repeatInterval);
        notification.SmallIcon = "small_icon";
        notification.LargeIcon = "large_icon";
        CheckAndSend(notification, channel, notificationID);
#endif
    }

#if UNITY_ANDROID
    public static NotificationStatus GetStatus(int id)
    {
        return AndroidNotificationCenter.CheckScheduledNotificationStatus(id);
    }
#endif

#if UNITY_ANDROID
    static void CheckAndSend(AndroidNotification notification, string channel, int notificationID)
    {
        var notificationStatus = GetStatus(notificationID);
        Debug.Log(notificationStatus);
        switch (notificationStatus)
        {
            case NotificationStatus.Unavailable:
                Debug.Log($"{notification.Title} Unavailable");
                break;
            case NotificationStatus.Unknown:
                AndroidNotificationCenter.SendNotificationWithExplicitID(notification, channel, notificationID);
                //.Log($"Noti Sent {notification.FireTime}");
                break;
            case NotificationStatus.Scheduled:// Replace the scheduled notification with a new notification.
                AndroidNotificationCenter.UpdateScheduledNotification(notificationID, notification, channel);
                //.Log($"Noti Updated {notification.FireTime}");
                break;
            case NotificationStatus.Delivered:// Remove the previously shown notification from the status bar.
                AndroidNotificationCenter.CancelDisplayedNotification(notificationID);
                goto case NotificationStatus.Scheduled; //then update the scheduled
        }
    }
#endif

    public static void Cancel(int notificationID)
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelNotification(notificationID);
#endif
    }

    public static void CancelDisplayed(int notificationID)
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelDisplayedNotification(notificationID);
#endif
    }
}