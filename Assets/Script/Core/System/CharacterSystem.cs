using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 角色管理器
/// </summary>
public class CharacterSystem : SM<CharacterSystem>
{
    /// <summary>
    /// 人物信息
    /// </summary>
    public class CHARACTER_INFO
    {
        public string Name = string.Empty;
        public string CastingName = string.Empty;
        public string RootCharacterFolder = string.Empty;
        public CharacterConfigData Config = null;
        public GameObject Prefab = null;
    }

    public const string CHARACTER_CASTING_ID = " as ";
    
    private readonly Dictionary<string, Character> characterDic = new Dictionary<string, Character>();

    public Character[] allCharacters => characterDic.Values.ToArray();
    
    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="characterName"></param>
    /// <param name="revealAfterCreation"></param>
    /// <returns></returns>
    public Character CreateCharacter(string characterName, bool revealAfterCreation = false)
    {
        if (characterDic.ContainsKey(characterName.ToLower()))
        {
            $"一个叫做 '{characterName}' 已经存在。不是自己创造的角色吗.".Warning();
            return null;
        }

        CHARACTER_INFO info = GetCharacterInfo(characterName);
        Character character = CreatCharacterFromInfo(info);
        characterDic.Add(info.Name.ToLower(), character);
        if (revealAfterCreation)
            character.Show();
        return character;
    }

    /// <summary>
    /// 获取角色信息配置
    /// </summary>
    /// <param name="characterName"></param>
    /// <param name="getOriginal">是否获取原始的</param>
    /// <returns></returns>
    public CharacterConfigData GetCharacterConfig(string characterName, bool getOriginal = false)
    {
        if (getOriginal)
        {
            return R.DialogueSystem.Config.CharacterConfigurationAssetr.GetConfig(characterName);
        }
        else
        {
            return GetCharacter(characterName)?.Config;
        }
    }


    /// <summary>
    /// 获取角色
    /// </summary>
    /// <param name="characterName"></param>
    /// <param name="createIfDoesNotExist"></param>
    /// <returns></returns>
    public Character GetCharacter(string characterName, bool createIfDoesNotExist = false)
    {
        if (characterDic.ContainsKey(characterName.ToLower()))
            return characterDic[characterName.ToLower()];
        return createIfDoesNotExist ? CreateCharacter(characterName) : null;
    }


    /// <summary>
    /// 获取角色信息
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    private CHARACTER_INFO GetCharacterInfo(string characterName)
    {
        CHARACTER_INFO result = new CHARACTER_INFO();
        string[] nameData = characterName.Split(ConfigString.CHARACTER_CASTING_ID, System.StringSplitOptions.RemoveEmptyEntries);
        result.Name = nameData[0];
        result.CastingName = nameData.Length > 1 ? nameData[1] : result.Name;
        result.Config = R.DialogueSystem.Config.CharacterConfigurationAssetr.GetConfig(result.CastingName);
        result.Prefab = GetPrefabForCharacter(result.CastingName);
        result.RootCharacterFolder = FormatCharacterPath(ConfigString.CharacterRootPathFormat, result.CastingName);
        return result;
    }

    private GameObject GetPrefabForCharacter(string characterName)
    {
        string path = FormatCharacterPath(ConfigString.CharacterPrefabPathFormat, characterName.Trim());
        return R.AssetLoadSystem.Load<GameObject>(path);
    }

    public string FormatCharacterPath(string path, string characterName)
    {
        return path.Replace(ConfigString.CHARACTER_NAME_ID, characterName);
    }


    private Character CreatCharacterFromInfo(CHARACTER_INFO info)
    {
        switch (info.Config.characterType)
        {
            case Character.CharacterType.Text:
                return new Character_Text(info.Name, info.Config, info.Prefab);
            case Character.CharacterType.SpriteSheet:
            case Character.CharacterType.Sprite:
                return new Character_Sprite(info.Name, info.Config, info.Prefab, info.RootCharacterFolder);
            case Character.CharacterType.Live2D:
                return new Character_Live2D(info.Name, info.Config, info.Prefab, info.RootCharacterFolder);
            case Character.CharacterType.Model3D:
                return new Character_ModeL3D(info.Name, info.Config, info.Prefab, info.RootCharacterFolder);
            default:
                throw new Exception("未找到对应角色信息的类型");
        }

        return default;
    }

    public bool HasCharacter(string characterName) => characterDic.ContainsKey(characterName.ToLower());

    public void SortCharacters(params string[] characterNames)
    {
        List<Character> sortedCharacters = new List<Character>();
        sortedCharacters = characterNames.Select(nameValue => GetCharacter(nameValue)).Where(character => character != null).ToList();
        List<Character> remainingCharacters = characterDic.Values.Except(sortedCharacters).OrderBy(character => character.priority).ToList();
        sortedCharacters.Reverse();
        int startingPriority = remainingCharacters.Count > 0 ? remainingCharacters.Max(c => c.priority) : 0;
        for (int i = 0; i < sortedCharacters.Count; i++)
        {
            Character character = sortedCharacters[i];
            character.SetPriority(startingPriority + i + 1, autoSortCharactersOnUI: false);
        }

        List<Character> allCharacters = remainingCharacters.Concat(sortedCharacters).ToList();
        SortCharacters(allCharacters);
    }

    public void SortCharacters()
    {
        List<Character> activeCharacters = characterDic.Values.Where(c => c.Root.gameObject.activeInHierarchy && c.isVisible).ToList();
        List<Character> inactiveCharacters = characterDic.Values.Except(activeCharacters).ToList();
        activeCharacters.Sort((a, b) => a.priority.CompareTo(b.priority));
        activeCharacters.Concat(inactiveCharacters);
        SortCharacters(activeCharacters);
    }

    private void SortCharacters(List<Character> charactersSortingOrder)
    {
        for (int i = 0; i < charactersSortingOrder.Count; i++)
            charactersSortingOrder[i].Root.SetSiblingIndex(i++);
    }
}