using System;
using System.Collections.Generic;

public class LoadItemMgr:Singleton<LoadItemMgr>
{
    private static Dictionary<string, LoadItem> loadMap = new Dictionary<string, LoadItem>();

    //是否已经存在loaditem
    private  bool hasItem(string resName)
    {
        return loadMap.ContainsKey(resName);
    }
    private  void addHandler(Action<string> callBack)
    {

    }
    private void addItem(string name) {
        if (hasItem(name)) {

        }
    }

    public static void add(string name) {
        Instance.addItem(name);
    }

}

