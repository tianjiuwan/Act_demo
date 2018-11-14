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

    /// <summary>
    /// 创建池
    /// </summary>
    /// <param name="resName"></param>
    /// <param name="mode"></param>
    /// <param name="pType"></param>
    /// <param name="time"></param>
    private void createPool(string resName, E_PoolMode mode = E_PoolMode.Time, E_PoolType pType = E_PoolType.None, float time = 60)
    {
        if (!pools.ContainsKey(resName))
        {
            string resPath = Path.Combine(Define.abPre, resName).ToLower();
            BasePool p = PoolFactory.create(resName, resPath, mode, pType, time);
            pools.Add(resName, p);
        }
    }

    /// <summary>
    /// 释放一个池子(手动销毁池子)
    /// 释放一个池子 只会销毁池子缓存的gameobject 并释放引用ab次数
    /// ab的释放 每段时间检查refCount<零释放 todo
    /// </summary>
    /// <param name="resName"></param>
    public void disposePool(string resName) {
        if (pools.ContainsKey(resName)) {
            BasePool bp = pools[resName];
            pools.Remove(resName);
            bp.onDispose();
        }
    }

    //获取gameObject
    public void getObj(string resName, Action<GameObject> callBack, E_PoolMode mode = E_PoolMode.Time, E_PoolType pType = E_PoolType.None, float time = 60)
    {
        resName = resName.ToLower();
        createPool(resName, mode, pType, time);
        pools[resName].getObj(callBack);
    }

    //获取Sprite
    public void getObj(string spName, Action<Sprite> callBack, E_PoolMode mode = E_PoolMode.Time, E_PoolType pType = E_PoolType.Atlas, float time = 60)
    {
        if (AtlasMgr.hasKey(spName))
        {
            string resName = AtlasMgr.getName(spName);
            createPool(resName, mode, pType, time);
            pools[resName].getObj(spName, callBack);
        }
        else
        {
            Debug.LogError("没有找到bundle名称 spName " + spName);
        }
    }

    /// <summary>
    /// 预创建池子
    /// 可能在进入场景的时候需要预加载一些gameobject或者load一些bundle
    /// </summary>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    /// <param name="mode"></param>
    /// <param name="pType"></param>
    /// <param name="preLoadCount"></param> 初始化个数
    /// <param name="time"></param>
    public void preLoad(string resName, Action<string> callBack, E_PoolMode mode, E_PoolType pType, int preLoadCount = 0, float time = 60)
    {
        resName = resName.ToLower();
        if (!pools.ContainsKey(resName))
        {
            string resPath = Path.Combine(Define.abPre, resName).ToLower();
            BasePool p = PoolFactory.create(resName, resPath, mode, pType, time);
            pools.Add(resName, p);
        }
        pools[resName].preLoad(callBack, preLoadCount);
    }

    /// <summary>
    /// 移除一个正在加载的任务
    /// 上层的创建一个gameobject的时候 很有可能调了创建 马上又销毁(这个时候unload掉)
    /// </summary>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    public void unLoad(string resName, Action<GameObject> callBack)
    {
        resName = resName.ToLower();
        if (pools.ContainsKey(resName))
        {
            pools[resName].removeHandler(callBack);
        }
        if (AssetMgr.has(resName))
        {
            PackAsset pka = AssetMgr.get(resName);
            pka.remove(callBack);
        }
    }

    /// <summary>
    /// 回收策略
    /// 不是从池子出来的obj 直接销毁
    /// 如果池子已经被销毁了 创建池子 丢池子里面去(为了统一释放ab引用)
    /// </summary>
    /// <param name="go"></param>
    public void recyleObj(GameObject go)
    {
        PoolObj obj = go.GetComponent<PoolObj>();
        if (obj != null)
        {
            if (!pools.ContainsKey(obj.resName))
            {
                string resName = obj.resName;
                createPool(resName, E_PoolMode.Level, E_PoolType.None, 60);
            }
            pools[obj.resName].recyle(go);
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
