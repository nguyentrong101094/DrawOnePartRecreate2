using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintCounterBoard : MonoBehaviour
{
    [SerializeField] TMP_Text counterText;
    [SerializeField] GameObject plusBtn;
    [SerializeField] GameObject counterBoard;

    public static HintCounterBoard instance;
    private void Awake()
    {
        instance = this;
        SetCounterText(User.data.hintAcquired);
    }

    void SetCounterText(int gem)
    {
        counterText.text = gem.ToString();
        bool outOfHint = User.data.hintAcquired == 0;
        plusBtn.SetActive(outOfHint);
        counterBoard.SetActive(!outOfHint);
    }

    private void OnEnable()
    {
        User.callbackOnHintAcquire += SetCounterText;
    }

    private void OnDisable()
    {
        User.callbackOnHintAcquire -= SetCounterText;
    }

    private void Reset()
    {
        counterText = GetComponent<TMP_Text>();
    }
}
