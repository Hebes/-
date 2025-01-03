// using System;
// using System.Collections.Generic;
// using System.Text;
// using UnityEditor;
// using UnityEditor.SceneManagement;
// using Object = UnityEngine.Object;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;
// using Debug = UnityEngine.Debug;
// using EditorTools;
//
//
// public class SpriteRenderTools
// {
//     /// <summary>
//     /// https://blog.csdn.net/qq_31042143/article/details/122498295 Editor模式批量修改Sprite导入参数
//     /// https://zhuanlan.zhihu.com/p/48921252 修改Sprite的Pivot
//     /// https://blog.csdn.net/linxinfa/article/details/114867642 Unity将Slice分割
//     /// </summary>
//     [MenuItem("Assets/修改图片锚点/GetAll", false, 1)]
//     private static void GetAll()
//     {
//         Object[] obj = Selection.objects;
//         foreach (Object texture in obj)
//         {
//             string selectionPath = AssetDatabase.GetAssetPath(texture);
//             TextureImporter textureIm = AssetImporter.GetAtPath(selectionPath) as TextureImporter;
//             TextureImporterSettings setting = new TextureImporterSettings();
//             //TextureImporterPlatformSettings setting = textureIm.GetDefaultPlatformTextureSettings();
//             //textureIm.SetPlatformTextureSettings(setting);
//             textureIm.ReadTextureSettings(setting);
//             setting.textureType = TextureImporterType.Sprite;
//             setting.spritePixelsPerUnit = 40f;
//             setting.spriteMode = (int)SpriteImportMode.Single;
//             setting.spriteAlignment = (int)SpriteAlignment.Custom;
//             setting.spritePivot = new Vector2(0.44f, 0.23f);
//             textureIm.SetTextureSettings(setting);
//             textureIm.SaveAndReimport();
//         }
//     }
// }
//
// public class SceneTools : UnityEditor.Editor
// {
//     // [MenuItem("GameObject/场景/切换/00Splash", false, 1)]
//     // public static void Switch00Splash() => SwitchScene($"Assets/Scenes/00Splash.unity");
//     //
//     // [MenuItem("GameObject/场景/切换/01InitScene", false, 1)]
//     // public static void Switch01InitScene() => SwitchScene($"Assets/Scenes/01InitScene.unity");
//     //
//     // [MenuItem("GameObject/场景/切换/02Game", false, 1)]
//     // public static void Switch02Game() => SwitchScene($"Assets/Scenes/02Game.unity");
//     //
//     // [MenuItem("GameObject/场景/添加/02Game", false, 1)]
//     // public static void AddScene1() => SwitchScene($"Assets/Scenes/02Game.unity");
//
//     private static void SwitchScene(string scenePath)
//     {
//         string currentScenePath = EditorSceneManager.GetActiveScene().path; // 获取当前场景路径
//         //string targetScenePath = "Assets/Scenes/TargetScene.unity";// 定义需要切换到的场景路径
//         EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
//         //EditorSceneManager.OpenScene(path); // 切换到目标场景
//         //Debug.Log("Switched from " + currentScenePath + " to " + targetScenePath);
//     }
//
//     private static void AddScene(string scenePath)
//     {
//         EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
//     }
// }
//
// public class GenerateTools
// {
//     [MenuItem("Tools/生成/GameObject", false, 1)]
//     public static void Generate4()
//     {
//         //GenerateGameObject($"{Application.dataPath}/Resources", "*.prefab");
//     }
//
//     [MenuItem("GameObject/Tools/生成UI", false, 1)]
//     private static void Generate1()
//     {
//         StringBuilder sb = new StringBuilder();
//     }
// }
//
// public class PrefabTools : UnityEditor.Editor
// {
//     [MenuItem("GameObject/实例化/Player", false, 1)]
//     public static void LoadInstantiatePrefab1() => InstantiatePrefab("Assets/Resources/Prefab/Core/Player.prefab");
//
//     [MenuItem("GameObject/实例化/Camera", false, 1)]
//     public static void LoadInstantiatePrefab2() => InstantiatePrefab("Assets/Resources/Prefab/Core/Camera.prefab");
//
//     [MenuItem("GameObject/打开/UI", false, 1)]
//     public static void LoadOpenPrefab1() => OpenAsset("Assets/Resources/Prefab/Core/UI.prefab");
//
//     public void ttt()
//     {
//         EditorApplication.ExecuteMenuItem("");
//     }
//
//
//     /// <summary>
//     /// 实例化资源
//     /// </summary>
//     /// <param name="path"></param>
//     /// <exception cref="NullReferenceException"></exception>
//     private static void InstantiatePrefab(string path)
//     {
//         // 加载Prefab资源（这里假设预制体在"Assets/Prefabs"文件夹下）
//         GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
//
//         // 检查Prefab是否加载成功
//         if (prefab == null)
//         {
//             throw new NullReferenceException();
//         }
//
//         // 实例化Prefab到场景中
//         GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
//
//         // 设置实例化后的Prefab位置
//         // if (Selection.activeTransform != null)
//         // {
//         //     instance.transform.position = Selection.activeTransform.position + Vector3.up; // 举例：将实例化Prefab的位置设置为当前选中对象的位置上方
//         // }
//     }
//
//     /// <summary>
//     /// 打开预制体
//     /// </summary>
//     /// <param name="path"></param>
//     private static void OpenAsset(string path)
//     {
//         Object prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
//         if (prefab != null)
//         {
//             AssetDatabase.OpenAsset(prefab);
//         }
//         else
//         {
//             Debug.LogError("没找到: " + path);
//         }
//     }
// }
//
// /// <summary>
// /// https://blog.csdn.net/final5788/article/details/129914973 语言国际化工具,生成多语言Excel自
// /// </summary>
// public static class EditorTool
// {
//     public static List<GameObject> ScanPrefab()
//     {
//         string[] str = new[]
//         {
//             "Assets/Resources/Prefab",
//         };
//         var assetGUIDs = AssetDatabase.FindAssets("t:Prefab", str);
//         List<GameObject> keyList = new List<GameObject>();
//         int totalCount = assetGUIDs.Length;
//         for (int i = 0; i < totalCount; i++)
//         {
//             string path = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
//             GameObject pfb = AssetDatabase.LoadAssetAtPath<GameObject>(path);
//             keyList.Add(pfb);
//         }
//
//         return keyList;
//     }
//
//     /// <summary>
//     /// 扫描Prefab
//     /// </summary>
//     public static List<string> ScanLocalizationTextFromPrefab(Action<string, int, int> onProgressUpdate = null)
//     {
//         string[] str = new[]
//         {
//             "Assets/Resources/Prefab",
//         };
//         var assetGUIDs = AssetDatabase.FindAssets("t:Prefab", str);
//         List<string> keyList = new List<string>();
//         int totalCount = assetGUIDs.Length;
//         for (int i = 0; i < totalCount; i++)
//         {
//             string path = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
//             GameObject pfb = AssetDatabase.LoadAssetAtPath<GameObject>(path);
//             // onProgressUpdate?.Invoke(path, totalCount, i);
//             // var keyArr = pfb.GetComponentsInChildren<UnityGameFramework.Runtime.UIStringKey>(true);
//             // foreach (var newKey in keyArr)
//             // {
//             //     if (string.IsNullOrWhiteSpace(newKey.Key) || keyList.Contains(newKey.Key)) continue;
//             //     keyList.Add(newKey.Key);
//             // }
//         }
//
//         return keyList;
//     }
// }