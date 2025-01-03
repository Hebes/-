using System;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
    private static T _instance;
    private static readonly object _lock = new object();

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