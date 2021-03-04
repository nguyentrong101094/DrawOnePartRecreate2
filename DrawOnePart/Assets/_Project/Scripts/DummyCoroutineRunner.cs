using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCoroutineRunner : MonoBehaviour
{
    static DummyCoroutineRunner instance;
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

    public static DummyCoroutineRunner Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject tempGO = new GameObject("DummyCoroutineRunner");
                instance = tempGO.AddComponent<DummyCoroutineRunner>();
            }
            return instance;
        }
    }
}
