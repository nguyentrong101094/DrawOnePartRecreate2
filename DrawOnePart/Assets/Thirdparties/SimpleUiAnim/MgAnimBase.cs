using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgAnimBase : MonoBehaviour
{
    protected RectTransform m_RectTransform;
    public const float ANIM_TIME = 0.5f;
    public float AnimTime
    {
        get
        {
            if (!isCustomAnim)
            {
                return ANIM_TIME;
            }
            else
            {
                return customAnimTime;
            }
        }
    }

    [SerializeField] bool isCustomAnim;
    [SerializeField] float customAnimTime = 0.5f;
    [SerializeField] bool hideOnAwake; //hide immediately on awake


    protected virtual void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        if (hideOnAwake) Hide(true);
    }

    public virtual void Show(bool immediately = false) { }

    public virtual void Hide(bool immediately = false) { }
}
