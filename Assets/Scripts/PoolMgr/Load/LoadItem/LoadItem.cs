using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadItem
{
    private string resName;
    private List<string> deps;

    private LoadItem(string name)
    {
        deps = ManifestMgr.get(name);

    }


}

