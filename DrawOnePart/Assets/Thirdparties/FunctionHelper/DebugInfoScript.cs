using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gront.Helper
{
    public class DebugInfoScript : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI fps;
        [SerializeField] GameObject debugMenu;
        //Declare these in your class
        int m_frameCounter = 0;
        float m_timeCounter = 0.0f;
        float m_lastFramerate = 0.0f;
        public float m_refreshTime = 0.5f;
        [SerializeField] bool showFps = false;
        //bool isFastForward = false;

        public void ToggleMenu(bool on)
        {
            debugMenu.SetActive(on);
        }

        public void ToggleFPS(bool on)
        {
            showFps = on;
            fps.text = string.Empty;
        }

        public void ToggleFastForward(bool on)
        {
            //isFastForward = on;
            Time.timeScale += 2f;
            if (Time.timeScale >= 8f) Time.timeScale = 1f;
            //Time.timeScale = isFastForward ? 10f : 1f;
            //if (Time.timeScale <= 1f)
            //    Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        public void OnWinButton()
        {
            //GameObject.FindObjectOfType<GameDefendController>().OnEvent(StageResult.win);
        }

        void Update()
        {
            if (showFps)
            {
                if (m_timeCounter < m_refreshTime)
                {
                    m_timeCounter += Time.deltaTime;
                    m_frameCounter++;
                }
                else
                {
                    //This code will break if you set your m_refreshTime to 0, which makes no sense.
                    m_lastFramerate = (float)m_frameCounter / m_timeCounter;
                    m_frameCounter = 0;
                    m_timeCounter = 0.0f;
                }
                fps.text = string.Format($"FPS: {m_lastFramerate}");
            }
        }
    }
}