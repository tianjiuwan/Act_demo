using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManifestMgr : Singleton<ManifestMgr>
{
    private string manifestPath = Define.abPre + "AssetBundleExport";
    AssetBundleManifest assetBundleManifest = null;//初始化获取依赖 todo

    protected override void initialize()
    {
        loadManifest();
    }

    //初始化ab依赖
    private void init()
    {

    }
    void loadManifest()
    {
        //LoadFromFile不可以有.assetbundle后缀
        AssetBundle bundle = AssetBundle.LoadFromFile(manifestPath);
        assetBundleManifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        // 压缩包释放掉
        bundle.Unload(false);
        bundle = null;
    }

    private void getAllDeps(string name, ref List<string> lst)
    {
        if (Instance.assetBundleManifest == null) return;
        name = name.ToLower();
        if (Instance.assetBundleManifest != null)
        {
            string[] deps = Instance.assetBundleManifest.GetAllDependencies(name);
            lst.AddRange(deps);
        }
    }

    private void dispose()
    {

    }

    #region 接口
    public static void Init() {
        Instance.init();
    }

    public static void getDeps(string name, ref List<string> lst)
    {
        Instance.getAllDeps(name, ref lst);
    }
    #endregion

}

