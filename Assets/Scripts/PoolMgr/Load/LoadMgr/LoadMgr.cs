using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoadMgr : Singleton<LoadMgr>, IDispose
{
    private Dictionary<string, LoadTask> loadMap = null;

    private void addLoad(string resName,string resPath, Action<string> callBack)
    {
        LoadTask task = new LoadTask(resName, resPath);
        loadMap.Add(resName, task);
        task.addHandler(onLoadFinish);
        task.addHandler(callBack);
        LoadThread.Instance.StartCoroutine(task.doLoad());
    }

    //加载任务回调Mgr
    private void onLoadFinish(string resName)
    {
        if (loadMap.ContainsKey(resName))
        {
            loadMap.Remove(resName);
        }
    }

    //当前加载任务是否已经存在
    private bool has(string resName)
    {
        return loadMap.ContainsKey(resName);
    }
    //加载任务 添加一个回调
    private void addCall(string resName, Action<string> callBack)
    {
        LoadTask task = loadMap[resName];
        task.addHandler(callBack);
    }

    protected override void initialize()
    {
        loadMap = new Dictionary<string, LoadTask>();
    }

    public void onDispose()
    {

    }

    #region 接口
    public static void doLoad(string resName,string resPath, Action<string> callBack)
    {
        Instance.addLoad(resName, resPath, callBack);
    }
    public static bool hasTask(string resName)
    {
        return Instance.has(resName);
    }
    public static void addHandler(string resName, Action<string> callBack)
    {
        Instance.addCall(resName, callBack);
    }
    #endregion
}
