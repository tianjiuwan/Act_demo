using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTask
{
    public string name;
    //完成回调
    private List<Action<string>> callBack = new List<Action<string>>();
    public void addHandler(Action<string> callBack)
    {
        this.callBack.Add(callBack);
    }
    //异步加载
    IEnumerator doLoad()
    {
        yield return new WaitForEndOfFrame();
    }
}

