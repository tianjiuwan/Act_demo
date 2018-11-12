using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadItem
{
    private string resName;
    private string resPath;
    public LoadItem(string resName, string resPath,Action<string> callBack) {
        this.resName = resName;
        this.resPath = resPath;
        initDeps();
        addHandler(callBack);
        onStart();       
    }

    //资源加载队列 可能依赖多个资源
    //private Queue<string> loadQueue = new Queue<string>();
    private List<string> loadQueue = new List<string>();
    //loadItem完成回调
    private List<Action<string>> handler = new List<Action<string>>();

    //添加完成回调
    public void addHandler(Action<string> callBack)
    {
        this.handler.Add(callBack);
    }
    //移除完成回调(这里提供一个移除回调)
    public void removeHandler(Action<string> callBack)
    {
        if (this.handler.Contains(callBack))
        {
            this.handler.Remove(callBack);
        }
    }

    //初始化依赖任务 todo
    private void initDeps() {
        ManifestMgr.getDeps(resName, ref loadQueue);
        loadQueue.Add(resName);
    }

    //创建则开始执行第一个加载任务
    private void onStart()
    {
        taskFinish("");
    }

    void doCheck(string name)
    {
        //1 ab池子是否已经有了
        bool hasAB = AssetMgr.has(name);
        if (hasAB)
        {
            AssetMgr.addRef(name);
            taskFinish("");
        }
        else
        {
            //如果加载任务里面有此任务 则加回调
            if (LoadMgr.hasTask(name))
            {
                LoadMgr.addHandler(name, taskFinish);
            }
            else
            {
                string resPath = Path.Combine(Define.abPre, name).ToLower();
                LoadMgr.doLoad(name, resPath, taskFinish);
            }
        }
    }

    //当一个加载任务完成
    void taskFinish(string resName)
    {
        if (loadQueue.Count > 0)
        {
            string temp = loadQueue[0];
            loadQueue.RemoveAt(0);
            doCheck(temp);
        }
        else
        {
            itemFinish();
        }
    }

    //整个加载任务完成 回调
    void itemFinish()
    {
        if (handler != null) {
            for (int i = 0; i < handler.Count; i++)
            {
                handler[i].Invoke(resName);
            }
        }
    }


}

