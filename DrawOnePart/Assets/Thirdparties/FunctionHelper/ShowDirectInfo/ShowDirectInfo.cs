using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Gront
{
    public class ShowDirectInfo : MonoBehaviour
    {
        [SerializeField] Canvas m_Canvas;
        [SerializeField] TextMeshProUGUI m_Text;
        [SerializeField] Canvas canvasPrefab;
        string currentText;

        private void Awake()
        {
            if (m_Canvas == null)
            {
                if (canvasPrefab == null)
                {
                    canvasPrefab = Resources.Load<Canvas>("ShowDirectInfoCanvas");
                }
                m_Canvas = Instantiate(canvasPrefab, transform);
            }
            if (m_Text == null) { m_Text = GetComponentInChildren<TextMeshProUGUI>(); }
        }

        public void SetText<T>(T obj)
        {
            SetText(obj.ToString());
        }

        public void SetText(string str)
        {
            if (!string.Equals(currentText, str))
                m_Text.text = currentText = str;
        }

        public static ShowDirectInfo MakeCanvas(MonoBehaviour sender)
        {
            ShowDirectInfo showDirectInfo = sender.GetComponent<ShowDirectInfo>();
            if (showDirectInfo == null)
            {
                showDirectInfo = sender.gameObject.AddComponent<ShowDirectInfo>();
            }
            return showDirectInfo;
        }

        private void Update()
        {
            //make sure canvas stay up right for easier reading
            m_Canvas.transform.rotation = Quaternion.identity;
        }
    }
}