using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackAsset : IDispose
{
    private string name;
    private AssetBundle ab;
    private UnityEngine.Object obj;
    private int refCount = 0;
    private E_LoadStatus status = E_LoadStatus.Wait;
    private List<Action<GameObject>> handler = null;

    public void addRef()
    {
        refCount++;
    }
    public void subRef()
    {
        refCount--;
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

    public void remove(Action<GameObject> call) {
        if (handler != null) {
            if (handler.Contains(call)) {
                handler.Remove(call);
            }
        }
    }

    public void getObj(Action<GameObject> callBack)
    {
        if (obj != null)
        {
            GameObject go = GameObject.Instantiate(obj) as GameObject;
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
        AssetBundleRequest req = ab.LoadAssetAsync(name);
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
                handler[i].Invoke(go);
            }
            handler.Clear();
        }
    }

    public void onDispose()
    {

    }
}

