using System;
using System.Collections.Generic;

public class ModelPool : BasePool
{
    public ModelPool(string resName, string resPath, E_PoolMode mode, E_PoolType pType, float time) : base(resName, resPath, mode, pType, time)
    {

    }

    protected override void onPerCreate(string name)
    {
        base.onPerCreate(name);
        if (preLoadCount > 0)
        {
            for (int i = 0; i < preLoadCount; i++)
            {
                getObj(recyle);
            }
            preLoadCount = 0;
        }
    }

}

