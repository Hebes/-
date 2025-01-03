using System;
using System.Collections.Generic;

public class CharacterSystem : SingletonMono<CharacterSystem>
{
    private readonly Dictionary<string, Character> _characterCid = new Dictionary<string, Character>();

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

    private CHARACTER_INFO GetCharacterInfo(string characterName)
    {
        CHARACTER_INFO result = new CHARACTER_INFO();
        result.Name = characterName;
        result.Config = R.DialogueSystem.Config.CharacterConfigurationAssetr.GetConfig(characterName);
        return result;
    }

    public Character CreatCharacterFromInfo(CHARACTER_INFO info)
    {
        CharacterConfigData config = info.Config;
        switch (config.characterType)
        {
            case Character.CharacterType.Text:
                return new Character_Text(info.Name);
            case Character.CharacterType.Sprite:
                return new Character_Sprite(info.Name);
            case Character.CharacterType.SpriteSheet:
            case Character.CharacterType.Live2D:
                return new Character_Live2D(info.Name);
            case Character.CharacterType.ModeL3D:
                return new Character_ModeL3D(info.Name);
            default:
                throw new Exception("未找到对应角色信息的类型");
        }
    }


    /// <summary>
    /// 人物信息
    /// </summary>
    public class CHARACTER_INFO
    {
        public string Name = string.Empty;
        public CharacterConfigData Config = null;
    }
}