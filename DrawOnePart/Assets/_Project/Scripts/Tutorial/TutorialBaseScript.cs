using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialBaseScript : MonoBehaviour
{
    [SerializeField] protected TMP_Text textDialog;
    [SerializeField] bool destroyOnDone = true;
    [HideInInspector] public TutorialData m_Data;

    public delegate void CallbackDoneTutorial();
    public CallbackDoneTutorial callbackDoneTutorial;

    bool isUnexpectedDestroy = true;

    public virtual void Setup(TutorialData data, GameObject initObject = null)
    {
        m_Data = data;
        if (textDialog != null) { textDialog.text = data.textDialog; }
    }

    public virtual void OnDoneTutorial()
    {
        if (!TutorialManager.HasSeenTutorial(m_Data))
        {
            isUnexpectedDestroy = false;
            TutorialManager.CompleteTutorial(m_Data);
            callbackDoneTutorial?.Invoke();
            if (destroyOnDone) Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (isUnexpectedDestroy) TutorialManager.OnTutorialEndUnexpected();
    }
}
