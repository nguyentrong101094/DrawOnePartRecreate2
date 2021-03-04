using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Const
{
    public const int BUILD_VERSION_CODE = 10;
    public const int ADS_PRIORITY_VERSION = 3;

    public const string PREF_EXIST_DATABASE_VERSION_CODE = "GameDatabaseVersion";
    public const string PREF_VIBRATION = "VibrateSetting";
    public const int PREF_VIBRATION_DEFAULT = 1;
    public const string PREF_HAS_REVIEWED = "HasReviewed";
    public const string PREF_NO_ADS = "PURCHASE_ADS";
    public const string PREF_OPEN_COUNT = "OPEN_COUNT"; //count how many time user opened the game

    //public const string GESTURE_FOLDER_NAME = "Gestures"; //TODO: if use gesture v2, then move old gesture folder out of resources
    public const string GESTURE_FOLDER_NAME = "Gestures_V3";
    public const string GESTURE_V4_FOLDER_NAME = "Gestures_V4";
    public const string PICTURE_FOLDER_NAME = "Pictures";
    public const string GESTURE_LIBRARY_TEMPLATE_NAME = "_TEMPLATE";

    public const float GESTURE_SCORE_REQUIRED_DEFAULT = 0.7f; //minimum score required to pass gesture recognition
    public const float GESTURE_V4_SCORE_REQUIRED_DEFAULT = 0.8f; //minimum score required to pass gesture recognition

    public const float ADDITION_PLAY_FIELD_Y = 80f;
    public const float ADDITION_PLAY_FIELD_Y_WORLD = 0.4f;

    public const string PICTURE_SOLUTION_SUFFIX = "_2";

    public const string PROGRESS_CURRENT_STAGE = "CurrentStage";
    public const string PROGRESS_UNLOCKED_STAGE = "UnlockedStage";
    public const int MAX_STAGE = 490;

    public const string BGM_GAME = "jump and run - tropics";

    public const string DIALOG_ICON_OPPS_PATH = "Popup/Oppss";
    public const string DIALOG_ICON_NOINTERNET_PATH = "Popup/NoInternet";

    public const string FEEDBACK_MAIL = "puzzleworld.developer@gmail.com";
    public static string PLAY_STORE_APP_URL => $"market://details?id={Application.identifier}";
    public static string PLAY_STORE_URL => $"https://play.google.com/store/apps/details?id={Application.identifier}";

    public static string RMCF_ADS_PRIORITY => $"ads_priority_{ADS_PRIORITY_VERSION}";
    public const string RMCF_TIME_BETWEEN_ADS = "time_between_inter_ads";
    public const string RMCF_SHOW_ADS_FIRST_OPEN = "show_ads_first_open";
    public const string RMCF_NOTI_DAY_2 = "remind_play_day_2";
    public const string RMCF_NOTI_DAY_4 = "remind_play_day_4";
    public const string RMCF_NOTI_DAY_6 = "remind_play_day_6";
    public const string RMCF_USE_DATABASE_V4 = "use_database_v4";
    public const string RMCF_REQUIRE_INTERNET = "require_internet";

    public const string NOTI_TITLE = "Draw One Part";
    public const int NOTI_REPEAT_ID_DAY_2 = 10000;
    public const int NOTI_REPEAT_ID_DAY_4 = 10001;
    public const int NOTI_REPEAT_ID_DAY_6 = 10002;

    public const string MSG_NO_INTERNET = "No internet. Please check your connection and try again!";

    public const string CRASHLYTIC_LOG_PICTURE = "PictureName";
    public const string GAMEID = "3912021";
}
