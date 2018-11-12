using System;

/// <summary>
/// 池子类型
/// </summary>
public enum E_PoolType
{
    None,
    UICache,//缓存UI
    UISingle,//单例UI
    Model,//模型
    Effect,//特效
    Atlas,//图集
}

/// <summary>
/// 池子模式 
/// </summary>
public enum E_PoolMode
{
    None,
    Time,//时间池
    Level,//关卡池
    Overall,//全局池
}


public enum E_LoadStatus
{
    Wait,
    Loading,
    Finish,
}

public class Define
{
    /// <summary>
    /// 编辑器下面资源的前缀
    /// </summary>
    public const string editorPre = "Assets/Res/";
    /// <summary>
    /// ab资源前缀
    /// </summary>
    public const string abPre = "Assets/Res/AssetBundleExport/";
    public const string atlasBundleName = "AssetBundle/cfgs/atlasCfg";


    public const int checkAssetBundleCacheSec = 10;
    public const int checkBasePoolSec = 60;

    //定义meunItem index
    public int buildAsssetBundle = 1001;
    public int clearAssetBundle = 1002;
    public int clearAssetBundleName = 5001;
    public int exportAtlasCfg = 6001;
}

