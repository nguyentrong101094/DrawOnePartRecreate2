using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;
using TMPro;
using PaintIn3D;
using UnityEngine.UI;

public class LevelMakerController : Controller
{
    public const string LEVELMAKER_SCENE_NAME = "LevelMaker";

    public override string SceneName()
    {
        return LEVELMAKER_SCENE_NAME;
    }

    [SerializeField] TMP_InputField levelNameInput;
    [SerializeField] StagePictureControl stagePictureControl;
    [SerializeField] StagePictureControl stageAnswerPictureControl;
    [SerializeField] PaintHitScreen paintHitScreen;
    [SerializeField] GestureRecorder gestureRecorder;
    [SerializeField] DrawDotRecorder drawDotRecorder; //v4
    [SerializeField] DrawBoundMaker drawBoundMaker;

    [SerializeField] Button[] btnEnableAfterOpen; //only enable these buttons when a file is opened
    [SerializeField] ToggleActiveObject toggleDrawBtn;
    [SerializeField] Button toggleSetBoundBtn;
    [SerializeField] Button addGestureBtn;
    [SerializeField] TMP_Text setBoundBtnText;
    [SerializeField] Toggle toggleBoxUseV4;
    [SerializeField] TMP_InputField boundSizeInput; //v4
    [SerializeField] TMP_InputField scoreRequiredInput; //v4
    bool isSettingBound;
    string pictureName;
    PictureData pictureData;

    public static LevelMakerController instance;

    private void Awake()
    {
        instance = this;
        levelNameInput.onEndEdit.AddListener(OnStageNameInputSubmit);
        boundSizeInput.onEndEdit.AddListener(OnBoundSizeSubmit);
        scoreRequiredInput.onEndEdit.AddListener(OnScoreTargetSubmit);
    }

    public override void OnActive(object data)
    {
        base.OnActive(data);
        string id = "";
        if (data != null)
        {
            id = data as string;
            LoadLevel(id);
        }
        paintHitScreen.onFingerHitBegin += OnFingerDown;
    }

    public override void OnShown()
    {
        base.OnShown();
        AdsManager.Instance.HideBanner();
    }

    void OnFingerDown(object sender, Vector3 pos) { paintHitScreen.ClearAll(sender, null); }

    public void OpenLevel()
    {
        LoadLevel(levelNameInput.text);
    }

    /*void LoadLevelOld(string id)
    {
        pictureName = id;
        //Check if picture file exist
        //Sprite sprite = StagePictureControl.GetStagePicture(id);
        Sprite sprite = null;
        StartCoroutine(stagePictureControl.SetupImageFromAddressable(id, false, (Sprite spr) =>
        {
            sprite = spr;
            if (sprite == null)
            {
                Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.OK, "Picture file not found. Please check name and file"));
                return;
            }
            ContinueLoadLevel();
        }));
    }*/

    async void LoadLevel(string id)
    {
        pictureName = id.ToUpper();
        //Check if picture file exist
        //Sprite sprite = StagePictureControl.GetStagePicture(id);
        //bool atlasExist = await AddressableImage.CheckExist(id);
        /*bool atlasExist = false;
        Sprite sprite = await AddressableImage.GetSpriteFromAtlasAsync(pictureName);
        if (sprite == null)
        {
            Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.OK, "Picture file not found. Please check name and file"));
            return;
        }*/

        Sprite sprite;
        StageResLoader.Instance.GetSpriteFromAtlasAsync(pictureName, (spr) =>
        {
            sprite = spr;
            if (sprite == null)
            {
                Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.OK, "Picture file not found. Please check name and file"));
                return;
            }
            ContinueLoadLevel();
        });

        /*StartCoroutine(stagePictureControl.SetupImageFromAddressable(id, atlasExist, (Sprite spr) =>
        {
            sprite = spr;
            if (sprite == null)
            {
                Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.OK, "Picture file not found. Please check name and file"));
                return;
            }
            ContinueLoadLevel();
        }));*/

    }

    void ContinueLoadLevel()
    {

#if SQLITE4UNITY
        pictureData = GameDatabase.Service.GetPictureData(pictureName);
#else
        PictureData pictureData = DatabaseSimpleSQL.Instance.GetPictureData(id);
#endif
        //Check PictureData
        if (pictureData == null)
        {
            //if no pictureData exist, ask if user want to add new picture data
            Debug.LogError($"PictureData not found {pictureName}");
            Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.YES_NO, "No PictureData's name match. Do you want to insert new PictureData?",
                () =>
                {
#if SQLITE4UNITY
                    GameDatabase.Service.AddPictureData(pictureName);
                    LoadLevel(pictureName);
#else
                    throw new System.NotImplementedException();
#endif
                }
                ));
        }
        else
        {
            stagePictureControl.Setup(pictureName, true);
            stageAnswerPictureControl.Setup(pictureName, true);
            stageAnswerPictureControl.OnCorrect();
            levelNameInput.text = pictureName;
            gestureRecorder.Setup(pictureName);
            CheckSetupDrawDotV4();
            drawBoundMaker.Setup(pictureData);
            foreach (var item in btnEnableAfterOpen)
            {
                item.interactable = true;
            }
            toggleDrawBtn.SetInteractable(true);
            toggleBoxUseV4.isOn = pictureData.use_v4;
            toggleBoxUseV4.onValueChanged.AddListener(ToggleUseV4);
            boundSizeInput.text = pictureData.bound_width.ToString();
            if (pictureData.score_required > 0f)
                scoreRequiredInput.text = pictureData.score_required.ToString();
        }
    }

    void CheckSetupDrawDotV4()
    {
        if (pictureData.use_v4)
        {
            drawDotRecorder.Setup(pictureData.name);
        }
    }

    public void ToggleSetBound()
    {
        isSettingBound = !isSettingBound;
        if (isSettingBound)
        {
            setBoundBtnText.text = "<color=red> DONE set bound </color>";
            paintHitScreen.enabled = false;
            gestureRecorder.isEnabled = false;
            drawBoundMaker.isSettingBound = true;
            toggleDrawBtn.SetInteractable(false);
        }
        else
        {
            //done set bound
            setBoundBtnText.text = "Set Bound";
            paintHitScreen.enabled = true;
            gestureRecorder.isEnabled = true;
            drawBoundMaker.isSettingBound = false;
            toggleDrawBtn.SetInteractable(true);
            drawBoundMaker.SaveBound();
        }
    }

    public void ToggleDraw(bool active)
    {
        addGestureBtn.interactable = paintHitScreen.enabled = gestureRecorder.isEnabled = active;
        if (active)
        {
            toggleSetBoundBtn.interactable = false;
        }
        else
        {
            toggleSetBoundBtn.interactable = true;
        }
    }

    public void ToggleUseV4(bool value)
    {
        pictureData.use_v4 = value;
        GameDatabase.Service.UpdateData(pictureData);
        CheckSetupDrawDotV4();
    }

    public void ToggleDrawV4(bool active)
    {
        paintHitScreen.enabled = drawDotRecorder.isEnabled = active;
    }

    public void PlayStage()
    {
        Manager.Load(GameDrawController.GAMEDRAW_SCENE_NAME, new GameSceneData(levelNameInput.text));
    }

    void OnStageNameInputSubmit(string value)
    {
        OpenLevel();
        levelNameInput.DeactivateInputField(true);
    }

    void OnBoundSizeSubmit(string value)
    {
        pictureData.bound_width = float.Parse(value);
        GameDatabase.Service.UpdateData(pictureData);
        Debug.Log($"Picture bound_width updated {pictureData.bound_width}");
        if (pictureData.use_v4 && pictureData.bound_width > 100f)
        {
            Debug.LogError($"{pictureData.name} bound size > 50 too big. Check your input");
        }
    }

    void OnScoreTargetSubmit(string value)
    {
        pictureData.score_required = float.Parse(value);
        GameDatabase.Service.UpdateData(pictureData);
        Debug.Log($"Picture score_required updated {pictureData.ScoreRequired}");
    }

    public void ResetGesture()
    {
        Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.YES_NO, "Delete this picture's recorded gesture to start over?", MakeNewGestureFile));
    }

    void MakeNewGestureFile()
    {
        if (!pictureData.use_v4)
            gestureRecorder.MakeNewGestureFile();
        else
            drawDotRecorder.MakeNewGestureFile();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Adding Gesture");
            gestureRecorder.AddGesture();
            paintHitScreen.ClearAll(this, null);
        }
    }

    public override void OnKeyBack()
    {
        GameDrawController.DebugStageList();
    }
}