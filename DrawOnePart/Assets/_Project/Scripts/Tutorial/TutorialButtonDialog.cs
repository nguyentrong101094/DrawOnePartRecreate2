using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialButtonDialog : TutorialBaseScript
{
    Button m_Button;

    public override void Setup(TutorialData data, GameObject initObject = null)
    {
        base.Setup(data, initObject);
        m_Button = initObject.GetComponent<Button>();
        m_Button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        base.OnDoneTutorial();
    }
}
