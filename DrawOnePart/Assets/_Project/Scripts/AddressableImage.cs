using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CoroutineWithData
{
    public Coroutine coroutine { get; private set; }
    public object result;
    private IEnumerator target;
    public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
    {
        this.target = target;
        this.coroutine = owner.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (target.MoveNext())
        {
            result = target.Current;
            yield return result;
        }
        Debug.Log(result);
    }
}

public class AddressableImage : MonoBehaviour
{
    public delegate void LoadSpriteDelegate(Sprite loadedSprite);

    UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<SpriteAtlas> asyncOperationHandle;
    Queue<AsyncOperationHandle<SpriteAtlas>> asyncOperationHandleQueues = new Queue<AsyncOperationHandle<SpriteAtlas>>();
    UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Sprite> asyncOperationSpriteHandle;
    const string spriteAtlasCommonAddress = "Assets/_Project/Addressables/{0}.spriteatlas";

    IEnumerator GetSprite()
    {
        asyncOperationSpriteHandle = Addressables.LoadAssetAsync<Sprite>("Picture_Atlas_1[ANIMAL_01]");
        //asyncOperationSpriteHandle = Addressables.LoadAssetAsync<Sprite>("Assets/_Project/Addressables/PictureTest.spriteatlas[ANIMAL_01]");

        yield return asyncOperationHandle;
        Sprite sprite = asyncOperationSpriteHandle.Result;
    }

    public IEnumerator GetSpriteFromAtlas(string name)
    {
        CoroutineWithData checkAtlasExist = new CoroutineWithData(this, CoCheckAtlasExist(name, null));
        yield return checkAtlasExist.coroutine;
        Debug.Log("result is " + checkAtlasExist.result);
        bool atlasExist = (bool)checkAtlasExist.result;
        string atlasAddress;
        if (atlasExist)
        {
            atlasAddress = GetAtlasAddressFromPictureName(name);
        }
        else atlasAddress = GetAtlasAddressFromPictureName("OTHER");

        var asyncOperationHandle = Addressables.LoadAssetAsync<SpriteAtlas>(atlasAddress);
        asyncOperationHandleQueues.Enqueue(asyncOperationHandle);
        yield return asyncOperationHandle;
        if (asyncOperationHandle.Result != null)
            yield return asyncOperationHandle.Result.GetSprite(name);
    }

    [System.Obsolete("Use StageResLoader.GetSpriteFromAtlasAsync() instead")]
    public static async Task<Sprite> GetSpriteFromAtlasAsync(string name)
    {
        FirebaseManager.CheckWaitForReady((object sender, bool success) =>
        {
            if (success) FirebaseManager.LogCrashlytics($"{Const.CRASHLYTIC_LOG_PICTURE}_{name}");
        });
        //check if atlas exist
        string atlasAddress = GetAtlasAddressFromPictureName(name);
        AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync(atlasAddress);
        await handle.Task;
        bool atlasExist = handle.Result.Count > 0;
        //.Log($"Atlas Exist: {atlasExist}");
        Addressables.Release(handle);

        //if not, use OTHER atlas
        if (!atlasExist) atlasAddress = GetAtlasAddressFromPictureName("OTHER");

        //return sprite from atlas
        var cachedAddressable = await AddressablePreloader.Load<SpriteAtlas>(atlasAddress);
        SpriteAtlas atlas = cachedAddressable.Result as SpriteAtlas;
        if (atlas != null)
        {
            Sprite spr = atlas.GetSprite(name);
            if (spr == null)
                Debug.LogError($"Sprite [{name}] not found in atlas {atlasAddress}");
            return spr;
        }
        else
        {
            Debug.LogError($"Atlas addressable [{name}] not found {atlasAddress}");
            return null;
        }

        /*var asyncOperationHandle = Addressables.LoadAssetAsync<SpriteAtlas>(atlasAddress);
        await asyncOperationHandle.Task;
        if (asyncOperationHandle.Result != null)
            return asyncOperationHandle.Result.GetSprite(name);
        else return null;*/
    }

    public static string GetAtlasAddressFromPictureName(string pictureName)
    {
        string[] splits = pictureName.Split('_');
        string group = splits[0];
        string atlasAddress = string.Format(spriteAtlasCommonAddress, group);
        return atlasAddress;
    }

    public static async Task<bool> CheckExist(string name)
    {
        string atlasAddress = GetAtlasAddressFromPictureName(name);
        AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync(atlasAddress);
        await handle.Task;
        bool isExist = handle.Result.Count > 0;
        Debug.Log($"Atlas Exist: {isExist}");
        Addressables.Release(handle);
        return isExist;
    }

    public static IEnumerator CoCheckAtlasExist(string name, System.EventHandler<bool> onComplete)
    {
        string atlasAddress = GetAtlasAddressFromPictureName(name);
        AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync(atlasAddress);
        yield return handle;
        bool isExist = handle.Result.Count > 0;
        Debug.Log($"Atlas Exist: {isExist}");
        onComplete?.Invoke(null, isExist);
        Addressables.Release(handle);
        yield return isExist;
    }

    private void OnDestroy()
    {
        while (asyncOperationHandleQueues.Count > 0)
        {
            Addressables.Release(asyncOperationHandleQueues.Dequeue());
        }
    }
}
