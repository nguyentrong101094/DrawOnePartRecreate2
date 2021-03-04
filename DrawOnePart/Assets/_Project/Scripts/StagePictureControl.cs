using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StagePictureControl : MonoBehaviour
{
    [SerializeField] Image pictureDrawing;
    public Canvas pictureUICanvas;
    string stageID;
    AddressableImage m_AddressableImage;
    Sprite beforeSprite;
    Sprite afterSprite;

    private void Awake()
    {
        m_AddressableImage = GetComponent<AddressableImage>();
    }

    public void Setup(string id, bool moveUpABit = false)
    {
        stageID = id;
        //StartCoroutine(AddressableImage.CoCheckAtlasExist(stageID, ContinueSetup));
        //bool atlasExist = await AddressableImage.CheckExist(stageID);
        bool atlasExist = false;
        ContinueSetup(this, atlasExist);
        if (!moveUpABit)
        {
            //gestureLimitRectBounds.anchoredPosition = gestureLimitRectBounds.anchoredPosition + new Vector2(0f, Const.ADDITION_PLAY_FIELD_Y);
            transform.position += new Vector3(0f, Const.ADDITION_PLAY_FIELD_Y_WORLD);
        }
    }

    void ContinueSetup(object sender, bool atlasExist)
    {
        //pictureDrawing.sprite = GetStagePicture(stageID);
        /*StartCoroutine(SetupImageFromAddressable(stageID, !atlasExist,
            (Sprite spr) =>
            {
                pictureDrawing.sprite = spr;
                pictureDrawing.SetNativeSize();
            }));
        StartCoroutine(SetupImageFromAddressable($"{stageID}{Const.PICTURE_SOLUTION_SUFFIX}", !atlasExist,
            (Sprite spr) =>
            {
                afterSprite = spr;
            }));*/
        SetupImageFromAddressable(stageID, false, (Sprite spr) =>
        {
            pictureDrawing.sprite = spr;
            pictureDrawing.SetNativeSize();

            SetupImageFromAddressable(GetPictureSolutionName(stageID), false, (Sprite sprSol) =>
            {
                afterSprite = sprSol;
            });
        });
    }

    public static string GetPictureSolutionName(string name) => $"{name}{Const.PICTURE_SOLUTION_SUFFIX}";

    /*public IEnumerator SetupImageFromAddressable(string id, bool useCommonAtlas, AddressableImage.LoadSpriteDelegate onComplete)
    {
        CoroutineWithData cd = new CoroutineWithData(this, m_AddressableImage.GetSpriteFromAtlas(id));
        yield return cd.coroutine;
        Debug.Log("result is " + cd.result);
        Sprite sprite = cd.result as Sprite;
        onComplete?.Invoke(sprite);
    }*/

    public void SetupImageFromAddressable(string id, bool useCommonAtlas, StageResLoader.LoadSpriteDelegate onComplete)
    {
        /*Sprite sprite = await AddressableImage.GetSpriteFromAtlasAsync(id);
        onComplete?.Invoke(sprite);*/

        StageResLoader.Instance.GetSpriteFromAtlasAsync(id, onComplete);
    }

    public async void OnCorrect()
    {
        int timeout = 9999;
        while (afterSprite == null && timeout > 0) //if afterSprite hasn't been loaded, wait 99s 
        {
            timeout--;
            await Task.Delay(10);
        }
        pictureDrawing.sprite = afterSprite;
        pictureDrawing.SetNativeSize();
    }

    /*public static Sprite GetStagePicture(string id)
    {
        Sprite spr = Resources.Load<Sprite>($"{Const.PICTURE_FOLDER_NAME}/{id}");
        if (spr == null)
        {
            Debug.LogError($"Picture file not found {id}");
            //throw new System.Exception($"Picture file not found {id}");
        }
        return spr;
    }*/
}
