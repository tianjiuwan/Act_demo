using System;
using System.Collections.Generic;
using UnityEngine;

public class AtlasPool : BasePool
{   
    private new  Dictionary<string, List<Action<Sprite>>> handler = null; 

    public AtlasPool(string resName, string resPath, E_PoolMode mode, E_PoolType pType, float time) : base(resName, resPath, mode, pType, time)
    {

    }

    protected override void initialize()
    {
        handler = new Dictionary<string, List<Action<Sprite>>>();
    }

    public override void getObj(string name, Action<Sprite> callBack, bool forceRemove = false)
    {
        //bundle池有
        if (AssetMgr.has(resName))
        {
            PackAsset pab = AssetMgr.get(resName);
            pab.getObj<Sprite>(name, callBack);
        }
        else
        {
            if (forceRemove)
            {
                Debug.LogError("加载资源失败 path " + resName);
            }
            else
            {
                if (!handler.ContainsKey(name))
                {
                    handler.Add(name, new List<Action<Sprite>>());
                }
                handler[name].Add(callBack);
                LoadItemMgr.add(resName, resPath, loadFinish);
            }
        }
    }

    protected override void loadFinish(string name)
    {
        foreach (var item in handler)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                getObj(item.Key,item.Value[i], true);
            }
        }
    }
}


