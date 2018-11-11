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
}

