using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseCloudMessagingHelper : MonoBehaviour
{
    static bool initSuccess = false;

    public void Awake()
    {
        if (FirebaseManager.FirebaseReady) OnFirebaseReady(this, true);
        else FirebaseManager.handleOnReady += OnFirebaseReady;
    }

    void OnFirebaseReady(object sender, bool isReady)
    {
        initSuccess = isReady;
        if (isReady)
        {
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        }
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
    }
}