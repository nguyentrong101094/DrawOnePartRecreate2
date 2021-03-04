using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialItem : MonoBehaviour
{
    public TutorialData m_Data;
    public bool checkOnStart = true; //set to false if you're going to chain tutorial
    [SerializeField] List<Image> imagesToDisable = new List<Image>(); //disable raycast of potential button that is inside spot light
    public UnityEvent onDoneThisTutorial; //use to chain tutorial
    [SerializeField] bool deactivateOnDone;
    [SerializeField] bool isRepeatTutorial; //if true, isDone variable won't be set on OnDoneTutorial

    TutorialBaseScript m_Tut;
    [HideInInspector] public bool isDone;

    public TutorialBaseScript TutObject { get => m_Tut; set => m_Tut = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (!isDone)
        {
            if (TutorialManager.HasSeenTutorial(m_Data))
            {
                isDone = true;
                if (deactivateOnDone)
                {
                    gameObject.SetActive(false);
                }
                return;
            }
        }
        if (checkOnStart)
        {
            Init();
        }
    }

    public void Init()
    {
        if (!isDone && TutorialManager.CanShowTutorial(m_Data))
        {
            //m_Tut = Instantiate(m_Data.tutObject, transform);
            m_Tut = TutorialManager.ShowTutorial(this);
            m_Tut.transform.SetParent(transform.parent);
            m_Tut.Setup(m_Data, gameObject);
            m_Tut.callbackDoneTutorial -= HandleOnDoneTutorial;
            m_Tut.callbackDoneTutorial += HandleOnDoneTutorial;
            ToggleNeighborImagesRaycast(false);
            //isDone = false;
            //(m_Tut);
        }
    }

    public void DoneTutorial() //called via unity event from UI in game
    {
        if (!isDone && m_Tut != null)
        {
            if (!isRepeatTutorial)
                isDone = true;
            m_Tut.OnDoneTutorial(); //this will call HandleOnDoneTutorial next through callback
        }
    }

    void HandleOnDoneTutorial()
    {
        ToggleNeighborImagesRaycast(true);
        onDoneThisTutorial.Invoke();
        if (deactivateOnDone)
        {
            gameObject.SetActive(false);
        }
    }

    void ToggleNeighborImagesRaycast(bool enable)
    {
        for (int i = 0; i < imagesToDisable.Count; i++)
        {
            imagesToDisable[i].raycastTarget = enable;
        }
    }
}
