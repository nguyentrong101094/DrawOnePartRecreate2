using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SendMail : MonoBehaviour
{
    //[SerializeField] string m_Email = "me@example.com";
    public string m_Subject = "My Subject";
    public string m_Body = "My Body\r\nFull of non-escaped chars";

    public void SendEmail()
    {
        Send(m_Subject, m_Body);
    }

    public static void Send(string subject, string body)
    {
        subject = MyEscapeURL(subject);
        body = MyEscapeURL(body);
        Application.OpenURL("mailto:" + Const.FEEDBACK_MAIL + "?subject=" + subject + "&body=" + body);
    }

    static string MyEscapeURL(string url)
    {
        return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
    }
}
