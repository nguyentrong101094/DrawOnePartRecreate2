using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalizedTMproDirect : MonoBehaviour
{
    //[SerializeField] string id;
    [SerializeField] bool hasParam;
    [SerializeField] int param;

    string localizeKey;
    TMP_Text m_Text;

    void OnEnable()
    {
        if (m_Text == null)
        {
            m_Text = GetComponent<TMP_Text>();
        }
        localizeKey = m_Text.text;
        UpdateText();
    }

    public void UpdateText()
    {
        m_Text.text = (!hasParam) ? Localizes.GetString(localizeKey) : string.Format(Localizes.GetString(localizeKey), param);
    }
}