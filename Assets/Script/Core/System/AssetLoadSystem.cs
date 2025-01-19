using UnityEngine;

/// <summary>
/// 资源加载系统
/// </summary>
public class AssetLoadSystem : SM<AssetLoadSystem>
{
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public  T[] LoadAll<T>(string path) where T : Object
    {
        return Resources.LoadAll<T>(path);
    }
}