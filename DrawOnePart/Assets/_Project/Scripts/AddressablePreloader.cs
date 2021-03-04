using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablePreloader
{
    public class CachedOperation
    {
        private object obj;
        public AsyncOperationHandle operation;

        public object Result { get => operation.Result; }

        public CachedOperation(AsyncOperationHandle operation)
        {
            this.operation = operation;
            this.obj = operation.Result;
        }
    }
    static Dictionary<string, CachedOperation> cacheObjects = new Dictionary<string, CachedOperation>();

    public static async void Preload<T>(string address)
    {
        if (!cacheObjects.ContainsKey(address))
        {
            await Load<T>(address);
        }
    }

    public static async Task<CachedOperation> Load<T>(string address)
    {
        if (!cacheObjects.ContainsKey(address))
        {
            AsyncOperationHandle<T> asyncOperationHandle = Addressables.LoadAssetAsync<T>(address);
            //.Log($"Adding to cache dict {address}");
            var cached = new CachedOperation(asyncOperationHandle);
            cacheObjects.Add(address, cached);

            await asyncOperationHandle.Task;
            //ReleaseAsyncOperation(asyncOperationHandle);
            if (asyncOperationHandle.Result != null)
            {
                return cached;
            }
            else
            {
                Unload(address);
                return default;
            }
        }
        else
        {
            //var dict = cacheObjects;
            while (!cacheObjects[address].operation.IsDone)
            {
                //await cacheObjects[address].operation.Task; //Task become null, Unity is dumb
                await Task.Delay(50);
            }
            return cacheObjects[address];
        }
    }

    public static void Unload(string address)
    {
        if (cacheObjects.ContainsKey(address))
        {
            //.Log($"unloading address {address}");
            Addressables.Release(cacheObjects[address].operation);
            cacheObjects[address] = null;
            cacheObjects.Remove(address);
        }
    }

    static async void ReleaseAsyncOperation<T>(AsyncOperationHandle<T> operation, float delay = 1f)
    {
        //break on mobile
        await Task.Delay((int)(delay * 1000));
        Addressables.Release(operation);
    }
}
