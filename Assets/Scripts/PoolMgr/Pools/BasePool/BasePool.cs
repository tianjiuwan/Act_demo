using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 池子基类
/// </summary>
public class BasePool : IDispose
{
    private string poolName;
    private string resName;
    private E_PoolMode pMode = E_PoolMode.None;
    private E_PoolType pType = E_PoolType.None;
    private double lastUseTime = -1;
    private List<GameObject> cacheLst = null;
    private List<Action<GameObject>> handler = null;

    public void getObj(Action<GameObject> callBack)
    {
        if (cacheLst.Count > 0)
        {
            //缓存池有
            GameObject obj = cacheLst[0];
            cacheLst.RemoveAt(0);
            callBack(obj);
        }
        else {
            //bundle池有
            if (AssetMgr.has(resName))
            {
                PackAsset pab = AssetMgr.get(resName);
                pab.getObj(callBack);
            }
            else {
                //都没有 load
                LoadItemMgr.add(resName);
            }
        }
    }

    public void onDispose()
    {

    }
}

