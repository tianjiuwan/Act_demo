using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadThread : MonoBehaviour
{
    private static LoadThread _instance;
    public static LoadThread Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("LoadThread");
                _instance = go.AddComponent<LoadThread>();
            }
            return _instance;
        }
    }

}

