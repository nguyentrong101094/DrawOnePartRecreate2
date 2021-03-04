
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonRepeat : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private float m_repeatDelay = 1f;
    [SerializeField]
    private float m_repeatRetriggerInterval = 0.1f;
    private Button m_button;
    private PointerEventData m_pointerData = null;
    private float m_lastTrigger = 0;

    private void Awake()
    {
        m_button = transform.GetComponent<Button>();
    }

    private void Update()
    {
        if (m_pointerData != null)
        {
            if (Time.realtimeSinceStartup - m_lastTrigger >= m_repeatDelay)
            {
                m_lastTrigger = Time.realtimeSinceStartup - (m_repeatDelay - m_repeatRetriggerInterval);
                m_button.OnSubmit(m_pointerData);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_lastTrigger = Time.realtimeSinceStartup;
        m_pointerData = eventData;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_pointerData = null;
        m_lastTrigger = 0f;
    }
}