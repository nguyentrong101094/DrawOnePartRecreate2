using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gront
{
    public class ErrorDialog : MonoBehaviour
    {
        [SerializeField] TMP_Text errorText;

        private void Start()
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        public void SetText(string text) { errorText.text = text; }

        public void AddText(string text) { errorText.text += text; }

        public void Close()
        {
            Destroy(gameObject);
        }
    }
}
