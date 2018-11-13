﻿using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 池子基类
/// </summary>
public class BasePool : IDispose
{
    protected string resName;
    protected string resPath;
    protected E_PoolMode pMode = E_PoolMode.None;
    protected E_PoolType pType = E_PoolType.None;
    protected double lastUseTime = -1;
    protected float lifeTime = 60;
    protected Transform poolRoot;
    protected List<string> deps = null;
    protected List<GameObject> cacheLst = null;
    protected List<Action<GameObject>> handler = null;

    public BasePool(string resName, string resPath, E_PoolMode mode, E_PoolType pType, float time)
    {
        this.resName = resName;
        this.resPath = resPath;
        pMode = mode;
        this.pType = pType;
        lifeTime = time;
        string poolName = string.Format("[{0}][{1}][{2}]", mode.ToString(), pType.ToString(), resName);
        GameObject go = new GameObject(poolName);
        this.poolRoot = go.transform;
        this.poolRoot.SetParent(PoolMgr.Instance.PoolRoot);
        this.poolRoot.transform.localPosition = Vector3.zero;
        initialize();
    }

    //池子初始化
    protected virtual void initialize()
    {
        deps = new List<string>();
        ManifestMgr.getDeps(resName, ref deps);
        cacheLst = new List<GameObject>();
    }

    public virtual void getObj(string name, Action<Sprite> callBack, bool forceRemove = false)
    {

    }

    public virtual void getObj(Action<GameObject> callBack, bool forceRemove = false)
    {
        if (cacheLst.Count > 0)
        {
            //缓存池有
            GameObject obj = cacheLst[0];
            cacheLst.RemoveAt(0);
            obj.transform.SetParent(null);
            callBack(obj);
        }
        else
        {
            //bundle池有
            if (AssetMgr.has(resName))
            {
                PackAsset pab = AssetMgr.get(resName);
                pab.getObj(callBack);
            }
            else
            {
                if (forceRemove)
                {
                    Debug.LogError("加载资源失败 path " + resName);
                }
                else
                {
                    //都没有 先load ab
                    if (handler == null)
                    {
                        handler = new List<Action<GameObject>>();
                    }
                    handler.Add(callBack);
                    LoadItemMgr.add(resName, resPath, loadFinish);
                }
            }
        }
    }

    //ab load完成
    protected virtual void loadFinish(string name)
    {
        if (handler != null)
        {
            for (int i = 0; i < handler.Count; i++)
            {
                getObj(handler[i], true);
            }
            handler.Clear();
        }
    }

    //移除回调
    public virtual void removeHandler(Action<GameObject> callBack)
    {
        if (handler != null)
        {
            if (handler.Contains(callBack))
            {
                handler.Remove(callBack);
            }
        }
    }

    //回收
    public virtual void recyle(GameObject go)
    {
        cacheLst.Add(go);
        go.transform.SetParent(this.poolRoot);
        go.transform.localPosition = Vector3.zero;
    }

    //销毁池
    public virtual void onDispose()
    {
        LoadItemMgr.remove(resName, loadFinish);
        int count = cacheLst.Count;
        for (int i = 0; i < deps.Count; i++)
        {
            AssetMgr.subRef(resName, count);
        }
        for (int i = 0; i < cacheLst.Count; i++)
        {
            GameObject.Destroy(cacheLst[i]);
        }
        cacheLst.Clear();
        cacheLst = null;
        deps.Clear();
        deps = null;
    }
}

