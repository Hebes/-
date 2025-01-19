using System;
using UnityEngine;

/// <summary>
/// SingletonMono
/// 可以在子类中添加标签表示这个类不会被DontDestroyOnLoad
/// [NoDontDestroyOnLoad]
/// </summary>
/// <typeparam name="T"></typeparam>
public class SM<T> : MonoBehaviour where T : SM<T>
{
    private static T _instance;
    private static readonly object _lock = new object();
    private GameObject _gameObject;
    private Transform _transform;

    public static T I
    {
        get
        {
            if (_instance) return _instance;

            lock (_lock)
            {
                _instance ??= FindObjectOfType<T>();
            }

            if (!_instance)
            {
                _instance = new GameObject(typeof(T).Name).AddComponent<T>();
            }

            //更具特性决定是否DontDestroyOnLoad
            Type type = _instance.GetType();
            if (!Attribute.IsDefined(type, typeof(NoDontDestroyOnLoad)) || _instance.transform.parent == null)
            {
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
        protected set
        {
            if (_instance) return;
            _instance = value;
            DontDestroyOnLoad(_instance);
        }
    }

    public new GameObject gameObject
    {
        get
        {
            GameObject result;
            if ((result = _gameObject) == null)
            {
                result = (_gameObject = base.gameObject);
            }

            return result;
        }
    }

    public new Transform transform
    {
        get
        {
            Transform result;
            if ((result = _transform) == null)
            {
                result = (_transform = base.transform);
            }

            return result;
        }
    }
}


public class Singleton<T> where T : new()
{
    private static T _instance;
    public static T I => _instance ?? new T();
}

public class Singleton1<T> where T : class, new()
{
    public static readonly T I = Activator.CreateInstance<T>();
}

//注解
public class NoDontDestroyOnLoad : Attribute
{
}