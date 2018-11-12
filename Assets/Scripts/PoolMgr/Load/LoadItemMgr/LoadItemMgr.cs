using System;
using System.Collections.Generic;

public class LoadItemMgr : Singleton<LoadItemMgr>
{
    private static Dictionary<string, LoadItem> loadMap = new Dictionary<string, LoadItem>();

    //是否已经存在loaditem
    private bool hasItem(string resName)
    {
        return loadMap.ContainsKey(resName);
    }

    //添加
    private void addItem(string resName, string resPath, Action<string> onFinish)
    {
        if (!hasItem(resName))
        {
            LoadItem item = new LoadItem(resName, resPath, onFinish);
            loadMap.Add(resName, item);
        }
        else
        {
            loadMap[resName].addHandler(onFinish);
        }
    }
    //移除
    private void removeItem(string name, Action<string> onFinish)
    {
        if (hasItem(name))
        {
            loadMap[name].removeHandler(onFinish);
        }
    }

    #region 接口
    public static void add(string resName, string resPath, Action<string> onFinish)
    {
        Instance.addItem(resName, resPath, onFinish);
    }
    public static bool has(string name)
    {
        return Instance.hasItem(name);
    }
    public static void remove(string name, Action<string> onFinish)
    {
        Instance.removeItem(name, onFinish);
    }
    #endregion
}

