using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageThumbnailItem : MonoBehaviour
{
    [SerializeField] Image m_Image;
    [SerializeField] TMP_Text m_Text;
    public StageData stageData;
    public event System.EventHandler onClick;

    public void Setup(StageData stageData)
    {
        this.stageData = stageData;
        m_Text.text = $"{stageData.id} - {stageData.picture_name}";
    }

    public async Task SetImage()
    {
        /*Sprite spr = await AddressableImage.GetSpriteFromAtlasAsync(stageData.picture_name);
        if (m_Image != null)
            m_Image.sprite = spr;*/

        StageResLoader.Instance.GetSpriteFromAtlasAsync(stageData.picture_name, (spr) =>
        {
            if (m_Image != null)
                m_Image.sprite = spr;
        });
    }

    public void OnClick()
    {
        onClick?.Invoke(this, null);
    }
}
