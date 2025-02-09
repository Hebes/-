using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

/// <summary>
/// 历史缓存
/// </summary>
public class HistoryCache
{
    public static Dictionary<string, (object asset, int staleIndex)> loadedAssets = new Dictionary<string, (object asset, int staleIndex)>();

    public static T TryLoadObject<T>(string key)
    {
        object resource = null;

        if (loadedAssets.TryGetValue(key, out var loadedAsset))
            resource = (T)loadedAsset.asset;
        else
        {
            resource = R.Load(key);
            if (resource != null)
            {
                loadedAssets[key] = (resource, 0);
            }
        }

        if (resource != null)
        {
            if (resource is T resource1)
                return resource1;
            $"检索对象 '{key}' 不是预期的类型!".LogWarning();
        }

        $"无法从缓存加载对象 '{key}'".LogWarning();
        return default(T);
    }

    public static TMP_FontAsset LoadFont(string key) => TryLoadObject<TMP_FontAsset>(key);
    public static AudioClip LoadAudio(string key) => TryLoadObject<AudioClip>(key);
    public static Texture2D LoadImage(string key) => TryLoadObject<Texture2D>(key);
    public static VideoClip LoadVideo(string key) => TryLoadObject<VideoClip>(key);
}