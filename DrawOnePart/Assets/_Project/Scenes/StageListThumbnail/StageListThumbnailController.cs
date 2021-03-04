using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;
using System.Linq;

public class StageListThumbnailController : Controller
{
    public const string STAGELISTTHUMBNAIL_SCENE_NAME = "StageListThumbnail";

    public override string SceneName()
    {
        return STAGELISTTHUMBNAIL_SCENE_NAME;
    }

    [SerializeField] StageThumbnailItem thumbnailPrefab;
    [SerializeField] Transform thumbnailGroup;

    private async void Start()
    {
        var service = GameDatabase.Service;
        List<StageData> stageDatas = service.GetAllStageData().ToList();
        List<StageThumbnailItem> thumbnailItems = new List<StageThumbnailItem>();
        for (int i = 0; i < stageDatas.Count; i++)
        {
            var thumbnail = Instantiate(thumbnailPrefab, thumbnailGroup);
            thumbnailItems.Add(thumbnail);
            thumbnail.Setup(stageDatas[i]);
            thumbnail.onClick += OnThumbnailClick;
        }

        for (int i = 0; i < stageDatas.Count; i++)
        {
            await thumbnailItems[i].SetImage();
        }
    }

    void OnThumbnailClick(object sender, System.EventArgs args)
    {
        StageThumbnailItem item = sender as StageThumbnailItem;
        Manager.Load(LevelMakerController.LEVELMAKER_SCENE_NAME, item.stageData.picture_name);
    }
}