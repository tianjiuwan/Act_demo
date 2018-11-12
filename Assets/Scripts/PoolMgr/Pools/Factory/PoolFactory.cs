using System;

public class PoolFactory
{
    public static BasePool create(string resName, string resPath, E_PoolMode mode, E_PoolType pType, float time)
    {
        BasePool p = null;
        switch (pType)
        {
            case E_PoolType.UICache:
                p = new UICachePool(resName, resPath, mode, pType, time);
                break;
            default:
                p = new BasePool(resName, resPath, mode, pType, time);
                break;
        }
        return p;
    }
}

