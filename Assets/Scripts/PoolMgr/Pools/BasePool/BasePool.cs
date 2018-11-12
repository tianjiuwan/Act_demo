using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 池子基类
/// </summary>
public class BasePool : IDispose
{
    private string resName;
    private string resPath;
    private E_PoolMode pMode = E_PoolMode.None;
    private E_PoolType pType = E_PoolType.None;
    private double lastUseTime = -1;
    private float lifeTime = 60;
    private Transform poolRoot;
    private List<string> deps = null;
    private List<GameObject> cacheLst = null;
    private List<Action<GameObject>> handler = null;

    public BasePool(string resName,string resPath, E_PoolMode mode, E_PoolType pType, float time)
    {
        this.resName = resName;
        this.resPath = resPath;
        pMode = mode;
        this.pType = pType;
        lifeTime = time;
        deps = new List<string>();
        ManifestMgr.getDeps(resName, ref deps);
        cacheLst = new List<GameObject>();
        string poolName = string.Format("[{0}][{1}][{2}]", mode.ToString(), pType.ToString(), resName);
        GameObject go = new GameObject(poolName);
        this.poolRoot = go.transform;
        this.poolRoot.SetParent(PoolMgr.Instance.PoolRoot);
        this.poolRoot.transform.localPosition = Vector3.zero;
    }

    public void getObj(Action<GameObject> callBack,bool forceRemove=false)
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
                else {
                    //都没有 先load ab
                    if (handler == null)
                    {
                        handler = new List<Action<GameObject>>();
                    }
                    handler.Add(callBack);
                    LoadItemMgr.add(resName,resPath, loadFinish);
                }
            }
        }
    }

    //ab load完成
    private void loadFinish(string name)
    {
        if (handler != null)
        {
            for (int i = 0; i < handler.Count; i++)
            {
                getObj(handler[i],true);
            }
            handler.Clear();
        }
    }

    //移除回调
    public void removeHandler(Action<GameObject> callBack)
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
    public void recyle(GameObject go)
    {
        cacheLst.Add(go);
        go.transform.SetParent(this.poolRoot);
        go.transform.localPosition = Vector3.zero;
    }

    //销毁池
    public void onDispose()
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

