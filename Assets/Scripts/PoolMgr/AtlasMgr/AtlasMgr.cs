using System;
using System.Collections.Generic;
using UnityEngine;

public class AtlasMgr : Singleton<AtlasMgr>
{
    private Dictionary<string, string> atlasMap = null;

    protected override void initialize()
    {
        atlasMap = new Dictionary<string, string>();
        atlasMap.Add("chosenKuang".ToLower(), "AssetBundle/Atlas/CommonAtlas".ToLower());
    }

    private bool hasBundleKey(string name)
    {
        return atlasMap.ContainsKey(name);
    }

    private string getBundleName(string name)
    {
        return atlasMap[name];
    }

    #region 接口
    public static bool hasKey(string name) {
        return Instance.hasBundleKey(name);
    }
    public static string getName(string name)
    {
        return Instance.getBundleName(name);
    }
    #endregion

}

