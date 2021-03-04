using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimLoop : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    float animSampleRate = 6f;
    float AnimChangeInterval { get => 1f / animSampleRate; }
    Image m_Image;
    float timerAnim;

    private void Start()
    {
        m_Image = GetComponent<Image>();
    }

    private void Update()
    {
        int frameID = (int)((timerAnim / AnimChangeInterval) % sprites.Length);
        m_Image.sprite = sprites[frameID];
        timerAnim += Time.deltaTime;
    }
}
