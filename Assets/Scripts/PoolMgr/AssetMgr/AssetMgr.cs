using System;
using System.Collections.Generic;

public class AssetMgr : Singleton<AssetMgr>
{
    private Dictionary<string, PackAsset> bundles = null;

    private bool hasAsset(string name)
    {
        return true;
    }

    private PackAsset getAsset(string name)
    {
        return bundles[name];
    }

    private  void addRef(string name)
    {
        bundles[name].addRef();
    }
    private  void subRef(string name)
    {
        bundles[name].subRef();
    }

    protected override void initialize()
    {
        bundles = new Dictionary<string, PackAsset>();
    }

    public void onDispose()
    {

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
    #endregion
}

