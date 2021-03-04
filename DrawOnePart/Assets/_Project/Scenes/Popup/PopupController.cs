using UnityEngine;
using SS.View;
using TMPro;
using UnityEngine.UI;

public enum PopupType
{
    OK,
    YES_NO
}

public class PopupData
{
    public string title { get; set; }
    public string text { get; set; }
    public PopupType type { get; set; }
    public Manager.Callback onOk { get; set; }
    public Manager.Callback onCancel { get; set; }
    public string okText;
    public string cancelText;

    public PopupData(PopupType type, string title, string text, Manager.Callback onOk = null, Manager.Callback onCancel = null) : base()
    {
        this.title = title;
        this.text = text;
        this.type = type;
        this.onOk = onOk;
        this.onCancel = onCancel;
    }

    public PopupData(PopupType type, string text, Manager.Callback onOk = null, Manager.Callback onCancel = null)
    : this(type, "NOTICE", text, onOk, onCancel)
    {
    }

    public void SetOkText(string okText)
    {
        this.okText = okText;
    }

    public void SetCancelText(string cancelText)
    {
        this.cancelText = cancelText;
    }
}

public class PopupController : Controller
{
    public const string POPUP_SCENE_NAME = "Popup";

    public override string SceneName()
    {
        return POPUP_SCENE_NAME;
    }

    [SerializeField] protected TextMeshProUGUI m_Title;
    [SerializeField] protected TextMeshProUGUI m_Text;
    [SerializeField] protected Button m_OkButton;
    [SerializeField] protected Button m_YesButton;
    [SerializeField] protected Button m_NoButton;

    protected PopupData m_PopupData;
    protected bool m_OkTapped;
    const bool autoAddListenerToButtons = true; //if true, all referenced button will get added listener via code

    public override void OnActive(object data)
    {
        base.OnActive(data);

        if (data != null)
        {
            m_PopupData = (PopupData)data;

            switch (m_PopupData.type)
            {
                case PopupType.OK:
                    m_OkButton.gameObject.SetActive(true);
                    m_YesButton.gameObject.SetActive(false);
                    m_NoButton.gameObject.SetActive(false);
                    break;
                case PopupType.YES_NO:
                    m_OkButton.gameObject.SetActive(false);
                    m_YesButton.gameObject.SetActive(true);
                    m_NoButton.gameObject.SetActive(true);
                    break;
            }
            m_Title.text = m_PopupData.title;
            m_Text.text = m_PopupData.text;
            if (!string.IsNullOrEmpty(m_PopupData.okText))
            {
                m_YesButton.GetComponentInChildren<TMP_Text>().text = m_PopupData.okText;
                m_OkButton.GetComponentInChildren<TMP_Text>().text = m_PopupData.okText;
            }
            if (!string.IsNullOrEmpty(m_PopupData.cancelText))
            {
                m_NoButton.GetComponentInChildren<TMP_Text>().text = m_PopupData.cancelText;
            }
        }
        else
        {
            m_PopupData = null;
        }

        if (autoAddListenerToButtons)
        {
            m_OkButton.onClick.AddListener(OnOkButton);
            m_NoButton.onClick.AddListener(OnCancelButton);
            m_YesButton.onClick.AddListener(OnOkButton);
        }
    }

    public override void OnHidden()
    {
        if (m_OkTapped)
        {
            if (m_PopupData != null && m_PopupData.onOk != null)
            {
                m_PopupData.onOk();
            }
        }
        else
        {
            if (m_PopupData != null && m_PopupData.onCancel != null)
            {
                m_PopupData.onCancel();
            }
        }
    }

    public override void OnKeyBack()
    {
        //OnCancelButton();
    }

    public virtual void OnOkButton()
    {
        Manager.Close();
        m_OkTapped = true;
    }

    public virtual void OnCancelButton()
    {
        Manager.Close();
        m_OkTapped = false;
    }

#if UNITY_EDITOR || UNITY_STANDALONE
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnOkButton();
        }
    }
#endif
}
