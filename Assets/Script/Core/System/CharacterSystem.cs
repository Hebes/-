using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 角色管理器
/// </summary>
public class CharacterSystem : SingletonMono<CharacterSystem>
{
    /// <summary>
    /// 人物信息
    /// </summary>
    public class CHARACTER_INFO
    {
        public string Name = string.Empty;
        public CharacterConfigData Config = null;
        public GameObject Prefab = null;
    }

    private readonly Dictionary<string, Character> _characterCid = new Dictionary<string, Character>();

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    public Character CreateCharacter(string characterName)
    {
        if (_characterCid.ContainsKey(characterName.ToLower()))
        {
            $"A Character called '{characterName}' already exists. Did not-create the character.".Warning();
            return null;
        }

        CHARACTER_INFO info = GetCharacterInfo(characterName);
        Character character = CreatCharacterFromInfo(info);
        return character;
    }

    /// <summary>
    /// 获取角色信息配置
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    public CharacterConfigData GetCharacterConfig(string characterName)
    {
        return R.DialogueSystem.Config.CharacterConfigurationAssetr.GetConfig(characterName);
    }


    /// <summary>
    /// 获取角色
    /// </summary>
    /// <param name="characterName"></param>
    /// <param name="createIfDoesNotExist"></param>
    /// <returns></returns>
    public Character GetCharacter(string characterName, bool createIfDoesNotExist = false)
    {
        if (_characterCid.ContainsKey(characterName.ToLower()))
            return _characterCid[characterName.ToLower()];
        if (createIfDoesNotExist)
            return CreateCharacter(characterName);
        return null;
    }


    private CHARACTER_INFO GetCharacterInfo(string characterName)
    {
        CHARACTER_INFO result = new CHARACTER_INFO();
        result.Name = characterName;
        result.Config = R.DialogueSystem.Config.CharacterConfigurationAssetr.GetConfig(characterName);
        result.Prefab = GetPrefabForCharacter(characterName);
        return result;
    }

    private GameObject GetPrefabForCharacter(string characterName)
    {
        string path = FormatCharacterPath(ConfigString.CharacterPrefabPath, characterName);
        return Resources.Load<GameObject>(path);
    }

    private string FormatCharacterPath(string path, string characterName)
    {
        return path.Replace(ConfigString.CHARACTER_NAME_ID, characterName);
    }


    private Character CreatCharacterFromInfo(CHARACTER_INFO info)
    {
        switch (info.Config.characterType)
        {
            case Character.CharacterType.Text:
                return new Character_Text(info.Name, info.Config, info.Prefab);
            case Character.CharacterType.Sprite:
                return new Character_Sprite(info.Name, info.Config, info.Prefab);
            case Character.CharacterType.SpriteSheet:
            case Character.CharacterType.Live2D:
                return new Character_Live2D(info.Name, info.Config, info.Prefab);
            case Character.CharacterType.ModeL3D:
                return new Character_ModeL3D(info.Name, info.Config, info.Prefab);
            default:
                throw new Exception("未找到对应角色信息的类型");
        }

        return default;
    }
}