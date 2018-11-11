using System;
using System.Collections.Generic;

/// <summary>
/// ab依赖
/// </summary>
public class ManifestMgr : Singleton<ManifestMgr>
{
    private List<string> getDeps(string name)
    {
        return new List<string>();
    }

    public static List<string> get(string name)
    {
        return Instance.getDeps(name);
    }

}

