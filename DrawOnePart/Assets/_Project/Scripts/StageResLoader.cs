using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

public class StageResLoader : MonoBehaviour
{
    bool coroutineIsRunning = false;
    const string spriteAtlasCommonAddress = "StageAtlas/{0}";
    public delegate void LoadSpriteDelegate(Sprite loadedSprite);

    public static StageResLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Instantiate(Resources.Load<StageResLoader>("StageResLoader"));
            }
            return instance;
        }
    }
    static StageResLoader instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void GetSpriteFromAtlasAsync(string name, LoadSpriteDelegate onLoadComplete)
    {
        StartCoroutine(CoGetSpriteFromAtlasAsync(name, onLoadComplete));
    }

    public IEnumerator CoGetSpriteFromAtlasAsync(string name, LoadSpriteDelegate onLoadComplete)
    {
        FirebaseManager.CheckWaitForReady((object sender, bool success) =>
        {
            if (success) FirebaseManager.LogCrashlytics($"{Const.CRASHLYTIC_LOG_PICTURE}_{name}");
        });
        //check if atlas exist
        string atlasAddress = GetAtlasAddressFromPictureName(name);

        ResourceRequest request = null;
        SpriteAtlas atlas = null;

        if (coroutineIsRunning) { Debug.LogWarning("This asynchronous operation is already being yielded from another coroutine."); }
        while (coroutineIsRunning)
        {
            yield return 0;
        }

        coroutineIsRunning = true;
        yield return StartCoroutine(ResourcesPreloader.CoLoad<SpriteAtlas>(atlasAddress, (sender, cachedOperation) =>
        {
            atlas = cachedOperation as SpriteAtlas;
        }));
        coroutineIsRunning = false;

        //ResourceRequest request = Resources.LoadAsync<SpriteAtlas>(atlasAddress);
        //yield return request;

        if (atlas == null)
        {
            atlasAddress = GetAtlasAddressFromPictureName("OTHER");

            //request = Resources.LoadAsync<SpriteAtlas>(atlasAddress);
            //yield return request;

            yield return StartCoroutine(ResourcesPreloader.CoLoad<SpriteAtlas>(atlasAddress, (sender, cachedOperation) =>
            {
                atlas = cachedOperation as SpriteAtlas;
            }));
        }
        var req = request;
        Sprite spr = atlas.GetSprite(name);
        if (spr == null)
            Debug.LogError($"Sprite [{name}] not found in atlas {atlasAddress}");
        onLoadComplete?.Invoke(spr);
    }

    public static string GetAtlasAddressFromPictureName(string pictureName)
    {
        string[] splits = pictureName.Split('_');
        string group = splits[0];
        string atlasAddress = string.Format(spriteAtlasCommonAddress, group);
        return atlasAddress;
    }
}
