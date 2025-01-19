using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SpriteLoaderEditor : EditorWindow
{
    private string rootFolderPath = "Assets/";
    private CharacterConfigSo characterConfigFile;
    private int selectedCharacterIndex = 0;
    private ICollection sprites;

    [MenuItem("Tools/Sprite Loader")]
    public static void ShowWindow()
    {
        GetWindow<SpriteLoaderEditor>("Sprite Loader");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("图片加载", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        rootFolderPath = EditorGUILayout.TextField("根目录路径:", rootFolderPath);
        if (GUILayout.Button("浏览", GUILayout.Width(80)))
        {
            string selectedFolder = EditorUtility.OpenFolderPanel("选择文件夹", rootFolderPath, "");
            if (!string.IsNullOrEmpty(selectedFolder) && selectedFolder.Contains(Application.dataPath))
            {
                rootFolderPath = "Assets" + selectedFolder.Substring(Application.dataPath.Length);
            }
            else if (!string.IsNullOrEmpty(selectedFolder))
            {
                Debug.LogError("所选文件夹必须位于Assets目录内!");
            }
        }

        EditorGUILayout.EndHorizontal();

        characterConfigFile = (CharacterConfigSo)EditorGUILayout.ObjectField("Character Config:", characterConfigFile, typeof(CharacterConfigSo), false);

        if (characterConfigFile != null && characterConfigFile.characterArray != null && characterConfigFile.characterArray.Length > 0)
        {
            string[] characterNames = GetCharacterNames(characterConfigFile.characterArray);
            selectedCharacterIndex = EditorGUILayout.Popup("角色名称:", selectedCharacterIndex, characterNames);
        }

        if (GUILayout.Button("加载图片"))
        {
            LoadSpritesFromFolder(rootFolderPath);
            Debug.Log(sprites != null ? $"{sprites.Count} 精灵加载!" : "没有精灵加载!");
        }

        if (sprites != null && sprites.Count > 0)
        {
            EditorGUILayout.LabelField($"加载 {sprites.Count} 精灵:");

            // foreach (object pair in sprites)
            // {
            //     EditorGUILayout.LabelField($"关键字: {pair.Key}, 精灵: {pair.Value.name}");
            // }
        }
    }

    private string[] GetCharacterNames(CharacterConfigData[] characters)
    {
        List<string> names = new List<string>();
        foreach (var character in characters)
        {
            names.Add(character.name);
        }

        return names.ToArray();
    }

    private void LoadSpritesFromFolder(string folderPath)
    {
        if (characterConfigFile != null && characterConfigFile.characterArray != null && selectedCharacterIndex < characterConfigFile.characterArray.Length)
        {
            string characterName = characterConfigFile.characterArray[selectedCharacterIndex].name;
            CharacterConfigData config = characterConfigFile.GetConfig(characterName, safe: false);

            if (config.characterType == Character.CharacterType.SpriteSheet)
            {
                string[] texturePaths = Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories);

                foreach (string path in texturePaths)
                {
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    if (texture != null)
                    {
                        string subFolderPath = Path.GetDirectoryName(path).Replace(rootFolderPath, ""); //子文件夹路径
                        Sprite[] spritesFromTexture = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();

                        foreach (Sprite spriteFromSheet in spritesFromTexture)
                        {
                            //添加数据到数据结构中
                            string keyName = spriteFromSheet.name;
                            config.spriteList.Add(new SpriteData(keyName, spriteFromSheet));
                        }
                    }
                }
            }
            else // 假设characterType为“Sprite”
            {
                string[] spritePaths = Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories);

                foreach (string path in spritePaths)
                {
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    if (sprite != null)
                    {
                        string     keyName = path.Replace($"{rootFolderPath}\\", "").Replace(".png", "").ToLower();
                        string fullKeyName = keyName.Substring(1); // Removing the starting "/"
                        config.spriteList.Add(new SpriteData(keyName, sprite));
                    }
                }
            }

            sprites = config.spriteList;
        }
        else
        {
            Debug.LogWarning("无效CharacterConfigSO或字符选择.");
        }
    }
}