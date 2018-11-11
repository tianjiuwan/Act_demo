using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolMgr : Singleton<PoolMgr>, IDispose
{
    private Dictionary<string, BasePool> pools = null;

    protected override void initialize()
    {
        pools = new Dictionary<string, BasePool>();
    }


    public void onDispose()
    {
        foreach (var item in pools)
        {
            item.Value.onDispose();
        }
        pools.Clear();
    }
}
