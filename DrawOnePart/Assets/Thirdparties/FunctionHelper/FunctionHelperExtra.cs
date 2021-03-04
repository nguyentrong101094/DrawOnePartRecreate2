//using DigitalRuby.Pooling;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Extra function helper, using thirdparties' function
public static partial class FunctionHelper
{
    public static Dictionary<TKey, TValue> Shuffle<TKey, TValue>(
       this Dictionary<TKey, TValue> source)
    {
        System.Random r = new System.Random();
        return source.OrderBy(x => r.Next())
           .ToDictionary(item => item.Key, item => item.Value);
    }

    /// <summary>
    /// Short for Localizes.GetString(str)
    /// </summary>
    //public static string LC(this string str)
    //{
    //    return Localizes.GetString(str);
    //}
/*
    public static void AddPrefabCheck(string key, GameObject gO)
    {
        if (!SpawningPool.ContainsPrefab(key))
        {
            SpawningPool.AddPrefab(key, gO);
        }
    }

    public static void ReturnToCacheOrDeactive(GameObject gO)
    {
        if (!SpawningPool.ReturnToCache(gO))
        {
            gO.SetActive(false);
        }
    }*/
}
