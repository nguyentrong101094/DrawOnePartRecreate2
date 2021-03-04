using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalizedTMproText : MonoBehaviour
{
    [SerializeField] string id;
    [SerializeField] bool hasParam;
    [SerializeField] int param;

    TMP_Text m_Text;

    void OnEnable()
    {
        if (m_Text == null)
        {
            m_Text = GetComponent<TMP_Text>();
        }

        m_Text.text = (!hasParam) ? Localizes.GetString(id) : string.Format(Localizes.GetString(id), param);
    }
}