﻿using System;
using System.Collections.Generic;

public class AssetMgr : Singleton<AssetMgr>
{
    private Dictionary<string, PackAsset> bundles = null;

    protected override void initialize()
    {
        bundles = new Dictionary<string, PackAsset>();
    }

    //提供编辑器查看
    public Dictionary<string, PackAsset> getAll()
    {
        return bundles;
    }

    //是否有ab了
    private bool hasAsset(string name)
    {
        return bundles.ContainsKey(name);
    }

    private void addAsset(PackAsset ab)
    {
        bundles.Add(ab.Name, ab);
    }

    private PackAsset getAsset(string name)
    {
        return bundles[name];
    }

    private void addRefCount(string name)
    {
        bundles[name].addRef();
    }
    private void subRefCount(string name, int count)
    {
        bundles[name].subRef(count);
    }

    public void onDispose()
    {
        disposeAll();
    }

    private void disposeAll(bool forceUnload = false)
    {
        foreach (var item in bundles)
        {
            item.Value.unload(forceUnload);
        }
        bundles.Clear();
    }

    #region 提供接口
    public static bool has(string name)
    {
        return Instance.hasAsset(name);
    }

    public static PackAsset get(string name)
    {
        return Instance.getAsset(name);
    }

    public static void add(PackAsset ab)
    {
        Instance.addAsset(ab);
    }

    public static void addRef(string name)
    {
        Instance.bundles[name].addRef();
    }
    public static void subRef(string name, int count = 1)
    {
        Instance.bundles[name].subRef(count);
    }

    public static void dispose(bool forceUnload = false)
    {
        Instance.disposeAll(forceUnload);
    }
    #endregion
}

