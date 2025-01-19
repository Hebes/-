using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// CSV文件读取
/// </summary>
public static partial class Tools
{
    public static IList<T> Csv2List<T>(string fileName, Func<string[], T> setFunc)
    {
        List<T> list = new List<T>();
        TextAsset textAsset = Asset.LoadFromResources<TextAsset>("Conf/DB", fileName);
        using (StringReader stringReader = new StringReader(textAsset.text))
        {
            string text;
            while ((text = stringReader.ReadLine()) != null)
            {
                string[] arg = text.Split(new char[] { ',' });
                list.Add(setFunc(arg));
            }
        }

        return list;
    }

    public static IDictionary<TKey, TValue> Csv2Dictionary<TKey, TValue>(string fileName, Func<string, TKey> setKey, Func<string[], TValue> setValue)
    {
        Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
        TextAsset textAsset = Asset.LoadFromResources<TextAsset>("Conf/DB", fileName);
        using (StringReader stringReader = new StringReader(textAsset.text))
        {
            string text;
            while ((text = stringReader.ReadLine()) != null)
            {
                string[] array = text.Split(new char[] { ',' });
                dictionary.Add(setKey(array[0]), setValue(array));
            }
        }

        return dictionary;
    }
}