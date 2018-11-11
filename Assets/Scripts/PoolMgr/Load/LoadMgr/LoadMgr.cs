using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoadMgr : Singleton<LoadMgr>, IDispose
{
    static Dictionary<string, LoadTask> loadMap = new Dictionary<string, LoadTask>();

    public static void doLoad(LoadTask task)
    {
        //loadMap.Add(task.resName, task);
        //task.addHandler(onLoadFinish);
        //StartCourtine(task.doLoad)
    }

    //加载任务回调Mgr
    public static void onLoadFinish(string resName)
    {
        if (loadMap.ContainsKey(resName))
        {
            loadMap.Remove(resName);
        }
    }

    //当前加载任务是否已经存在
    public static bool hasTask(string resName)
    {
        return loadMap.ContainsKey(resName);
    }
    //加载任务 添加一个回调
    public static void addHandler(string resName, Action<string> callBack)
    {
        //loadTask task = loadMap[resName];
        //task.addHandler(callBack);
    }


    private Dictionary<string, LoadItem> loaders = null;

    private bool hasItem(string name) {
        return loaders.ContainsKey(name);
    }

    private void addItem(string name) {
        //LoadItem item = new LoadItem(name);
    }

    protected override void initialize()
    {
        loaders = new Dictionary<string, LoadItem>();
    }

    public void onDispose()
    {

    }
}
