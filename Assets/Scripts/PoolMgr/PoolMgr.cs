using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class PoolMgr : Singleton<PoolMgr>, IDispose
{
    private Dictionary<string, BasePool> pools = null;
    public Transform PoolRoot;

    protected override void initialize()
    {
        pools = new Dictionary<string, BasePool>();
        GameObject go = new GameObject("PoolRoot");
        go.transform.position = new Vector3(10000, 10000, 10000);
        PoolRoot = go.transform;
        GameObject.DontDestroyOnLoad(go);
    }

    //获取gameObject
    public void getObj(string resName, Action<GameObject> callBack, E_PoolMode mode = E_PoolMode.Time, E_PoolType pType = E_PoolType.None, float time = 60)
    {
        resName = resName.ToLower();
        if (!pools.ContainsKey(resName))
        {
            string resPath = Path.Combine(Define.abPre, resName).ToLower();
            BasePool p = PoolFactory.create(resName, resPath, mode, pType, time);
            pools.Add(resName, p);
        }
        pools[resName].getObj(callBack);
    }

    //获取Sprite
    //resName = ab name
    //objName = asset name
    public void getObj(string spName, Action<Sprite> callBack, E_PoolMode mode = E_PoolMode.Time, E_PoolType pType = E_PoolType.Atlas, float time = 60) 
    {
        if (AtlasMgr.hasKey(spName))
        {
            string resName = AtlasMgr.getName(spName);
            if (!pools.ContainsKey(resName))
            {
                string resPath = Path.Combine(Define.abPre, resName).ToLower();
                BasePool p = PoolFactory.create(resName, resPath, mode, pType, time);
                pools.Add(resName, p);
            }
            pools[resName].getObj(spName,callBack);
        }
        else {
            Debug.LogError("没有找到bundle名称 spName " + spName);
        }
    }

    //移除
    public void remove(string resName, Action<GameObject> callBack)
    {
        if (pools.ContainsKey(resName))
        {
            pools[resName].removeHandler(callBack);
        }
    }

    //回收
    public void recyleObj(GameObject go)
    {
        PoolObj obj = go.GetComponent<PoolObj>();
        if (obj != null)
        {
            if (pools.ContainsKey(obj.resName))
            {
                pools[obj.resName].recyle(go);
            }
            else
            {
                GameObject.Destroy(go);
            }
        }
        else
        {
            GameObject.Destroy(go);
        }
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
