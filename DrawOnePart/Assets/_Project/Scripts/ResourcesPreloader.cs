using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesPreloader : MonoBehaviour
{
    static DummyCoroutineRunner m_CoroutineRunner;

    public class CachedOperation
    {
        private object obj;
        public ResourceRequest operation;

        public object Result { get => operation.asset; }

        public CachedOperation(ResourceRequest operation)
        {
            this.operation = operation;
            this.obj = operation.asset;
        }
    }
    static Dictionary<string, CachedOperation> cacheObjects = new Dictionary<string, CachedOperation>();

    public static void Load<T>(string address, System.EventHandler<object> onComplete)
    {
        DummyCoroutineRunner.Instance.StartCoroutine(CoLoad<T>(address, onComplete));
    }

    public static IEnumerator CoLoad<T>(string address, System.EventHandler<object> onComplete)
    {
        if (!cacheObjects.ContainsKey(address))
        {
            ResourceRequest asyncOperationHandle = Resources.LoadAsync(address);
            //.Log($"Adding to cache dict {address}");
            var cached = new CachedOperation(asyncOperationHandle);
            cacheObjects.Add(address, cached);

            yield return asyncOperationHandle;
            //ReleaseAsyncOperation(asyncOperationHandle);
            if (asyncOperationHandle.asset != null)
            {
                onComplete?.Invoke(null, cached.Result);
            }
            else
            {
                Unload(address);
                onComplete?.Invoke(null, default(T));
            }
        }
        else
        {
            //var dict = cacheObjects;
            yield return cacheObjects[address].operation;
            onComplete?.Invoke(null, cacheObjects[address].Result);
        }
    }

    public static void Unload(string address)
    {
        if (cacheObjects.ContainsKey(address))
        {
            //.Log($"unloading address {address}");
            cacheObjects[address] = null;
            cacheObjects.Remove(address);
        }
    }
}
