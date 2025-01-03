using System;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
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
            lock (_lock) _instance ??= FindObjectOfType<T>();
            if (_instance) return _instance;
            var obj = new GameObject(typeof(T).Name);
            _instance = obj.AddComponent<T>();
            DontDestroyOnLoad(obj);
            return _instance;
        }
        protected set
        {
            if (!_instance) return;
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