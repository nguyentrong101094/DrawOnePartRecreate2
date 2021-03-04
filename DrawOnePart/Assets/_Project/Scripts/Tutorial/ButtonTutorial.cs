using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//attach this to buttons that finish a tutorial
[RequireComponent(typeof(TutorialItem))]
public class ButtonTutorial : MonoBehaviour
{
    TutorialItem m_Master;

    private void Start()
    {
        m_Master = GetComponent<TutorialItem>();
    }

    public void OnClick()
    {
        if (!m_Master.isDone)
        {
            //var tut = TutorialManager.activeTutorials.Find(x => x.m_Data.Id.Equals(m_Master.m_Data.Id));
            if (TutorialManager.activeTutorials.ContainsKey(m_Master.m_Data.Id))
            {
                TutorialManager.activeTutorials[m_Master.m_Data.Id].OnDoneTutorial();
            }
        }
    }
}
