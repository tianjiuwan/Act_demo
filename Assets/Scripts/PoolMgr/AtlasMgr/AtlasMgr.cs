using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AtlasMgr : Singleton<AtlasMgr>
{
    private string infoPath = "assetbundle/cfgs/atlasinfo";
    private Dictionary<string, string> atlasMap = null;

    protected override void initialize()
    {
        atlasMap = new Dictionary<string, string>();
        readAtlasInfo();
    }

    public void Init() {

    }

    //读取图集配置 key = spName   value = abName
    private void readAtlasInfo()
    {
        string path = Path.Combine(Define.abPre, infoPath);
        AssetBundle ab = AssetBundle.LoadFromFile(path);
        string[] names = ab.GetAllAssetNames();
        AtlasInfo info = ab.LoadAsset(names[0]) as AtlasInfo;
        for (int i = 0; i < info.keys.Count; i++)
        {
            atlasMap.Add(info.keys[i], info.abs[i]);
        }
    }

    private bool hasBundleKey(string name)
    {
        name = name.ToLower();
        return atlasMap.ContainsKey(name);
    }

    private string getBundleName(string name)
    {
        name = name.ToLower();
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

