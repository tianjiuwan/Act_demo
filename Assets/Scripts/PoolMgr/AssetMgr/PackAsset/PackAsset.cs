using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PackAsset : IDispose
{
    private string resName;
    private string resPath;
    private string mainName;
    public string Name
    {
        get
        {
            return resName;
        }
    }
    private AssetBundle ab;
    private UnityEngine.Object obj;
    private int refCount = 0;
    private E_LoadStatus status = E_LoadStatus.Wait;//bundle obj状态
    private List<Action<GameObject>> handler = null;

    public PackAsset(string resName, string resPath, AssetBundle ab)
    {
        this.resName = resName;
        this.resPath = resPath;
        this.ab = ab;
        mainName = ab.GetAllAssetNames()[0];
    }

    public void addRef()
    {
        refCount++;
    }
    public void subRef(int count = 1)
    {
        refCount -= count;
        //<0 dispose
    }

    private void addHandler(Action<GameObject> call)
    {
        if (handler == null)
        {
            handler = new List<Action<GameObject>>();
        }
        handler.Add(call);
    }

    public void remove(Action<GameObject> call)
    {
        if (handler != null)
        {
            if (handler.Contains(call))
            {
                handler.Remove(call);
            }
        }
    }

    public void getObj(Action<GameObject> callBack)
    {
        if (obj != null)
        {
            GameObject go = GameObject.Instantiate(obj) as GameObject;
            GameObject.DontDestroyOnLoad(go);
            PoolObj po = go.AddComponent<PoolObj>();
            po.resName = resName;
            callBack(go);
        }
        else
        {
            addHandler(callBack);
            if (status == E_LoadStatus.Wait)
            {
                status = E_LoadStatus.Loading;
                LoadThread.Instance.StartCoroutine(loadObj());
            }
        }
    }

    IEnumerator loadObj()
    {
        AssetBundleRequest req = ab.LoadAssetAsync(mainName);
        while (!req.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        this.obj = req.asset;
        status = E_LoadStatus.Finish;
        if (handler != null)
        {
            for (int i = 0; i < handler.Count; i++)
            {
                GameObject go = GameObject.Instantiate(this.obj) as GameObject;
                GameObject.DontDestroyOnLoad(go);
                PoolObj po= go.AddComponent<PoolObj>();
                po.resName = resName;
                handler[i].Invoke(go);
            }
            handler.Clear();
        }
    }

    public void onDispose()
    {

    }
}

