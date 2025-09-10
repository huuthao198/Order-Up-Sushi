using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantAndSingletonBehavior<T> : MonoBehaviour where T : PersistantAndSingletonBehavior<T>
{
    private static T _instance;

    public virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static T Instance => _instance;

    public static bool HasInstance() => _instance != null;
}

public class SingletonBehavior<T> : MonoBehaviour where T : SingletonBehavior<T>
{
    private static T _instance;

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static T Instance => _instance;

    public static bool HasInstance() => _instance != null;
}
