using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;
using System.Linq;
using TMPro;
using UnityEngine.UI.Extensions;

public class StageSelectController : Controller
{
    public const string STAGESELECT_SCENE_NAME = "StageSelect";

    public override string SceneName()
    {
        return STAGESELECT_SCENE_NAME;
    }

    [SerializeField] StageSelectItem thumbnailPrefab;
    [SerializeField] Transform thumbnailGroup;
    [SerializeField] ScrollRectEnsureVisible scrollRectMover;
    [SerializeField] TMP_Text continueBtnText;
    [SerializeField] GameObject itemPagePrefab;
    [SerializeField] HorizontalScrollSnap pageScrollSnap;
    [SerializeField] Transform pageParent;
    [SerializeField] TMP_Text textPageNumber;
    const int itemsPerPage = 12;
    int pageCount;
    List<StageSelectItem> stageSelectItems = new List<StageSelectItem>();

    private void Start()
    {
        var service = GameDatabase.Service;
        List<StageData> stageDatas = service.GetAllStageData().ToList();

        /*for (int i = 0; i < stageDatas.Count; i++)
        {
            var thumbnail = Instantiate(thumbnailPrefab, thumbnailGroup);
            stageSelectItems.Add(thumbnail);
            thumbnail.Setup(stageDatas[i].id);
            thumbnail.onClick += OnThumbnailClick;
        }
        StartCoroutine(CoCenterOnCurrentStage());*/

        //Make pages
        pageCount = Mathf.CeilToInt((float)Const.MAX_STAGE / itemsPerPage);
        int currentStage = 1;
        for (int i_page = 0; i_page < pageCount; i_page++)
        {
            Transform newPage = Instantiate(itemPagePrefab, pageParent.transform).transform;
            StageSelectItem[] itemsInPage = newPage.GetComponentsInChildren<StageSelectItem>(includeInactive: true);
            for (int i_stageItem = 0; i_stageItem < itemsInPage.Length; i_stageItem++)
            {
                if (currentStage > Const.MAX_STAGE)
                {
                    itemsInPage[i_stageItem].gameObject.SetActive(false);
                }
                else
                {
                    itemsInPage[i_stageItem].Setup(currentStage);
                    itemsInPage[i_stageItem].onClick += OnThumbnailClick;
                }
                currentStage++;
            }
            //pageScrollSnap.AddChild(newPage.gameObject);
        }
        int currentPage = Mathf.CeilToInt((float)User.GetCurrentStage() / itemsPerPage) - 1;
        pageScrollSnap.StartingScreen = currentPage;
        pageScrollSnap.gameObject.SetActive(true);
        //StartCoroutine(CoCenterOnCurrentStage());
        pageScrollSnap.OnSelectionPageChangedEvent.AddListener(OnPageChange);
        OnPageChange(currentPage);
        continueBtnText.text = $"Continue\nLevel {User.GetContinueStage()}";
        //for (int i = 0; i < stageDatas.Count; i++)
        //{
        //    await thumbnailItems[i].SetImage();
        //}
        AdsManager.Instance.ShowBanner();
    }

    IEnumerator CoCenterOnCurrentStage()
    {
        yield return new WaitForEndOfFrame();

        /*int previousStagePlayedArrayID = User.GetCurrentStage();
        previousStagePlayedArrayID = Mathf.Min(previousStagePlayedArrayID, stageSelectItems.Count - 1);
        if (previousStagePlayedArrayID < stageSelectItems.Count && previousStagePlayedArrayID >= 0)
        {
            scrollRectMover.CenterOnItem(stageSelectItems[previousStagePlayedArrayID].GetComponent<RectTransform>());
        }*/

        int currentPage = Mathf.CeilToInt((float)User.GetCurrentStage() / itemsPerPage) - 1;
        pageScrollSnap.ChangePage(currentPage);
    }

    void OnThumbnailClick(object sender, int id)
    {
        //.Log($"Firebase require internet: {FirebaseRemoteConfigHelper.GetBool(Const.RMCF_REQUIRE_INTERNET, true)}");
        /*if (FirebaseRemoteConfigHelper.GetBool(Const.RMCF_REQUIRE_INTERNET, true) && Application.internetReachability == NetworkReachability.NotReachable)
        {
            Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.OK, Const.MSG_NO_INTERNET));
        }
        else*/
            LoadStage(id);
    }

    public void OnSettingBtn()
    {
        Manager.Add(SettingController.SETTING_SCENE_NAME);
    }

    public void OnContinueBtn()
    {
        LoadStage(User.GetContinueStage());
    }

    void LoadStage(int id)
    {
        string nextPictureName = GameDatabase.Service.GetStageData(id).picture_name;
        //_ = AddressableImage.GetSpriteFromAtlasAsync(nextPictureName);
        StageResLoader.Instance.GetSpriteFromAtlasAsync(nextPictureName, null);
        Manager.Load(GameDrawController.GAMEDRAW_SCENE_NAME, new GameSceneData(id));
    }

    void OnPageChange(int currentPage)
    {
        //textPageNumber.text = $"Page {currentPage + 1}/{pageCount}";
    }

    public override void OnKeyBack()
    {
        Manager.Load(GameDrawController.GAMEDRAW_SCENE_NAME, new GameSceneData(User.GetCurrentStage()));
    }
}