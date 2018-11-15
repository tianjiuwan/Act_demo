using System;
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
    protected Action<string> preLoadHandler = null;
    protected int preLoadCount = 0;

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

    public virtual void getObj(Action<GameObject> callBack, bool forceRemove = false, int count = 0)
    {
        if (cacheLst.Count > count)
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

    //预加载
    public virtual void preLoad(Action<string> preLoad, int preLoadCount = 0)
    {
        this.preLoadCount = preLoadCount;
        this.preLoadHandler = preLoad;
        if (!AssetMgr.has(resName))
        {
            if (cacheLst != null && cacheLst.Count > 0) {
                Debug.LogError("预创建错误 gameobject池子有缓存 但是assetbundle已经不存在(可能造成预设资源丢失)");
            }
            LoadItemMgr.add(resName, resPath, onPerCreate);
        }
        else
        {
            onPerCreate(resName);
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

    //预创建完成
    protected virtual void onPerCreate(string name)
    {
        loadFinish(name);
        if (preLoadHandler != null)
        {
            preLoadHandler.Invoke(name);
            preLoadHandler = null;
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
        if (cacheLst != null)
        {
            int count = cacheLst.Count;
            //释放静态引用
            if (count > 0)
            {
                for (int i = 0; i < deps.Count; i++)
                {
                    AssetMgr.subRef(deps[i], count);
                }
                AssetMgr.subRef(resName, count);
            }
            //释放动态引用
            for (int i = 0; i < cacheLst.Count; i++)
            {
                PoolObj po = cacheLst[i].GetComponent<PoolObj>();
                if (po.getDepsNum() > 0)
                {
                    List<string> dyDeps = po.getDeps();
                    for (int j = 0; j < dyDeps.Count; j++)
                    {
                        AssetMgr.subRef(dyDeps[i]);
                    }
                }
            }
            //销毁gameobject
            for (int i = 0; i < cacheLst.Count; i++)
            {
                GameObject.Destroy(cacheLst[i]);
            }
            cacheLst.Clear();
            cacheLst = null;
        }
        if (deps != null)
        {
            deps.Clear();
            deps = null;
        }
        GameObject.Destroy(this.poolRoot.gameObject);
    }
}

