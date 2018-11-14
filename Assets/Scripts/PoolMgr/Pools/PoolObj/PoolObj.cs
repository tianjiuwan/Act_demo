using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolObj : MonoBehaviour
{
    public string resName;
    public List<string> deps;

    private void OnDestroy()
    {
        resName = null;
        deps.Clear();
        deps = null;
    }
}

