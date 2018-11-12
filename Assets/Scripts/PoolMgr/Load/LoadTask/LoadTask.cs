using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadTask
{
    public string resName;
    public string resPath;

    public LoadTask(string resName, string resPath)
    {
        this.resName = resName;
        this.resPath = resPath;
    }

    private E_LoadStatus status = E_LoadStatus.Wait;//bundle 状态
    //完成回调
    private List<Action<string>> handler = new List<Action<string>>();
    public void addHandler(Action<string> callBack)
    {
        this.handler.Add(callBack);
    }
    //异步加载
    public IEnumerator doLoad()
    {
        status = E_LoadStatus.Loading;
        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(resPath);
        while (!req.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        AssetMgr.add(new PackAsset(resName, resPath, req.assetBundle));
        for (int i = 0; i < handler.Count; i++)
        {
            handler[i].Invoke(resPath);
        }
        status = E_LoadStatus.Finish;
    }
}

