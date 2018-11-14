using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolObj : MonoBehaviour
{
    public string resName;
    private List<string> dyDeps;

    /// <summary>
    /// 添加一个动态引用
    /// </summary>
    /// <param name="resName"></param>
    public void addDeps(string resName)
    {
        if (dyDeps == null)
            dyDeps = new List<string>();
        if (!dyDeps.Contains(resName))
            dyDeps.Add(resName);
    }

    /// <summary>
    /// 获取所有动态引用
    /// </summary>
    /// <returns></returns>
    public List<string> getDeps()
    {
        return dyDeps;
    }
    public int getDepsNum() {
        if (dyDeps != null) {
            return dyDeps.Count;
        }
        return 0;
    }

    private void OnDestroy()
    {
        resName = null;
        if (dyDeps != null)
        {
            dyDeps.Clear();
            dyDeps = null;
        }
    }
}

