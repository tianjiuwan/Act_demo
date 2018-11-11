using System;

/// <summary>
/// 单例类
/// </summary>
public class Singleton<T> where T : class, new()
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }

    public Singleton()
    {
        initialize();
    }

    //初始化
    protected virtual void initialize()
    {

    }
}

