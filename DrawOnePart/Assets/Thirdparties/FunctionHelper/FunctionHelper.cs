using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DigitalRuby.Pooling;

public static partial class FunctionHelper
{
    #region CODE

    public static string HighlightText(string str, string color = "yellow")
    {
        return (str.Replace("[", string.Format("<color={0}>", color)).Replace("]", "</color>"));
    }

    public static void SetRootThenDontDestroy(this GameObject gO)
    {
        gO.transform.SetParent(null);
        GameObject.DontDestroyOnLoad(gO);
    }

    public static Color SetAlpha(this Color col, float alpha)
    {
        col.a = alpha;
        return col;
    }

    public static Color SetColorKeepAlpha(this Color col, Color newCol)
    {
        return new Color(newCol.r, newCol.g, newCol.b, col.a);
    }

    /// <summary>
    /// Short for Localizes.GetString(str)
    /// </summary>
    /*public static string LC(this string str)
    {
        return Localizes.GetString(str);
    }*/

    #region Array
    public static T GetRandom<T>(this List<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static T GetDict<T1, T>(this Dictionary<T1, T> dict, T1 index, bool showError = true)
    {
        if (dict.ContainsKey(index))
            return dict[index];
        else
        {
            Debug.LogError("Error: dictionary does not contain index " + index);
            return default(T);
        }
    }

    /// <summary>
    /// Get element at index, if list doesn't have that index then return the last element
    /// </summary>
    public static T GetIndexOrHighest<T>(this List<T> list, int index, bool showError = true, T p_Default = default(T))
    {
        if (index >= 0 && index < list.Count)
        {
            return list[index];
        }
        else if (list.Count >= 1)
        {
            if (showError)
                Debug.LogError(string.Format("Error: array out of bound, index {0}", index));
            if (index < 0) return list[0];
            else return list[list.Count - 1];
        }
        else
        {
            if (showError)
                Debug.LogError(string.Format("Error: array out of bound, index {0}", index));
            return p_Default;
        }
    }

    public static T GetIndexOrHighest<T>(this T[] list, int index, bool showError = true, T p_Default = default(T))
    {
        if (index >= 0 && index < list.Length)
        {
            return list[index];
        }
        else if (list.Length >= 1)
        {
            if (showError)
                Debug.LogError(string.Format("Error: array out of bound, index {0}", index));
            if (index < 0) return list[0];
            else return list[list.Length - 1];
        }
        else
        {
            if (showError)
                Debug.LogError(string.Format("Error: array out of bound, index {0}", index));
            return p_Default;
        }
    }

    public static T GetArrayStartAtOne<T>(this List<T> list, int id)
    {
        return list.GetIndexOrHighest(id - 1);
    }

    public static void Shuffle<T>(this IList<T> list, System.Random rnd = null)
    {
        if (rnd == null)
        {
            rnd = new System.Random((int)(UnityEngine.Random.value * 1000000));
        }
        for (var i = 0; i < list.Count - 1; i++)
        {
            int next = rnd.Next(i, list.Count);
            list.Swap(i, next);
        }
    }

    public static void Swap<T>(this IList<T> list, int i, int j)
    {
        var temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }
    #endregion

    #endregion

    #region GAME

    public static Vector3 SetX(this Vector3 vector, float x)
    {
        return new Vector3(x, vector.y);
    }

    public static Vector3 SetY(this Vector3 vector, float y)
    {
        return new Vector3(vector.x, y, vector.z);
    }
    public static Vector3 SetZ(this Vector3 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }
    /// <summary>
    /// convert vector2 to vector3 in X Z
    /// </summary>
    public static Vector3 YToZ(this Vector2 vect)
    {
        return new Vector3(vect.x, 0f, vect.y);
    }

    public static float GetAngle360(Vector3 from, Vector3 to)
    {
        Vector3 right = Vector3.right;
        float angle = Vector3.Angle(from, to);
        return (Vector3.Angle(right, to) > 90f) ? 360f - angle : angle;
    }
    #endregion

    #region DEBUG
    public static void DebugLog<T>(this List<T> list)
    {
        Debug.Log("Begin List");
        foreach (var item in list)
        {
            Debug.Log(item);
        }
    }
    #endregion
}
